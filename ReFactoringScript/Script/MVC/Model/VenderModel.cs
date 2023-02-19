using System.Collections.Generic;

using UnityEngine;

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


public interface IVenderModel : IModel, IMessageViewMediator, INPCMessageViewMediator, IVenderProductStructObserver, ISelectedVenderProductNumberObserver
{
    public void UpdateButtonOnClicked();
    public void UpdateYesButtonOnClicked();
    public void BuyButtonOnClicked();
    public void OnClickedVenderProductSlot(int prodcutName);
}

public class VenderModel : IVenderModel
{
    private IGameManagerForModel gameManger = null;
    private IMessageViewMediator messageMediator = null;
    private INPCMessageViewMediator NPCMessageMediator = null;

    private List<VenderProductStructObserver> venderProductStructObservers = new List<VenderProductStructObserver>();
    private List<SelectedVenderProductNumberObserver> selectedVenderProductNumberObservers = new List<SelectedVenderProductNumberObserver>();

    private List<VenderProductStruct> venderProductStructs = new List<VenderProductStruct>();
    private int selectedVenderProductNumber;
    
    public VenderModel()
    {
        gameManger = GameManager.GetGameManager();
        messageMediator = new MessageViewMediator();
        NPCMessageMediator = new NPCMessageViewMediator();

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
    public System.Collections.Generic.List<VenderProductStruct> GetVenderProductStruct()
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

    // 4. MessageMediator와 연결되는 부분.
    public void RegisterMessageObserver(MessageObserver observer)
    {
        messageMediator.RegisterMessageObserver(observer);
    }

    public string[] GetMessage()
    {
        return messageMediator.GetMessage();
    }

    // GM에서, AccountModel의 MessageMediator를 통해 메시지를 출력하고 싶을 때 사용한다.
    public void NotifyMessageObservers(int messageCode)
    {
        messageMediator.NotifyMessageObservers(messageCode);
    }
    // ---------------------------------------------------------------

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
        messageMediator.NotifyMessageObservers(104);
    }

    public void UpdateYesButtonOnClicked()
    {
        if (gameManger.ServerRequest("RequestVenderProductListUpdate_test"))
        {
            messageMediator.NotifyMessageObservers(105);
        }else
        {
            messageMediator.NotifyMessageObservers(106);
        }
    }

    public void BuyButtonOnClicked()
    {
        if (selectedVenderProductNumber == -1)
        {
            messageMediator.NotifyMessageObservers(101);
            return;
        }

        if (gameManger.ServerRequest("RequestVenderProductBuy_test"))
        {
            this.NotifyNPCMessageObservers("VenderNPC", "ThankYouForPurchase");
            messageMediator.NotifyMessageObservers(102);
        }
        else
        {
            // 구입 실패
            messageMediator.NotifyMessageObservers(103);
        }
    }

    public void OnClickedVenderProductSlot(int prodcutName)
    {
        selectedVenderProductNumber = prodcutName;

        NotifySelectedVenderProductNumberObservers();
    }

}
