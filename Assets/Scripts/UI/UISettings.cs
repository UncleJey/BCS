using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : UIHider
{
   [SerializeField] private Text text;

   [SerializeField] private Button back;
   private void OnEnable()
   {
      text.text = "";
   }

   private void Awake()
   {
      back.onClick.AddListener(UIMenu.MainScreen);
   }
}

