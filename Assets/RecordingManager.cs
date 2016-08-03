using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class RecordingManager : MonoBehaviour {

    [Serializable]
    internal struct _Vec3 {
        public float x, y, z;

        public _Vec3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(_Vec3 v) {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator _Vec3(Vector3 v) {
            return new _Vec3(v.x, v.y, v.z);
        }
    }

    [Serializable]
    internal struct _Vec4 {
        public float x, y, z, w;

        public _Vec4(float x, float y, float z, float w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Vector4(_Vec4 v) {
            return new Vector4(v.x, v.y, v.z, v.w);
        }

        public static implicit operator _Vec4(Vector4 v) {
            return new _Vec4(v.x, v.y, v.z, v.w);
        }

        public static implicit operator Quaternion(_Vec4 v) {
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

        public static implicit operator _Vec4(Quaternion v) {
            return new _Vec4(v.x, v.y, v.z, v.w);
        }
    }

    [Serializable]
    internal class RecState {
        public string obj_id;
        public _Vec3 pos;
        public _Vec4 rot;
        public bool enabled;

        public RecState(Recorder obj) {
            obj_id = obj.uniqueID;
            pos = obj.transform.position;
            rot = obj.transform.rotation;
            enabled = obj.gameObject.activeInHierarchy;
        }

        public void Unpack(Recorder obj) {
            if (obj.uniqueID != obj_id) {
                Debug.LogError("Cannot unpack recording state to unmatched object");
                return;
            }
            else {
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.gameObject.SetActive(enabled);
            }
        }

        public void Unpack(CustomPlayback obj) {
            if (obj.uniqueID != obj_id) {
                Debug.LogError("Cannot unpack recording state to unmatched object");
                return;
            }
            else {
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.gameObject.SetActive(enabled);
            }
        }
    }

    [Serializable]
    internal class RecFrame {
        public float time;
        public List<RecState> states = new List<RecState>();
    }

    [Serializable]
    internal class RecordingData {
        public int interval;
        public List<RecFrame> frames = new List<RecFrame>();
    }

    public enum Mode {
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

    private Recorder[] _objects;
    private CustomPlayback[] _playbackObjects;
    private Dictionary<string, Recorder> _objectMapping = new Dictionary<string, Recorder>();
    private Dictionary<string, CustomPlayback> _playbackMapping = new Dictionary<string, CustomPlayback>();

    private string _recordingFilePath;
    private RecordingData _recording;
    private int _ticCount;
    private int _playbackFrame = 0;
    private bool _stopped = false;

	// Use this for initialization
	void Start () {
        _ticCount = 0;
        _fixedFrameInterval = frameInterval;

	    if (mode == Mode.Record) {
            if (recordingFile == "") {
                _recordingFilePath = string.Format("{0}Recording_{1}.unityrec", rootFolder, GetTimestamp(DateTime.Now));
            }
            else {
                _recordingFilePath = string.Format("{0}{1}.unityrec", rootFolder, recordingFile);
            }
            _recording = new RecordingData();
            _recording.interval = frameInterval;

            _recording.frames.Capacity = 32000;

            // Fill out the object mapping dictionary
            _objects = transform.root.GetComponentsInChildren<Recorder>(true);
            foreach (Recorder rec in _objects) {
                _objectMapping.Add(rec.uniqueID, rec);
                Debug.Log("Recording "+rec.uniqueID);
            }
            // Disable custom playback objects
            _playbackObjects = transform.root.GetComponentsInChildren<CustomPlayback>(true);
            foreach (CustomPlayback player in _playbackObjects) {
                player.gameObject.SetActive(false);
            }
        }
        else if (mode == Mode.Playback) {
            // Deserialize the specified recording
            _recording = null;
            _recordingFilePath = string.Format("{0}{1}.unityrec", rootFolder, playbackFile);
            FileStream fs = null;
            try {
                fs = new FileStream(_recordingFilePath, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                _recording = (RecordingData)formatter.Deserialize(fs);
            }
            catch (Exception e) {
                Debug.Log("Failed to deserialize: " + e.Message);
                mode = Mode.Off;
                return;
            }
            finally {
                if (fs != null)
                    fs.Close();
            }
            // Fill out the object mapping dictionary
            _objects = transform.root.GetComponentsInChildren<Recorder>(true);
            foreach (Recorder rec in _objects) {
                _objectMapping.Add(rec.uniqueID, rec);
                Rigidbody body = rec.GetComponent<Rigidbody>();
                if (body != null) {
                    // completely disable physics on playback objects
                    body.isKinematic = true;
                    body.detectCollisions = false;
                }
            }
            // Fill out playback mapping dictionary
            _playbackObjects = transform.root.GetComponentsInChildren<CustomPlayback>(true);
            foreach (CustomPlayback player in _playbackObjects) {
                _playbackMapping.Add(player.uniqueID, player);
            }
        }
	}
	
	// Poll recorder objects at regular intervals
	void FixedUpdate () {
        if (_stopped) return;

        if (mode == Mode.Record) {
            if (_ticCount % _fixedFrameInterval == 0) {
                RecFrame frame = new RecFrame();
                frame.time = Time.unscaledTime;

                // Fill out the recording frame
                // States represent the state of a given object at each frame
                foreach (Recorder rec in _objects) {
                    RecState objectState = new RecState(rec);
                    frame.states.Add(objectState);
                }

                _recording.frames.Add(frame);
            }
        }
        else if (mode == Mode.Playback) {
            if (_ticCount % _recording.interval == 0) {
                if (_playbackFrame >= _recording.frames.Count) {
                    Debug.LogWarning("Recording over!");
                    _stopped = true;
                }
                else {
                    RecFrame frame = _recording.frames[_playbackFrame++];
                    foreach (RecState state in frame.states) {
                        // Get the object to update using the stored instance ID
                        Recorder obj;
                        CustomPlayback player;
                        if (_playbackMapping.TryGetValue(state.obj_id, out player)) {
                            state.Unpack(player);
                        }
                        else if (_objectMapping.TryGetValue(state.obj_id, out obj)) {
                            state.Unpack(obj);
                        }
                    }
                }
            }
        }

        _ticCount++;
	}

    void OnDisable() {
        if (mode == Mode.Record) {
            // Serialize the recording and write out to file
            FileStream fs = new FileStream(_recordingFilePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try {
                formatter.Serialize(fs, _recording);
            }
            catch (SerializationException e) {
                Debug.Log("Failed to serialize recording: " + e.Message);
                throw;
            }
            finally {
                fs.Close();
            }
        }
    }

    public static string GetTimestamp(DateTime value) {
        return value.ToString("yyyyMMddHHmmssfff");
    }
}
