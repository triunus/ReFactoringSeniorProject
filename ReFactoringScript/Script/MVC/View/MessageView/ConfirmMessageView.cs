using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IConfirmMessageViewController
{
    public void OnConfirmButtonClicked(int messageCode);
}

public interface IConfirmMessageView
{
    public void DeleteConfirmMessagePanel();
}

public class ConfirmMessageView : IConfirmMessageView, MessageObserver
{
    private IMessageViewMediator model;
    private IConfirmMessageViewController controller;

    private RectTransform canvas;
    private GameObject confirmMessageView;

    // View를 위한 Model과 Controller Interface가 존재하기에, 하나의 생성자로 여러 Model과 Controller를 인지할 수 있다.
    public ConfirmMessageView(IMessageViewMediator model, IConfirmMessageViewController controller)
    {
        this.model = model;
        this.controller = controller;

        this.model.RegisterMessageObserver(this);

        canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();
    }

    // Model의 값이 변경되면 Observer로 연결된 Update 메소드를 호출한다.
    public void UpdateMessageObserver()
    {
        string[] message = this.model.GetMessage();

        if(MessageViewType.ConfirmMessageView.Equals((MessageViewType)Enum.Parse(typeof(MessageViewType), message[1])))
        {
            CreateConfirmMessagePanel(message);
        }
    }

    // MessagePanel의 생성, 위치 조정, 내용 기입, 버튼 등록.
    private void CreateConfirmMessagePanel(string[] message)
    {
        confirmMessageView = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefab/MessagePanel/ConfirmMessagePanel"));
 
        confirmMessageView.transform.SetParent(canvas);
        confirmMessageView.SetActive(true);

        confirmMessageView.GetComponent<RectTransform>().offsetMin = new Vector2(Screen.width - 1895, Screen.height - 1055);
        confirmMessageView.GetComponent<RectTransform>().offsetMax = new Vector2(1895 - Screen.width, 1055 - Screen.height);

        confirmMessageView.GetComponent<RectTransform>().GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message[2];
        confirmMessageView.GetComponent<RectTransform>().GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = message[3];

        confirmMessageView.GetComponent<RectTransform>().GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { controller.OnConfirmButtonClicked(Int32.Parse(message[0])); });
    }

    // Controller에서 Panel을 삭제할 때 사용하는 메소드.
    public void DeleteConfirmMessagePanel()
    {
        MonoBehaviour.Destroy(confirmMessageView);
    }
}
