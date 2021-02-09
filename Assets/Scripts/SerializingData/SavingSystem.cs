using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SavingSystem
{
    public static string directory = "/Data.unk";

    public static void SaveSettings(SettingsMenu settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + directory;
        FileStream stream = new FileStream(path, FileMode.Create);

        SettingsData data = new SettingsData(settings);

        formatter.Serialize(stream, data);
        stream.Close();

        //Debug.Log("Your file is load from " + path);
    }

    public static SettingsData LoadSettings()
    {
        string path = Application.persistentDataPath + directory;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SettingsData data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();

            //Debug.Log("Your file is saved in " + path);
            return data;
        } 
        else
        {
            //Debug.LogError("Save file not found in" + path);
            return null;
        }
    }
}
