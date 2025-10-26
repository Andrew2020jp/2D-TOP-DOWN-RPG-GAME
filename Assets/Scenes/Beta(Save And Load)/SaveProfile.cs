using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveProfile<T> where T : SaveProfileData
{
    public string name;
    public T saveData;

    private SaveProfile() { }

    public SaveProfile(string name, T saveData)
    {
        this.name=name;
        this.saveData=saveData;
    }

}

public abstract record SaveProfileData { }

public recordÅ@PlayerSaveData:SaveProfileData
{
    public Vector2 position;
    public int[] achievements;
}
