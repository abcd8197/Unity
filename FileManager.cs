using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using PathFinder;   // 만든 dll, 현재 디바이스의 Document경로를 가져온다.


// 링크 참조 : https://glikmakesworld.tistory.com/14

public class FileManager<T> : MonoBehaviour
{
    private static readonly string privateKey = "2f8e1c6d2f1a6s5d4f1d2f3d5c4s8d5f";
    
    public void SaveData(T data, string path)
    {
        string jsonStr = DataToJson(data);
        string encryptStr = Encrypt(jsonStr);
        SaveFile(path, encryptStr);
    }

    public T LoadData(string path)
    {
        //파일이 존재하는지부터 체크.
        if (!File.Exists(GetPath(path))) return default(T);

        string encryptData = LoadFile(GetPath(path));
        string decryptData = Decrypt(encryptData);

        T data = JsonToData(decryptData);
        return data;
    }

    //===========================================================
    // Data -> Json으로 변환
    private static string DataToJson(T data)
    {
        string str = JsonUtility.ToJson(data);
        return str;
    }
    // Json -> Data로 변환
    private static T JsonToData(string json)
    {
        T data = JsonUtility.FromJson<T>(json);
        return data;
    }
    //=============================================================

    //json string을 파일로 저장
    static void SaveFile(string path, string jsonData)
    {
        using (FileStream fs = new FileStream(GetPath(path), FileMode.Create, FileAccess.Write))
        {
            //파일로 저장할 수 있게 바이트화
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            //bytes의 내용물을 0 ~ max 길이까지 fs에 복사
            fs.Write(bytes, 0, bytes.Length);
        }
    }

    //파일 불러오기
    static string LoadFile(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            //파일을 바이트화 했을 때 담을 변수를 제작
            byte[] bytes = new byte[(int)fs.Length];

            //파일스트림으로 부터 바이트 추출
            fs.Read(bytes, 0, (int)fs.Length);

            //추출한 바이트를 json string으로 인코딩
            string jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            return jsonString;
        }
    }

    // =========================================================

    private static string Encrypt(string strData)
    {
        // 직렬화 된 데이터의 바이트를 받는다.
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strData);

        RijndaelManaged rm = CreateRijndaelManaged();

        ICryptoTransform ct = rm.CreateEncryptor();

        byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);

        return System.Convert.ToBase64String(results, 0, results.Length);
    }

    private static string Decrypt(string strData)
    {
        byte[] bytes = System.Convert.FromBase64String(strData);

        RijndaelManaged rm = CreateRijndaelManaged();

        ICryptoTransform ct = rm.CreateDecryptor();

        byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }

    private static RijndaelManaged CreateRijndaelManaged()
    {
        byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(privateKey);
        RijndaelManaged result = new RijndaelManaged();

        byte[] newKeysArray = new byte[16];
        System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);

        result.Key = newKeysArray;
        result.Mode = CipherMode.ECB;
        result.Padding = PaddingMode.PKCS7;
        return result;
    }

    public void IDisposable() { }

    // =================================================================

    private static string GetPath(string fileName) => PathFinder.Path_Finder.PathForDocumentsFile(fileName);
}
