﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class RecordingManager : MonoBehaviour
{

    static RecordingManager _instance;
    public static RecordingManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Serializable]
    internal struct _Vec3
    {
        public float x, y, z;

        public _Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(_Vec3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator _Vec3(Vector3 v)
        {
            return new _Vec3(v.x, v.y, v.z);
        }
    }

    [Serializable]
    internal struct _Vec4
    {
        public float x, y, z, w;

        public _Vec4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Vector4(_Vec4 v)
        {
            return new Vector4(v.x, v.y, v.z, v.w);
        }

        public static implicit operator _Vec4(Vector4 v)
        {
            return new _Vec4(v.x, v.y, v.z, v.w);
        }

        public static implicit operator Quaternion(_Vec4 v)
        {
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

        public static implicit operator _Vec4(Quaternion v)
        {
            return new _Vec4(v.x, v.y, v.z, v.w);
        }
    }

    [Serializable]
    internal class RecState
    {
        public string obj_id;
        public _Vec3 pos;
        public _Vec4 rot;
        public bool enabled;

        public RecState(Recorder obj)
        {
            obj_id = obj.uniqueID;
            pos = obj.transform.position;
            rot = obj.transform.rotation;
            enabled = obj.gameObject.activeInHierarchy;
        }

        public void Unpack(Recorder obj)
        {
            if (obj.uniqueID != obj_id)
            {
                Debug.LogError("Cannot unpack recording state to unmatched object");
                return;
            }
            else
            {
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.gameObject.SetActive(enabled);
            }
        }

        public void Unpack(CustomPlayback obj)
        {
            if (obj.uniqueID != obj_id)
            {
                Debug.LogError("Cannot unpack recording state to unmatched object");
                return;
            }
            else
            {
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.gameObject.SetActive(enabled);
            }
        }
    }

    [Serializable]
    internal class RecFrame
    {
        public float time;
        public List<RecState> states = new List<RecState>();
        public List<string> messages = new List<string>();
    }

    [Serializable]
    internal class RecordingData
    {
        public int interval;
        public List<RecFrame> frames = new List<RecFrame>();
        public string metadata;
    }

    public enum Mode
    {
        Off,
        Record,
        Playback
    };

    public Mode mode;
    public string rootFolder = "F:\\";
    public string recordingFile = "";
    public string playbackFile = "somefile";

    public int frameInterval = 3;
    private int _fixedFrameInterval;

    public Camera mainCamera;

    private Recorder[] _objects;
    private CustomPlayback[] _playbackObjects;
    private Dictionary<string, Recorder> _objectMapping = new Dictionary<string, Recorder>();
    private Dictionary<string, CustomPlayback> _playbackMapping = new Dictionary<string, CustomPlayback>();

    private string _recordingFilePath;
    private RecordingData _recording;
    private int _ticCount;
    private int _playbackFrame = 0;
    private bool _stopped = false;

    private Queue<string> _messages = new Queue<string>(32);

    // Playback control
    private enum PlayMode
    {
        Pause,
        Play,
        Reverse
    }
    private PlayMode _playMode = PlayMode.Play;
    private int _playSpeed = 1;

    // Set the static instance variable
    public RecordingManager()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {
        _ticCount = 0;
        _fixedFrameInterval = frameInterval;

        if (mode == Mode.Record)
        {
            if (recordingFile == "")
            {
                _recordingFilePath = string.Format("{0}Recording_{1}.unityrec", rootFolder, GetTimestamp(DateTime.Now));
            }
            else
            {
                // Uniquely names the file
                int n = 0;
                string suffix = "";
                do
                {
                    _recordingFilePath = string.Format("{0}{1}{2}.unityrec", rootFolder, recordingFile, suffix);
                    suffix = string.Format("_{0}", n++);
                } while (File.Exists(_recordingFilePath));
            }
            _recording = new RecordingData();
            _recording.interval = frameInterval;

            _recording.frames.Capacity = 32000;

            // Fill out the object mapping dictionary
            _objects = transform.root.GetComponentsInChildren<Recorder>(true);
            foreach (Recorder rec in _objects)
            {
                if (_objectMapping.ContainsKey(rec.uniqueID))
                {
                    Debug.LogErrorFormat("Recorder ID {0} already used by {1}", rec.uniqueID, _objectMapping[rec.uniqueID]);
                }
                _objectMapping.Add(rec.uniqueID, rec);
                Debug.Log("Recording "+rec.uniqueID);
            }
            // Disable custom playback objects
            _playbackObjects = transform.root.GetComponentsInChildren<CustomPlayback>(true);
            foreach (CustomPlayback player in _playbackObjects)
            {
                player.gameObject.SetActive(false);
            }
        }
        else if (mode == Mode.Playback)
        {
            // Deserialize the specified recording
            _recording = null;
            _recordingFilePath = string.Format("{0}{1}.unityrec", rootFolder, playbackFile);
            FileStream fs = null;
            try
            {
                fs = new FileStream(_recordingFilePath, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                _recording = (RecordingData)formatter.Deserialize(fs);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to deserialize: " + e.Message);
                mode = Mode.Off;
                return;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            Debug.Log(_recording.metadata);
            // Fill out the object mapping dictionary
            _objects = transform.root.GetComponentsInChildren<Recorder>(true);
            foreach (Recorder rec in _objects)
            {
                if (_objectMapping.ContainsKey(rec.uniqueID))
                {
                    Debug.LogErrorFormat("Recorder ID {0} already used by {1}", rec.uniqueID, _objectMapping[rec.uniqueID]);
                }
                _objectMapping.Add(rec.uniqueID, rec);
                Rigidbody body = rec.GetComponent<Rigidbody>();
                if (body != null)
                {
                    // completely disable physics on playback objects
                    body.isKinematic = true;
                    body.detectCollisions = false;
                }
            }
            // Fill out playback mapping dictionary
            _playbackObjects = transform.root.GetComponentsInChildren<CustomPlayback>(true);
            foreach (CustomPlayback player in _playbackObjects)
            {
                _playbackMapping.Add(player.uniqueID, player);
            }

            // Display the arm in the main view
            mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Arm");
        }
        else if (mode == Mode.Off)
        {
            // Deactivate playback objects
            _playbackObjects = transform.root.GetComponentsInChildren<CustomPlayback>(true);
            foreach (CustomPlayback player in _playbackObjects)
            {
                player.gameObject.SetActive(false);
            }
        }
    }

    // Poll recorder objects at regular intervals
    void FixedUpdate()
    {
        if (_stopped) return;

        if (mode == Mode.Record)
        {
            if (_ticCount % _fixedFrameInterval == 0)
            {
                RecFrame frame = new RecFrame();
                frame.time = Time.unscaledTime;

                // Fill out the recording frame
                // States represent the state of a given object at each frame
                foreach (Recorder rec in _objects)
                {
                    RecState objectState = new RecState(rec);
                    frame.states.Add(objectState);
                }
                // Arbitrary strings can be logged by other systems
                while (_messages.Count > 0)
                {
                    frame.messages.Add(_messages.Dequeue());
                }

                _recording.frames.Add(frame);
            }
        }
        else if (mode == Mode.Playback)
        {
            if (_ticCount % _recording.interval == 0)
            {
                if (_playbackFrame >= _recording.frames.Count)
                {
                    //Debug.LogWarning("Recording over!");
                }
                else
                {
                    RecFrame frame = _recording.frames[_playbackFrame];
                    if (_playMode == PlayMode.Play)
                    {
                        Step(_playSpeed);
                    }
                    else if (_playMode == PlayMode.Reverse)
                    {
                        Step(-_playSpeed);
                    }

                    foreach (RecState state in frame.states)
                    {
                        // Get the object to update using the stored instance ID
                        Recorder obj;
                        CustomPlayback player;
                        if (_playbackMapping.TryGetValue(state.obj_id, out player))
                        {
                            state.Unpack(player);
                        }
                        else if (_objectMapping.TryGetValue(state.obj_id, out obj))
                        {
                            state.Unpack(obj);
                        }
                    }
                    // Playback messages
                    foreach (string message in frame.messages)
                    {
                        Debug.LogFormat("RECORDING> {0}", message);
                    }
                }
            }
        }

        _ticCount++;
    }

    void OnDisable()
    {
        if (mode == Mode.Record)
        {
            // Serialize the recording and write out to file
            FileStream fs = new FileStream(_recordingFilePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, _recording);
            }
            catch (SerializationException e)
            {
                Debug.Log("Failed to serialize recording: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
    }

    public static string GetTimestamp(DateTime value)
    {
        return value.ToString("yyyyMMddHHmmssfff");
    }

    // Enable logging of arbitrary information
    public static void Log(string str)
    {
        RecordingManager instance = Instance;

        if (instance == null)
        {
            Debug.LogFormat("<no recorder> {0}", str);
        }
        else if (instance.mode == Mode.Record)
        {
            Debug.LogFormat("<recorded> {0}", str);
            instance._messages.Enqueue(str);
        }
        else if (instance.mode == Mode.Off)
        {
            Debug.LogFormat("<recording off> {0}", str);
        }
    }

    private void Step(int dist)
    {
        _playbackFrame += dist;
        if (_playbackFrame >= _recording.frames.Count)
        {
            _playbackFrame = _recording.frames.Count - 1;
            _playMode = PlayMode.Pause;
        }
        else if (_playbackFrame < 0)
        {
            _playbackFrame = 0;
            _playMode = PlayMode.Pause;
        }
    }

    // Playback GUI
    void OnGUI()
    {
        if (mode == Mode.Playback)
        {
            // |< , <<, >/||, >>, >|
            int w = 50;
            int h = 30;
            int gap = 60;
            int y = 25;
            // Step back
            if (GUI.Button(new Rect(w, y, w, h), "|<"))
            {
                _playMode = PlayMode.Pause;
                Step(-1);
            }

            // Reverse
            if (GUI.Button(new Rect(w+gap, y, w, h), "<<"))
            {
                _playMode = PlayMode.Reverse;
            }

            // Pause/play
            if (_playMode == PlayMode.Pause)
            {
                if (GUI.Button(new Rect(w+gap*2, y, w, h), ">"))
                {
                    _playMode = PlayMode.Play;
                }
            }
            else
            {
                if (GUI.Button(new Rect(w+gap*2, y, w, h), "||"))
                {
                    _playMode = PlayMode.Pause;
                }
            }

            // Fast
            if (GUI.RepeatButton(new Rect(w+gap*3, y, w, h), ">>"))
            {
                _playSpeed = 4;
            }
            else
            {
                _playSpeed = 1;
            }

            // Step forward
            if (GUI.Button(new Rect(w+gap*4, y, w, h), ">|"))
            {
                _playMode = PlayMode.Pause;
                Step(1);
            }

            GUI.Label(new Rect(w+gap*5, y, 100, h), string.Format("{0}s", _recording.frames[_playbackFrame].time));
        }
    }
}
