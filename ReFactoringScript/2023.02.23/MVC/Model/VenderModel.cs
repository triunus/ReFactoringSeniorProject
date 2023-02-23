using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public interface VenderProductStructObserver
{
    public void UpdateVenderProductStructObserver();
}
public interface IVenderProductStructObserver
{
    public void RegisterVenderProductStructObserver(VenderProductStructObserver observer);
    public List<VenderProductStruct> GetVenderProductStruct();
}

public interface SelectedVenderProductNumberObserver
{
    public void UpdateSelectedVenderProductNumberObserver();
}

public interface ISelectedVenderProductNumberObserver
{
    public void RegisterSelectedVenderProductNumberObserver(SelectedVenderProductNumberObserver observer);
    public int GetSelectedVenderProductNumber();
}


public interface IVenderModel : IModel, INPCMessageViewClassifier, IVenderProductStructObserver, ISelectedVenderProductNumberObserver
{
    public void UpdateButtonOnClicked();
    public void UpdateYesButtonOnClicked();
    public void BuyButtonOnClicked();
    public void OnClickedVenderProductSlot(int prodcutName);

    /// <summary>
    /// MessageClassifier 객체에 접근하기 위한 메소드입니다.
    /// </summary>
    public IMessageClassifier GetMessageClassifier();
}

public class VenderModel : IVenderModel
{
    private IGameManagerForModel gameManger = null;
    private IMessageClassifier messageClassifier = null;
    private INPCMessageViewClassifier NPCMessageMediator = null;

    private List<VenderProductStructObserver> venderProductStructObservers = new List<VenderProductStructObserver>();
    private List<SelectedVenderProductNumberObserver> selectedVenderProductNumberObservers = new List<SelectedVenderProductNumberObserver>();

    private List<VenderProductStruct> venderProductStructs = new List<VenderProductStruct>();
    private int selectedVenderProductNumber;
    
    public VenderModel()
    {
        gameManger = GameManager.GetGameManager();
        messageClassifier = new MessageClassifier();
        NPCMessageMediator = new NPCMessageViewClassifier();

        gameManger.RegisterModel(this);
    }

    // 1.서버와의 소통을 하는데 사용되는 부분. GameManager가 모델의 데이터를 가져가고, 설정하는데 사용한다.
    public string GetModelData()
    {
        JObject parentJObject = new JObject();
        JObject childJObject = new JObject();

        parentJObject["venderModel"] = childJObject;

        childJObject["selectedVenderProductNumber"] = this.selectedVenderProductNumber;

        return JsonConvert.SerializeObject(parentJObject, Formatting.Indented);
    }

    public void SetModelData(string JString)
    {
        JObject responseData = JObject.Parse(JString);

        JArrayToVenderProductStructList((JArray)responseData["venderModel"]["venderProductStructs"]);
        selectedVenderProductNumber = -1;

        NotifyVenderProductStructObservers();
        NotifySelectedVenderProductNumberObservers();
    }
    // -------------------------------

    // 2. GetModelData()과 SetModelData()에서 데이터를 가공하는데 사용하는 함수들.
    private void JArrayToVenderProductStructList(JArray jArrayData)
    {
        this.venderProductStructs.Clear();

        for (int i = 0; i < jArrayData.Count; i++)
        {
            venderProductStructs.Add(JsonConvert.DeserializeObject<VenderProductStruct>(JsonConvert.SerializeObject(jArrayData[i])));
        }
    }
    // -------------------------------

    // 3. View와 Model이 데이터를 공유하기 위해 사용되는 부분.
    public void RegisterVenderProductStructObserver(VenderProductStructObserver observer)
    {
        venderProductStructObservers.Add(observer);
    }
    public List<VenderProductStruct> GetVenderProductStruct()
    {
        return venderProductStructs;
    }
    private void NotifyVenderProductStructObservers()
    {
        for (int i = 0; i < venderProductStructObservers.Count; i++)
        {
            venderProductStructObservers[i].UpdateVenderProductStructObserver();
        }
    }


    public void RegisterSelectedVenderProductNumberObserver(SelectedVenderProductNumberObserver observer)
    {
        selectedVenderProductNumberObservers.Add(observer);
    }
    public int GetSelectedVenderProductNumber()
    {
        return selectedVenderProductNumber;
    }
    private void NotifySelectedVenderProductNumberObservers()
    {
        for (int i = 0; i < selectedVenderProductNumberObservers.Count; i++)
        {
            selectedVenderProductNumberObservers[i].UpdateSelectedVenderProductNumberObserver();
        }
    }

    // 5. INPCMessageViewMediator와 연결되는 부분.
    public void RegisterNPCMessageObserver(NPCMessageObserver observer)
    {
        NPCMessageMediator.RegisterNPCMessageObserver(observer);
    }

    public string GetNPCMessage()
    {
        return NPCMessageMediator.GetNPCMessage();
    }

    public void NotifyNPCMessageObservers(string NPCName, string messageContentType)
    {
        NPCMessageMediator.NotifyNPCMessageObservers(NPCName, messageContentType);
    }
    // ---------------------------------

    // 6. Controller가 접근하는 부분.
     public void UpdateButtonOnClicked()
    {
        messageClassifier.ChangeMessageCode(104);
    }

    public void UpdateYesButtonOnClicked()
    {
        if (gameManger.ServerRequest("RequestVenderProductListUpdate_test"))
        {
            messageClassifier.ChangeMessageCode(105);
        }else
        {
            messageClassifier.ChangeMessageCode(106);
        }
    }

    public void BuyButtonOnClicked()
    {
        if (selectedVenderProductNumber == -1)
        {
            messageClassifier.ChangeMessageCode(101);
            return;
        }

        if (gameManger.ServerRequest("RequestVenderProductBuy_test"))
        {
            this.NotifyNPCMessageObservers("VenderNPC", "ThankYouForPurchase");
            messageClassifier.ChangeMessageCode(102);
        }
        else
        {
            // 구입 실패
            messageClassifier.ChangeMessageCode(103);
        }
    }

    public void OnClickedVenderProductSlot(int prodcutName)
    {
        selectedVenderProductNumber = prodcutName;

        NotifySelectedVenderProductNumberObservers();
    }

    public IMessageClassifier GetMessageClassifier()
    {
        return messageClassifier.GetMessageClassifier();
    }
}
