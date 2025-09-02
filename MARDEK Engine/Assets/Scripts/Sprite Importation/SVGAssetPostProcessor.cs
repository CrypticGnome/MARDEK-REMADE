using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;


class SVGStrokeFixer : AssetPostprocessor
{
     const float MIN_LINE_THICKNESS = 0.5f;
     void OnPreprocessAsset()
     {
          if (!assetPath.EndsWith(".svg")) return;
          string original = File.ReadAllText(assetPath);

          bool assetPathContainsCustomThickness = assetPath.Contains("Thickness");
          float thickness = MIN_LINE_THICKNESS;
          if (assetPathContainsCustomThickness)
          {
               Match match = Regex.Match(assetPath, @"Thickness(\d+(\.\d+)?)");
               if (match.Success)
               {
                    thickness = float.Parse(match.Groups[1].Value) / 10;
                    Debug.Log($"<color=green>{assetPath} asset generated with a min line thickness of {thickness}</color>");
               }
          }
          var doc = XDocument.Load(assetPath);
          foreach (var el in doc.Descendants())
          {
               var strokeWidth = el.Attribute("stroke-width");
               if (strokeWidth is null || !float.TryParse(strokeWidth.Value, out float w)) continue;
          
               if (w >= thickness) continue;
          
               el.SetAttributeValue("stroke-width", $"{thickness}");
               
          }

          string xml = doc.ToString();
          xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n" + xml + "\n";


          //// Fix any self-closing spacing if Unity cares
          xml = xml.Replace(" />", "/>");
          //xml = xml.Replace("><", ">\n  <");
          if (!string.Equals(original, xml, StringComparison.Ordinal))
          {
               File.WriteAllText(assetPath, xml);
          }
     }
}
