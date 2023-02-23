using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Observer ���� Interface 2�� �� 5 ��.
// UserModel
public interface UserInformationObserver
{
    public void UpdateUserInformationObserver();
}
public interface IUserInformationObserver
{
    public void RegisterUserInformationObserver(UserInformationObserver observer);
    public UserData GetUserInformation();
}

// SelectedCharacterNumber
public interface SelectedCharacterNumberObserver
{
    public void UpdateSelectedCharacterNumberObserver();
}
public interface ISelectedCharacterNumberObserver
{
    public void RegisterSelectedCharacterNumberObserver(SelectedCharacterNumberObserver observer);
    public int GetSelectedCharacterNumber();
}

// OwnedCharacterNumber
public interface OwnedCharacterNumberObserver
{
    public void UpdateOwnedCharacterNumberObserver();
}
public interface IOwnedCharacterNumberObserver
{
    public void RegisterOwnedCharacterNumberObserver(OwnedCharacterNumberObserver observer);
    public List<int> GetOwnedCharacterNumber();
}

// SelectedSkillNumber
public interface SelectedSkillNumberObserver
{
    public void UpdateSelectedSkillNumberObserver();
}
public interface ISelectedSkillNumberObserver
{
    public void RegisterSelectedSkillNumberObserver(SelectedSkillNumberObserver observer);
    public List<SkillStruct> GetSelectedSkillNumber();
}

// OwnedSkillNumber
public interface OwnedSkillNumberObserver
{
    public void UpdateOwnedSkillNumberObserver();
}
public interface IOwnedSkillNumberObserver
{
    public void RegisterOwnedSkillNumberObserver(OwnedSkillNumberObserver observer);
    public List<SkillStruct> GetOwnedSkillNumber();
}

public interface IUserModel : IModel,
    IUserInformationObserver, ISelectedCharacterNumberObserver, ISelectedSkillNumberObserver,
    IOwnedCharacterNumberObserver, IOwnedSkillNumberObserver
{
    public void UpdateSelectedCharacterNumber(int selectedCharacterNumber);
    public void UpdateSelectedSkillNumber_Add(string skillUniqueNumber);
    public void UpdateSelectedSkillNumber_Pop(string skillUniqueNumber);
}


public class UserModel : IUserModel
{
    private IGameManagerForModel gameManager = null;

    private List<UserInformationObserver> userInformationObservers = new List<UserInformationObserver>();
    private List<SelectedCharacterNumberObserver> selectedCharacterNumberObservers = new List<SelectedCharacterNumberObserver>();
    private List<OwnedCharacterNumberObserver> ownedCharacterNumberObservers = new List<OwnedCharacterNumberObserver>();
    private List<SelectedSkillNumberObserver> selectedSkillNumberObservers = new List<SelectedSkillNumberObserver>();
    private List<OwnedSkillNumberObserver> ownedSkillNumberObservers = new List<OwnedSkillNumberObserver>();

    private UserData userData = new UserData();      // ���� ������ & ���� ��ȭ ����

    private int selectedCharacterNumber = 0;
    private List<int> ownedCharacterNumber = new List<int>();

    private List<SkillStruct> selectedSkillNumber = new List<SkillStruct>();
    private List<SkillStruct> ownedSkillNumber = new List<SkillStruct>();

    public UserModel()
    {
        gameManager = GameManager.GetGameManager();
        gameManager.RegisterModel(this);
    }


    // 1. userData�� Observer ���.
    // Observer ���.
    public void RegisterUserInformationObserver(UserInformationObserver observer)
    {
        userInformationObservers.Add(observer);
    }

    // UserInformation ����.
    public UserData GetUserInformation()
    {
        return userData;
    }

    // Observer���� �뺸.
    private void NotifyUserInformationObservers()
    {
        for (int i = 0; i < userInformationObservers.Count; i++)
        {
            userInformationObservers[i].UpdateUserInformationObserver();
        }
    }
    // ----------------------------------------------------


    // 2. SelectedCharacterNumber�� Observer ���.
    // Observer ���.
    public void RegisterSelectedCharacterNumberObserver(SelectedCharacterNumberObserver observer)
    {
        selectedCharacterNumberObservers.Add(observer);
    }

    // SelectedCharacterNumber ����
    public int GetSelectedCharacterNumber()
    {
        return selectedCharacterNumber;
    }

    // Observer���� �뺸.
    private void NotifySelectedCharacterNumberObservers()
    {
        for (int i = 0; i < selectedCharacterNumberObservers.Count; i++)
        {
            selectedCharacterNumberObservers[i].UpdateSelectedCharacterNumberObserver();
        }
    }
    // ----------------------------------------------------


    // 3. OwnedCharacterNumber�� Observer ���.
    // Observer ���.
    public void RegisterOwnedCharacterNumberObserver(OwnedCharacterNumberObserver observer)
    {
        ownedCharacterNumberObservers.Add(observer);
    }

    // OwnedCharacterNumber ����
    public List<int> GetOwnedCharacterNumber()
    {
        return ownedCharacterNumber;
    }

    // Observer���� �뺸.
    private void NotifyOwnedCharacterNumberObservers()
    {
        for (int i = 0; i < ownedCharacterNumberObservers.Count; i++)
        {
            ownedCharacterNumberObservers[i].UpdateOwnedCharacterNumberObserver();
        }
    }
    // ----------------------------------------------------


    // 4. SelectedSkillNumber�� Observer ���.
    // Observer ���.
    public void RegisterSelectedSkillNumberObserver(SelectedSkillNumberObserver observer)
    {
        selectedSkillNumberObservers.Add(observer);
    }

    // SelectedSkillNumber ����
    public List<SkillStruct> GetSelectedSkillNumber()
    {
        return selectedSkillNumber;
    }

    // Observer���� �뺸.
    private void NotifySelectedSkillNumberObservers()
    {
        for (int i = 0; i < selectedSkillNumberObservers.Count; i++)
        {
            selectedSkillNumberObservers[i].UpdateSelectedSkillNumberObserver();
        }
    }
    // ----------------------------------------------------


    // 5. OwnedSkillNumber�� Observer ���.
    // Observer ���.
    public void RegisterOwnedSkillNumberObserver(OwnedSkillNumberObserver observer)
    {
        ownedSkillNumberObservers.Add(observer);
    }

    // OwnedSkillNumber ����
    public List<SkillStruct> GetOwnedSkillNumber()
    {
        return ownedSkillNumber;
    }

    // Observer���� �뺸.
    private void NotifyOwnedSkillNumberObservers()
    {
        for (int i = 0; i < ownedSkillNumberObservers.Count; i++)
        {
            ownedSkillNumberObservers[i].UpdateOwnedSkillNumberObserver();
        }
    }
    // ----------------------------------------------------

    // 8. GameManager���� Model�� �����͸� Get, Set�ϴ� �κ�.
    // userModel ��ü �ȿ�, userData, selectedCharacterNumber, selectedSkillNumber ��ü�� ���Եȴ�.
    // Server�� ��û�� ���信 3������ ������ ������ ��ü�� �޾� ����Ѵ�.
    public string GetModelData()
    {
        JObject parentJObject = new JObject();
        JObject chiedJObject = new JObject();

        parentJObject["userModel"] = chiedJObject;

        chiedJObject["userData"] = (JObject)JToken.FromObject(this.userData);
        chiedJObject["selectedCharacterNumber"] = this.selectedCharacterNumber;
        chiedJObject["selectedSkillNumber"] = SkillStructListToJArray(this.selectedSkillNumber);

        //        Debug.Log("parentJObject : " + parentJObject);
        //        Debug.Log("chiedJObject : " + chiedJObject);

        return JsonConvert.SerializeObject(parentJObject, Formatting.Indented);
    }

    // Jstring�� ���� �ް� ���⼭, userData�� �������� ����.
    public void SetModelData(string JString)
    {
        JObject responseData = JObject.Parse(JString);
        //        Debug.Log("responseData : " + responseData);

        this.userData = JsonConvert.DeserializeObject<UserData>(JsonConvert.SerializeObject(responseData["userModel"]["userData"], Formatting.Indented));
        JArrayToIntList((JArray)responseData["userModel"]["ownedCharacterNumber"]);
        JArrayToSkillStructList((JArray)responseData["userModel"]["ownedSkillNumber"]);

        NotifyUserInformationObservers();
        NotifyOwnedCharacterNumberObservers();
        NotifySelectedCharacterNumberObservers();
        NotifyOwnedSkillNumberObservers();
        NotifySelectedSkillNumberObservers();

    }

    // ���� 3����, LIst�� ��ü�� �Ǵ� ��ü�� List�� �ٲ��ִ� �Լ��̴�.
    // Json�� LIst�� ���� ������ �ʵ���� �ѹ��� JObject�� ���������� �ʱ⿡ ���� �޼ҵ带 �����Ѵ�.
    private JArray SkillStructListToJArray(List<SkillStruct> listData)
    {
        JArray temp = new JArray();

        for (int i = 0; i < listData.Count; i++)
        {
            JObject tmp = new JObject();
            tmp["skillUniqueNumber"] = listData[i].SkillUniqueNumber;
            tmp["NFTNumber"] = listData[i].NFTNumber;
            tmp["remainCount"] = listData[i].RemainCount;
            tmp["skillNumber"] = listData[i].SkillNumber;
            temp.Add(tmp);
        }

        return temp;
    }

    private void JArrayToSkillStructList(JArray jArrayData)
    {
        this.ownedSkillNumber.Clear();

        for (int i = 0; i < jArrayData.Count; i++)
        {
            ownedSkillNumber.Add(JsonConvert.DeserializeObject<SkillStruct>(JsonConvert.SerializeObject(jArrayData[i])));
        }
    }

    private void JArrayToIntList(JArray jArrayData)
    {
        this.ownedCharacterNumber.Clear();

        for (int i = 0; i < jArrayData.Count; i++)
        {
            ownedCharacterNumber.Add((int)jArrayData[i]);
        }
    }
    // ----------------------------------------



     // 9.Controller���� �����ϴ� �κ��̴�.
     // CharacterMenu�� �����ϴ� ĳ���� ������ �����ϴ� ���� ȣ��ȴ�.
    public void UpdateSelectedCharacterNumber(int selectedCharacterNumber)
    {
        this.selectedCharacterNumber = selectedCharacterNumber;

        NotifySelectedCharacterNumberObservers();
    }

    public void UpdateSelectedSkillNumber_Add(string skillUniqueNumber)
    {
        // �ߺ� ���� ����.
        for (int i = 0; i < selectedSkillNumber.Count; i++)
        {
            if (skillUniqueNumber.Equals(selectedSkillNumber[i].SkillUniqueNumber))
            {
                return;
            }
        }

        // 6�� �̻� ���� ����.
        if (selectedSkillNumber.Count >= 6) return;

        for (int i =0; i< ownedSkillNumber.Count; i++)
        {
            if (skillUniqueNumber.Equals(ownedSkillNumber[i].SkillUniqueNumber))
            {
                selectedSkillNumber.Add(ownedSkillNumber[i]);
                break;
            }
        }

        NotifySelectedSkillNumberObservers();
    }

    public void UpdateSelectedSkillNumber_Pop(string skillUniqueNumber)
    {
        for (int i = 0; i < selectedSkillNumber.Count; i++)
        {
            if (skillUniqueNumber.Equals(selectedSkillNumber[i].SkillUniqueNumber))
            {
                selectedSkillNumber.RemoveAt(i);
                break;
            }
        }

        NotifySelectedSkillNumberObservers();
    }
}
