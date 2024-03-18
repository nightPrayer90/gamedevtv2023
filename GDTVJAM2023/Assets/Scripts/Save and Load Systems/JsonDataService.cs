using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class JsonDataService : IDataService
{
    private const string KEY = "ggdPhkeOoiv6YMiPWa34kIuOdDUL7NwQFg611DVdwN8=";
    private const string IV = "JZuM0HQsWSBVpRHTeRZMYQ==";

    public bool SaveData<T>(string RelativePath, T Data, bool Encrypted)
    {
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + RelativePath;
        try
        {
            if (File.Exists(path) == true)
            {
                Debug.Log("Data exists.Deleting old file and writing a new one!");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Creating file for the first time!");
            } 
            using FileStream stream = File.Create(path);
            if (Encrypted == true)
            {
                WriteEncryptedData(Data, stream);
            }
            else
            {
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            }
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
        
    }

    private void WriteEncryptedData<T>(T Data, FileStream Stream)
    {
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new CryptoStream(
            Stream,
            cryptoTransform,
            CryptoStreamMode.Write
        );
        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(Data)));
    }



    public T LoadData<T>(string RelativePath, bool Encrypted)
    {
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + RelativePath;

        if (File.Exists(path) == false)
        {
            Debug.Log($"Cannot load file at {path}. File does not exist!");
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            T data;
            if (Encrypted == true)
            {
                data = ReadEncryptedData<T>(path);
            }
            else
            {
                data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            
            return data;
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }

    }

    private T ReadEncryptedData<T>(string Path)
    {
        byte[] fileBytes = File.ReadAllBytes(Path);
        using Aes aesProvider = Aes.Create();

        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(
            aesProvider.Key,
            aesProvider.IV
        );

        using MemoryStream decryptionStream = new MemoryStream(fileBytes);
        using CryptoStream cryptoStream = new CryptoStream(
            decryptionStream,
            cryptoTransform,
            CryptoStreamMode.Read
        );
        using StreamReader reader = new StreamReader(cryptoStream);
        string result = reader.ReadToEnd();

        Debug.Log($"Decrypted result (if the following is not legible, probably wrong key or iv): {result}");
        return JsonConvert.DeserializeObject<T>(result);
    }
}
