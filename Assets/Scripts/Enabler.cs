using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enabler : MonoBehaviour
{
   public enum State : byte
   {
      None = 0,
      Pause = 1,
      Play = 2
   }

   [SerializeField] private Button[] stateButtons;

   public byte Val
   {
      set
      {
         for (int i = 0; i < stateButtons.Length; i++)
         {
            bool vis = (i == value);
            Button b = stateButtons[i];
            if (b != null && b.interactable != vis)
            {
               b.interactable = vis;
               foreach (Image im in b.GetComponentsInChildren<Image>())
               {
                  im.color = vis ? Color.white : new Color(0.7f,0.7f,0.7f);
               }
            }
         }
      }
   }
}
