using UnityEngine;
using System.Collections;
using System.IO;

public class ScreenCap : MonoBehaviour
{

    public Camera _camera;

    private int frame = 0;
    private int screenshot_n = 0;

    private RenderTexture rt;

    // Use this for initialization
    void Start()
    {
        _camera.gameObject.SetActive(true);
        rt = new RenderTexture(512, 512, 24);
        _camera.targetTexture = rt;
    }

    // Update is called once per frame
    void Update()
    {
        if (frame % 2 == 0)
        {
            StartCoroutine(Capture());
        }

        frame++;
    }

    private IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        RenderTexture.active = null;
        yield return 0;
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes("Capture/screen_"+screenshot_n+".png", bytes);
        screenshot_n++;

        DestroyObject(tex);
    }
}
