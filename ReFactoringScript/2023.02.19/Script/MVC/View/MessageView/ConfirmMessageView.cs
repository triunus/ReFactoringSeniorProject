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

    // View�� ���� Model�� Controller Interface�� �����ϱ⿡, �ϳ��� �����ڷ� ���� Model�� Controller�� ������ �� �ִ�.
    public ConfirmMessageView(IMessageViewMediator model, IConfirmMessageViewController controller)
    {
        this.model = model;
        this.controller = controller;

        this.model.RegisterMessageObserver(this);

        canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();
    }

    // Model�� ���� ����Ǹ� Observer�� ����� Update �޼ҵ带 ȣ���Ѵ�.
    public void UpdateMessageObserver()
    {
        string[] message = this.model.GetMessage();

        if(MessageViewType.ConfirmMessageView.Equals((MessageViewType)Enum.Parse(typeof(MessageViewType), message[1])))
        {
            CreateConfirmMessagePanel(message);
        }
    }

    // MessagePanel�� ����, ��ġ ����, ���� ����, ��ư ���.
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

    // Controller���� Panel�� ������ �� ����ϴ� �޼ҵ�.
    public void DeleteConfirmMessagePanel()
    {
        MonoBehaviour.Destroy(confirmMessageView);
    }
}
