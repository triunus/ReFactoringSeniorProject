public class UserData
{
    private string userNickname;
    private int userImageNumber;
    private int coin;
    private int paidCoin;

    public UserData()
    {
        this.userNickname = null;
        this.userImageNumber = 0;
        this.coin = 0;
        this.paidCoin = 0;
    }

    public UserData(string userNickname, int userImageNumber, int coin, int paidCoin)
    {
        this.userNickname = userNickname;
        this.userImageNumber = userImageNumber;
        this.coin = coin;
        this.paidCoin = paidCoin;
    }

    public string UserNickname
    {
        get{ return userNickname; }
        set { userNickname = value; }
    }
    public int UserImageNumber
    {
        get { return userImageNumber; }
        set { userImageNumber = value; }
    }
    public int Coin
    {
        get { return coin; }
        set { coin = value; }
    }
    public int PaidCoin
    {
        get { return paidCoin; }
        set { paidCoin = value; }
    }
}
