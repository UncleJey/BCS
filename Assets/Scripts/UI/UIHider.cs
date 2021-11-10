using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIHider : MonoBehaviour
{
   private float exTime;
   private Animation _animation => GetComponent<Animation>();
   public bool Visible
   {
      get => gameObject.activeSelf;
      set
      {
         if (Time.time - exTime < 0.5f)
            return;

         exTime = Time.time;
         if (value != Visible)
         {
            if (value)
            {
               gameObject.SetActive(true);
            }
            else if (!value)
            {
               Off();
            }
            _animation.Play(value ? "show" : "hide");
         }
      }
   }

   async void Off()
   {
      await Task.Delay(300);
      gameObject.SetActive(false);
   }
   
}
