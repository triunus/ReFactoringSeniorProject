using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

/// <summary>
/// Local AccountStruct�� Get, Save, Delete�ϱ� ���� ����ϴ� Interface�̴�.
/// </summary>
public interface IGetSetAccountData
{
    /// <summary>
    /// "LocalData/AccountData/AccountData.txt"��ġ�� AccountStruct �����͸� �����ɴϴ�.
    /// </summary>
    public AccountStruct GetAccountData();
    /// <summary>
    /// "LocalData/AccountData/AccountData.txt"��ġ�� AccountStruct �����͸� �����մϴ�.
    /// �ش� ��ΰ� �������� ���� ��, ��θ� �����մϴ�.
    /// AccountStruct ��ü�� ����ȭ�Ͽ� �����մϴ�.
    /// </summary>
    public void SaveAccountData(AccountStruct accountData);
    /// <summary>
    /// "LocalData/AccountData/AccountData.txt"��ġ�� ȸ�� ������ �����մϴ�.
    /// </summary>
    public void DeleteAccountData();
}

// UnityEngine.Application.dataPath : ���ø����̼��� �����Ű�� �ִ� ��ġ�� ���� ������ ���� ��θ� ��Ÿ���ϴ�.

public class GetSetAccountData : IGetSetAccountData
{
    private IFormatter binaryFormatter = new BinaryFormatter();
    // ������ ��θ� �̿��Ͽ�, DirectoryInfo Ŭ���� �ν��Ͻ��� �ʱ�ȭ�մϴ�.
    DirectoryInfo directory = new DirectoryInfo(UnityEngine.Application.dataPath + "/LocalData/AccountData");

    public AccountStruct GetAccountData()
    {
        try
        {
            // ������ ��ΰ� ������, null�� ����.
            if (!directory.Exists) return null;

            Stream readInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/AccountData/AccountData.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            AccountStruct accountStruct = (AccountStruct)binaryFormatter.Deserialize(readInfo);

            readInfo.Close();
            return accountStruct;
        }
        catch (FileNotFoundException error01)
        {
            UnityEngine.Debug.Log("���� ���� : " + error01);
            return null;
        }
    }

    public void SaveAccountData(AccountStruct accountStruct)
    {
        try
        {
            // ������ ��ΰ� ������, ������ ��� ����.
            if (!directory.Exists) directory.Create();

            Stream writeInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/AccountData/AccountData.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            binaryFormatter.Serialize(writeInfo, accountStruct);

            writeInfo.Close();
        }
        catch (UnityEngine.UnassignedReferenceException error01)
        {
            UnityEngine.Debug.Log("���� ���� : " + error01);
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
            UnityEngine.Debug.Log("���� ���� : " + error01);
        }
    }
}
