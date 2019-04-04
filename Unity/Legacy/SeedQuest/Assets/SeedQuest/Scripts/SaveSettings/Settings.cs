﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settings
{
    //public Settings settingsHere;

    public float MasterVolume;
    public float MusicVolume;
    public float SoundEffectVolume;
    public float CameraSensitivity;
    public bool IsVolumeMuted;

    public string saveFileVersion;

    public Dictionary<string, string> userDids;
    public Dictionary<string, string> userSeeds;
}