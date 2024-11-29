using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
   public event Action StartCompleted; 
   
   public void Enable()
   {
      gameObject.SetActive(true);
      StartCoroutine(StartDisabling());
   }

   public void Disable()
   {
      gameObject.SetActive(false);
   }

   private IEnumerator StartDisabling()
   {
      int i = 2;
      
      while (i > 0)
      {
         i--;
         yield return new WaitForSeconds(1f);
      }
      
      StartCompleted?.Invoke();
      Disable();
   }
}
