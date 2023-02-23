using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// ConfirmMessageView에서 Controller의 기능을 사용할 때 사용한다.
/// ConfirmMessageView을 사용할려면, Controller는 해당 인터페이스를 상속해야한다.
/// </summary>
public interface IConfirmMessageViewController
{
    /// <summary>
    /// 버튼의 'Confirm 버튼'을 클릭하였을 때 방생하는 이벤트를 정의한다.
    /// </summary>
    /// <param name="messageCode"> 특정 messageCode마다 다른 작업을 발생시키고 싶을 때 사용한다. </param>
    public void OnConfirmButtonClicked(int messageCode);
}

/// <summary>
/// Controller에서 ConfirmMessageView 기능을 사용할 때 사용한다.
/// </summary>
public interface IConfirmMessageView
{
    /// <summary>
    /// ConfirmMessageView Panel을 삭제한다.
    /// </summary>
    public void DeleteConfirmMessagePanel();
}

public class ConfirmMessageView : IConfirmMessageView, MessageObserver
{
    private IMessageObserver model;
    private IConfirmMessageViewController controller;

    private RectTransform canvas;
    private GameObject confirmMessageView;

    // View를 위한 Model과 Controller Interface가 존재하기에, 하나의 생성자로 여러 Model과 Controller를 인지할 수 있다.
    public ConfirmMessageView(IMessageObserver model, IConfirmMessageViewController controller)
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
            SetConfirmMessagePanel(message);
        }
    }

    // MessagePanel의 생성, 위치 조정, 내용 기입, 버튼 등록.
    private void SetConfirmMessagePanel(string[] message)
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
