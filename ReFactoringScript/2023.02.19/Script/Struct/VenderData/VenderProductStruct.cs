public class VenderProductStruct
{
    private int productNumber;
    private int skillNumber;
    private string skillRank;
    private int price;
    private bool isBye;

    public VenderProductStruct(int productNumber, int skillNumber, string skillRank, int price, bool isBye)
    {
        this.productNumber = productNumber;
        this.skillNumber = skillNumber;
        this.skillRank = skillRank;
        this.price = price;
        this.isBye = isBye;
    }

    public int ProductNumber
    {
        get { return productNumber; }
        set { productNumber = value; }
    }
    public int SkillNumber
    {
        get { return skillNumber; }
        set { skillNumber = value; }
    }
    public string SkillRank
    {
        get { return skillRank; }
        set { skillRank = value; }
    }
    public int Price
    {
        get { return price; }
        set { price = value; }
    }
    public bool IsBye
    {
        get { return isBye; }
        set { isBye = value; }
    }
}