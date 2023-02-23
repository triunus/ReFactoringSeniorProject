using System;

// ����ȭ�� �ϱ� ���ؼ��� �ش� Ŭ������ ����ȭ�� �����ϴٴ� ���� ����� �־�� �Ѵ�.
[Serializable]      // SerializableAttribute Ŭ���� �Ǵ� ISerializable �������̽� ����
public class AccountData
{
    private string accountID;
    private string accountPW;
    private string phoneNumber;
    private string wallletAddressNumber;

    public AccountData(string accountID, string accountPW, string phoneNumber, string wallletAddressNumber)
    {
        this.accountID = accountID;
        this.accountPW = accountPW;
        this.phoneNumber = phoneNumber;
        this.wallletAddressNumber = wallletAddressNumber;
    }

    public string AccountID
    {
        get{ return accountID; }
        set { accountID = value; }
    }
    public string AccountPW
    {
        get { return accountPW; }
        set { accountPW = value; }
    }
    public string PhoneNumber
    {
        get { return phoneNumber; }
        set { phoneNumber = value; }
    }
    public string WallletAddressNumber
    {
        get { return wallletAddressNumber; }
        set { wallletAddressNumber = value; }
    }
}
