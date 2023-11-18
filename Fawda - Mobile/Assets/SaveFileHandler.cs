using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;


public class SaveFileHandler
{
    private string filePath;
    private string fileName;
    
    public SaveFileHandler(string _filePath, string _fileName){
        this.fileName = _fileName;
        this.filePath = _filePath;
    }

    public ProfileData Load(){
        string inPath = Path.Combine(filePath,fileName);
        if (!File.Exists(inPath)) return null;
        string rawData;
        using (FileStream fileStream = new FileStream(inPath, FileMode.Open)){
            using (StreamReader streamReader = new StreamReader(fileStream)){
                rawData = streamReader.ReadToEnd();
            }
        }

        return JsonUtility.FromJson<ProfileData>(rawData);
    }

    public void Save(ProfileData _profile){
        if(!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
        string outPath = Path.Combine(filePath,fileName);
        string data = JsonUtility.ToJson(_profile,true);
        using (FileStream fileStream = new FileStream(outPath, FileMode.Create)){
            using (StreamWriter streamWriter = new StreamWriter(fileStream)){
                streamWriter.Write(data);
            }
        }
    }
}
