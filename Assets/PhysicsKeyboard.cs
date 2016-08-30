using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

public class PhysicsKeyboard : PhysicsInput
{
    public class PhysicsKey
    {
        public string normal;
        public string shifted;
        public float width;
        public float x, y;
        public Vector2 dim;

        public GameObject obj;
    }

    public class KeyRow
    {
        public List<PhysicsKey> keys = new List<PhysicsKey>();
    }

    private List<KeyRow> keyRows = new List<KeyRow>();
    private GameObject keyContainer;
    private PhysicsKey hoveredKey = null, pressedKey = null;

    [Tooltip("YAML text asset specifying key mapping and layout")]
    public TextAsset layout;
    [Tooltip("Prefab for Keys, must contain resizeable 64x64 canvas with one Text child")]
    public GameObject keyPrefab;
    public GameObject panel = null;

    public float keySize = 1.0f;
    public float keyGap = 0.1f;

    protected override void OnStart()
    {
        base.OnStart();

        // Setup the keys - load YAML asset
        var loader = new StringReader(layout.ToString());
        var yaml = new YamlStream();
        yaml.Load(loader);

        var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
        var rows = (YamlSequenceNode)mapping.Children[new YamlScalarNode("rows")];
        foreach (YamlMappingNode row in rows)
        {
            var savedRow = new KeyRow();
            var keys = (YamlSequenceNode)row.Children[new YamlScalarNode("keys")];
            foreach (YamlMappingNode key in keys)
            {
                var savedKey = new PhysicsKey();
                savedKey.normal = ((YamlScalarNode)key.Children[new YamlScalarNode("std")]).Value;
                savedKey.shifted = ((YamlScalarNode)key.Children[new YamlScalarNode("shift")]).Value;
                savedKey.width = float.Parse(((YamlScalarNode)key.Children[new YamlScalarNode("width")]).Value);
                //Debug.LogFormat("{0} {1} w:{2}", savedKey.normal, savedKey.shifted, savedKey.width);

                savedRow.keys.Add(savedKey);
            }

            this.keyRows.Add(savedRow);
        }

        // Build the keyboard
        keyContainer = new GameObject("KeyContainer");
        keyContainer.transform.SetParent(this.transform, false);
        keyContainer.transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);

        float size = keySize / 100;
        float gap = keyGap / 100;

        float xLim = 0.0f;
        float yLim = 0.0f;

        float y = 0.0f;
        foreach (var row in this.keyRows)
        {
            float x = 0.0f;
            foreach (var key in row.keys)
            {
                // Instantiate key
                GameObject newKey = Instantiate(keyPrefab);
                newKey.transform.SetParent(keyContainer.transform, false);
                var pos = new Vector3();
                pos.x = x + size*key.width*0.5f;
                pos.y = y;
                newKey.transform.localPosition = pos;

                // Scale
                newKey.transform.localScale = new Vector3(size, size, size);

                // Set the Visible label string and the width
                var dim = newKey.GetComponentInChildren<RectTransform>().sizeDelta;
                dim.x *= key.width;
                newKey.GetComponentInChildren<RectTransform>().sizeDelta = dim;
                newKey.GetComponentInChildren<Text>().text = labelKey(false, key);

                // Save key properties
                key.x = x;
                key.y = y;
                key.dim = new Vector2(size*key.width, size);
                key.obj = newKey;

                x += size*key.width + gap;
            }
            if (x > xLim)
                xLim = x;
            yLim = y;
            y -= size + gap;
        }

        // Place key container to center keyboard
        keyContainer.transform.localPosition = new Vector3(xLim/2 + gap, -yLim/2 - gap);

        // Resize background panel
        if (panel)
        {
            var scale = panel.transform.localScale;
            panel.transform.localScale = new Vector3(xLim + 2*gap, -y + 2*gap, scale.z);
        }
    }

    protected override void OnHovering(Vector3 hoverPoint)
    {
        base.OnHovering(hoverPoint);

        if (hoveredKey != null && hoveredKey != pressedKey)
        {
            var pos = hoveredKey.obj.transform.localPosition;
            hoveredKey.obj.transform.localPosition = new Vector3(pos.x, pos.y, 0.0f);
        }

        // Find closest key to hover point
        hoveredKey = getKeyAtPosition(hoverPoint);
        if (hoveredKey != null && hoveredKey != pressedKey)
        {
            var pos = hoveredKey.obj.transform.localPosition;
            hoveredKey.obj.transform.localPosition = new Vector3(pos.x, pos.y, -0.003f);
        }
    }

    protected override void OnHoverExit()
    {
        base.OnHoverExit();
        if (hoveredKey != null)
        {
            var pos = hoveredKey.obj.transform.localPosition;
            hoveredKey.obj.transform.localPosition = new Vector3(pos.x, pos.y, 0.0f);
        }
        hoveredKey = null;
    }

    protected override void OnClickEnter()
    {
        base.OnClickEnter();
        if (hoveredKey != null)
        {
            if (pressedKey != null)
            {
                UnpressKey();
            }
            PressKey(hoveredKey);
        }
    }

    protected override void OnClickExit()
    {
        base.OnClickExit();
        if (pressedKey != null)
        {
            UnpressKey();
        }
    }

    private void PressKey(PhysicsKey key)
    {
        pressedKey = key;
        var pos = key.obj.transform.localPosition;
        key.obj.transform.localPosition = new Vector3(pos.x, pos.y, 0.003f);
    }

    private void UnpressKey()
    {
        var pos = pressedKey.obj.transform.localPosition;
        pressedKey.obj.transform.localPosition = new Vector3(pos.x, pos.y, 0.0f);
        pressedKey = null;
    }

    private string labelKey(bool shifted, PhysicsKey key)
    {
        switch (key.normal)
        {
            case "tab":
                return "Tab ->|";
            case "ret":
                return "Enter";
            case "shift":
                return "Shift";
            case "esc":
                return "Esc";
            case "caps":
                return "Caps lock";
            case "bsp":
                return "<- Backspace";
            case "del":
                return "Del";
            case "clr":
                return "AC";
            case "times":
                return "X";
            case "div":
                return "÷";
            case "plus":
                return "+";
            case "minus":
                return "-";
            case "ans":
                return "Ans";

            default:
                if (shifted)
                {
                    return key.shifted;
                }
                else
                {
                    return key.normal;
                }
        }
    }

    private PhysicsKey getKeyAtPosition(Vector3 position)
    {
        // get in keyboard-space
        var keyPos = keyContainer.transform.InverseTransformPoint(position);
        var pos2D = new Vector2(keyPos.x, keyPos.y);

        // Iterate over keys, finding the intersecting one
        float gap = keyGap / 200;
        foreach (var row in this.keyRows)
        {
            foreach (var key in row.keys)
            {
                if (pos2D.x > key.x - gap &&
                    pos2D.x < key.x + key.dim.x + gap &&
                    pos2D.y < key.y + key.dim.y/2 + gap &&
                    pos2D.y > key.y - key.dim.y/2 - gap)
                {
                    return key;
                }
            }
        }

        // Didn't hit any keys
        return null;
    }
}
