using System;

// 직렬화를 하기 위해서는 해당 클래스가 직렬화가 가능하다는 것을 명시해 주어야 한다.
[Serializable]      // SerializableAttribute 클래스 또는 ISerializable 인터페이스 참고
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
