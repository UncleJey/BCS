using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuButtons : UIHider
{
  [SerializeField] private Button settings;

  private void Awake()
  {
    settings.onClick.AddListener(UIMenu.SettingsScreen);
  }

  private void OnDestroy()
  {
    settings.onClick.RemoveAllListeners();
  }
}
