using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
   public enum State : byte
   {
      None = 0,
      Search = 1,
      Pause = 2,
      Good = 3,
      Bad = 4
   }

   [SerializeField] private Image[] stateIcon;

   public byte Val
   {
      set
      {
         for (int i = 0; i < stateIcon.Length; i++)
         {
            bool vis = (i == value);
            if (stateIcon[i] != null && stateIcon[i].gameObject.activeSelf != vis)
            {
               stateIcon[i].gameObject.SetActive(vis);
            }
         }
      }
   }
}
