using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Observer 관련 Interface 2개 씩 5 쌍.
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

    private UserData userData = new UserData();      // 유저 프로필 & 유저 재화 정보

    private int selectedCharacterNumber = 0;
    private List<int> ownedCharacterNumber = new List<int>();

    private List<SkillStruct> selectedSkillNumber = new List<SkillStruct>();
    private List<SkillStruct> ownedSkillNumber = new List<SkillStruct>();

    public UserModel()
    {
        gameManager = GameManager.GetGameManager();
        gameManager.RegisterModel(this);
    }


    // 1. userData의 Observer 기능.
    // Observer 등록.
    public void RegisterUserInformationObserver(UserInformationObserver observer)
    {
        userInformationObservers.Add(observer);
    }

    // UserInformation 리턴.
    public UserData GetUserInformation()
    {
        return userData;
    }

    // Observer에게 통보.
    private void NotifyUserInformationObservers()
    {
        for (int i = 0; i < userInformationObservers.Count; i++)
        {
            userInformationObservers[i].UpdateUserInformationObserver();
        }
    }
    // ----------------------------------------------------


    // 2. SelectedCharacterNumber의 Observer 기능.
    // Observer 등록.
    public void RegisterSelectedCharacterNumberObserver(SelectedCharacterNumberObserver observer)
    {
        selectedCharacterNumberObservers.Add(observer);
    }

    // SelectedCharacterNumber 리턴
    public int GetSelectedCharacterNumber()
    {
        return selectedCharacterNumber;
    }

    // Observer에게 통보.
    private void NotifySelectedCharacterNumberObservers()
    {
        for (int i = 0; i < selectedCharacterNumberObservers.Count; i++)
        {
            selectedCharacterNumberObservers[i].UpdateSelectedCharacterNumberObserver();
        }
    }
    // ----------------------------------------------------


    // 3. OwnedCharacterNumber의 Observer 기능.
    // Observer 등록.
    public void RegisterOwnedCharacterNumberObserver(OwnedCharacterNumberObserver observer)
    {
        ownedCharacterNumberObservers.Add(observer);
    }

    // OwnedCharacterNumber 리턴
    public List<int> GetOwnedCharacterNumber()
    {
        return ownedCharacterNumber;
    }

    // Observer에게 통보.
    private void NotifyOwnedCharacterNumberObservers()
    {
        for (int i = 0; i < ownedCharacterNumberObservers.Count; i++)
        {
            ownedCharacterNumberObservers[i].UpdateOwnedCharacterNumberObserver();
        }
    }
    // ----------------------------------------------------


    // 4. SelectedSkillNumber의 Observer 기능.
    // Observer 등록.
    public void RegisterSelectedSkillNumberObserver(SelectedSkillNumberObserver observer)
    {
        selectedSkillNumberObservers.Add(observer);
    }

    // SelectedSkillNumber 리턴
    public List<SkillStruct> GetSelectedSkillNumber()
    {
        return selectedSkillNumber;
    }

    // Observer에게 통보.
    private void NotifySelectedSkillNumberObservers()
    {
        for (int i = 0; i < selectedSkillNumberObservers.Count; i++)
        {
            selectedSkillNumberObservers[i].UpdateSelectedSkillNumberObserver();
        }
    }
    // ----------------------------------------------------


    // 5. OwnedSkillNumber의 Observer 기능.
    // Observer 등록.
    public void RegisterOwnedSkillNumberObserver(OwnedSkillNumberObserver observer)
    {
        ownedSkillNumberObservers.Add(observer);
    }

    // OwnedSkillNumber 리턴
    public List<SkillStruct> GetOwnedSkillNumber()
    {
        return ownedSkillNumber;
    }

    // Observer에게 통보.
    private void NotifyOwnedSkillNumberObservers()
    {
        for (int i = 0; i < ownedSkillNumberObservers.Count; i++)
        {
            ownedSkillNumberObservers[i].UpdateOwnedSkillNumberObserver();
        }
    }
    // ----------------------------------------------------

    // 8. GameManager에서 Model의 데이터를 Get, Set하는 부분.
    // userModel 객체 안에, userData, selectedCharacterNumber, selectedSkillNumber 객체가 포함된다.
    // Server의 요청과 응답에 3가지의 정보를 각가의 객체로 받아 사용한다.
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

    // Jstring을 전달 받고 여기서, userData만 빼가도록 하자.
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

    // 밑의 3개는, LIst를 객체로 또는 객체를 List로 바꿔주는 함수이다.
    // Json은 LIst와 같은 동적인 필드들을 한번에 JObject로 변경해주지 않기에 따로 메소드를 정의한다.
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



     // 9.Controller에서 접근하는 부분이다.
     // CharacterMenu에 존재하는 캐릭터 슬롯을 선택하는 순간 호출된다.
    public void UpdateSelectedCharacterNumber(int selectedCharacterNumber)
    {
        this.selectedCharacterNumber = selectedCharacterNumber;

        NotifySelectedCharacterNumberObservers();
    }

    public void UpdateSelectedSkillNumber_Add(string skillUniqueNumber)
    {
        // 중복 선택 방지.
        for (int i = 0; i < selectedSkillNumber.Count; i++)
        {
            if (skillUniqueNumber.Equals(selectedSkillNumber[i].SkillUniqueNumber))
            {
                return;
            }
        }

        // 6개 이상 선택 방지.
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
