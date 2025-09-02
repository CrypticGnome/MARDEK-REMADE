using UnityEngine;

namespace MARDEK.Stats
{
    [CreateAssetMenu(menuName = "MARDEK/Stats/Element")]
    public class Element : ScriptableObject
    {
          [SerializeField] Sprite _thinSprite;
          [SerializeField] Sprite _thickSprite;
          [SerializeField] Color _textColor;
          [SerializeField] ElementID element;
          public Sprite thinSprite { get { return _thinSprite; } }

          public Sprite thickSprite { get { return _thickSprite; } }

          public Color textColor { get { return _textColor; } }
          public ElementID ElementID { get { return element; } }
     }
     public enum ElementID
     {
          Physical = 0,
          Fire,
          Water,
          Air,
          Earth,
          Light,
          Dark,
          Aether,
          Fig,
          Thauma,
          Divine
     }
}