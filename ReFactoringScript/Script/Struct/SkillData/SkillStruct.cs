public class SkillStruct
{
    private string skillUniqueNumber;
    private string _NFTNumber;
    private int remainCount;
    private int skillNumber;

    public SkillStruct(string skillUniqueNumber, string _NFTNumber, int remainCount, int skillNumber)
    {
        this.skillUniqueNumber = skillUniqueNumber;
        this._NFTNumber = _NFTNumber;
        this.remainCount = remainCount;
        this.skillNumber = skillNumber;
    }

    public string SkillUniqueNumber
    {
        get{ return skillUniqueNumber; }
        set { skillUniqueNumber = value; }
    }

    public string NFTNumber
    {
        get { return _NFTNumber; }
        set { _NFTNumber = value; }
    }

    public int RemainCount
    {
        get { return remainCount; }
        set { remainCount = value; }
    }

    public int SkillNumber
    {
        get { return skillNumber; }
        set { skillNumber = value; }
    }
}
