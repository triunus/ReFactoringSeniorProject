using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

/// <summary>
/// Local AccountStruct를 Get, Save, Delete하기 위해 사용하는 Interface이다.
/// </summary>
public interface IGetSetAccountData
{
    /// <summary>
    /// "LocalData/AccountData/AccountData.txt"위치의 AccountStruct 데이터를 가져옵니다.
    /// </summary>
    public AccountStruct GetAccountData();
    /// <summary>
    /// "LocalData/AccountData/AccountData.txt"위치에 AccountStruct 데이터를 저장합니다.
    /// 해당 경로가 존재하지 않을 시, 경로를 생성합니다.
    /// AccountStruct 객체를 직렬화하여 저장합니다.
    /// </summary>
    public void SaveAccountData(AccountStruct accountData);
    /// <summary>
    /// "LocalData/AccountData/AccountData.txt"위치의 회원 정보를 삭제합니다.
    /// </summary>
    public void DeleteAccountData();
}

// UnityEngine.Application.dataPath : 애플리케이션을 실행시키고 있는 장치의 게임 데이터 폴더 경로를 나타냅니다.

public class GetSetAccountData : IGetSetAccountData
{
    private IFormatter binaryFormatter = new BinaryFormatter();
    // 지정된 경로를 이용하여, DirectoryInfo 클래스 인스턴스를 초기화합니다.
    DirectoryInfo directory = new DirectoryInfo(UnityEngine.Application.dataPath + "/LocalData/AccountData");

    public AccountStruct GetAccountData()
    {
        try
        {
            // 지정된 경로가 없으면, null을 리턴.
            if (!directory.Exists) return null;

            Stream readInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/AccountData/AccountData.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            AccountStruct accountStruct = (AccountStruct)binaryFormatter.Deserialize(readInfo);

            readInfo.Close();
            return accountStruct;
        }
        catch (FileNotFoundException error01)
        {
            UnityEngine.Debug.Log("파일 없음 : " + error01);
            return null;
        }
    }

    public void SaveAccountData(AccountStruct accountStruct)
    {
        try
        {
            // 지정된 경로가 없으면, 지정된 경로 생성.
            if (!directory.Exists) directory.Create();

            Stream writeInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/AccountData/AccountData.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            binaryFormatter.Serialize(writeInfo, accountStruct);

            writeInfo.Close();
        }
        catch (UnityEngine.UnassignedReferenceException error01)
        {
            UnityEngine.Debug.Log("파일 있음 : " + error01);
        }
    }

    public void DeleteAccountData()
    {
        try
        {
            FileInfo willBeDeletedFile = new FileInfo(UnityEngine.Application.dataPath + "/LocalData/AccountData/AccountData.txt");

            willBeDeletedFile.Delete();
        }
        catch (IOException error01)
        {
            UnityEngine.Debug.Log("파일 없음 : " + error01);
        }
    }
}
