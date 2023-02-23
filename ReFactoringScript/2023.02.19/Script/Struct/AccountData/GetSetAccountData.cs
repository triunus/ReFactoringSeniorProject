// ���� ����ȭ�� �ʿ�.
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

public interface IGetSetAccountData
{
    public AccountData GetAccountData();
    public void SaveAccountData(AccountData accountData);
    public void DeleteAccountData();
}

public class GetSetAccountData : IGetSetAccountData
{
    private IFormatter binaryFormatter;
    private AccountData accountData;
    DirectoryInfo directory = new DirectoryInfo(Application.dataPath + "/LocalData/AccountData");

    public AccountData GetAccountData()
    {
        binaryFormatter = new BinaryFormatter();

        try
        {
            if (!directory.Exists)
            {
                return null;
            }
            Stream readInfo = new FileStream(Application.dataPath + "/LocalData/AccountData/AccountData.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            accountData = (AccountData)binaryFormatter.Deserialize(readInfo);
            readInfo.Close();

            return accountData;
        }
        catch (FileNotFoundException error01)
        {
            // ���� ���� �ڵ� 21
            Debug.Log("���� ���� : " + error01);
            return null;
        }
    }

    public void SaveAccountData(AccountData accountData)
    {
        binaryFormatter = new BinaryFormatter();

        try
        {
            //            Debug.Log("SetAccountData ����");
            if (!directory.Exists)
            {
                directory.Create();
            }

            Stream writeInfo = new FileStream(Application.dataPath + "/LocalData/AccountData/AccountData.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            this.accountData = accountData;

            // ���� ���Ϸ� ����ȭ ����
            binaryFormatter.Serialize(writeInfo, this.accountData);

            writeInfo.Close();
            Debug.Log("Write Complete");
        }
        catch (UnassignedReferenceException error01)
        {
            Debug.Log(error01);
        }
    }

    public void DeleteAccountData()
    {
        Debug.Log("DeleteAccountData ����");

        FileInfo willBeDeletedFile = new FileInfo(Application.dataPath + "/LocalData/AccountData/AccountData.txt");

        try
        {
            willBeDeletedFile.Delete();
        }
        catch (IOException error01)
        {
            Debug.Log(error01);
        }

        Debug.Log("DeleteAccount Complete");
    }
}
