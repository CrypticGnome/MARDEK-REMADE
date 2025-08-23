using MARDEK.Event;
using UnityEngine;

public class CommandTrigger : MonoBehaviour
{
     [SerializeField] bool onStart = false;
     [SerializeField] bool onInteractionKey = false;
     [SerializeField] bool onTriggerEnter = false;
     [SerializeField] bool onTriggerExit = false;
     [SerializeField] string tagName = "Player";

     [SerializeField] Command command;

     void Start()
     {
          if (onStart)
               command.Trigger();
     }

     private void OnTriggerEnter2D(Collider2D collision)
     {
          if (!onTriggerEnter) return;

          if (!string.IsNullOrEmpty(tagName) && tagName != string.Empty)   // Don't know why I need to see if it's empty twice, but it doesn't work with null or empty
          {
               if (collision.gameObject.CompareTag(tagName))
               {
                    command.Trigger();
               }
               return;
          }
          command.Trigger();
     }
     private void OnTriggerExit2D(Collider2D collision)
     {
          if (!onTriggerExit) return;

          if (collision.gameObject.CompareTag("Player"))
               command.Trigger();
     }
     public void Interact()
     {
          if (onInteractionKey)
               command.Trigger();
     }

}
