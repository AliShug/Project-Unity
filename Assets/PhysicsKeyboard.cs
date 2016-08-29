using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

public class PhysicsKeyboard : PhysicsInput
{
    [System.Serializable]
    public class PhysicsKey
    {
        public string normal;
        public string shifted;
        public float width;
    }

    [System.Serializable]
    public class KeyRow
    {
        public List<PhysicsKey> keys = new List<PhysicsKey>();
    }

    private List<KeyRow> keyRows = new List<KeyRow>();
    private GameObject keyContainer;

    [Tooltip("YAML text asset specifying key mapping and layout")]
    public TextAsset layout;
    public GameObject keyPrefab;

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

                x += size*key.width + gap;
            }
            if (x > xLim)
                xLim = x;
            yLim = y;
            y -= size + gap;
        }

        // Place key container to center keyboard
        keyContainer.transform.localPosition = new Vector3(xLim/2, -yLim/2);
    }

    protected override void OnHovering(Vector3 hoverPoint)
    {
        base.OnHovering(hoverPoint);
        // Find closest key to hover point
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

        // TODO
    }
}
