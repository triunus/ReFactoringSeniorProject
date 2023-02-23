using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IYesOrNoMessageViewController
{
    public void OnYesButtonClicked(int messageCode);
    public void OnNoButtonClicked(int messageCode);
}

public interface IYesOrNoMessageView
{
    public void DeleteYesOrNoMessagePanel();
}

public class YesOrNoMessageView : IYesOrNoMessageView, MessageObserver
{
    private IMessageViewMediator model;
    private IYesOrNoMessageViewController controller;

    private RectTransform canvas;
    private GameObject yesOrNoMessageView;

    // View�� ���� Model�� Controller Interface�� �����ϱ⿡, �ϳ��� �����ڷ� ���� Model�� Controller�� ������ �� �ִ�.
    public YesOrNoMessageView(IMessageViewMediator model, IYesOrNoMessageViewController controller)
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

        if (MessageViewType.YesOrNoMessageView.Equals((MessageViewType)Enum.Parse(typeof(MessageViewType), message[1])))
        {
            CreateYesOrNoMessagePanel(message);
        }
    }

    // MessagePanel�� ����, ��ġ ����, ���� ����, ��ư ���.
    private void CreateYesOrNoMessagePanel(string[] message)
    {
        yesOrNoMessageView = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefab/MessagePanel/YesOrNoMessagePanel"));

        yesOrNoMessageView.transform.SetParent(canvas);
        yesOrNoMessageView.SetActive(true);

        yesOrNoMessageView.GetComponent<RectTransform>().offsetMin = new Vector2(Screen.width - 1895, Screen.height - 1055);
        yesOrNoMessageView.GetComponent<RectTransform>().offsetMax = new Vector2(1895 - Screen.width, 1055 - Screen.height);

        yesOrNoMessageView.GetComponent<RectTransform>().GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message[2];
        yesOrNoMessageView.GetComponent<RectTransform>().GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = message[3];

        yesOrNoMessageView.GetComponent<RectTransform>().GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { controller.OnNoButtonClicked(Int32.Parse(message[0])); });
        yesOrNoMessageView.GetComponent<RectTransform>().GetChild(0).GetChild(2).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { controller.OnYesButtonClicked(Int32.Parse(message[0])); });
    }

    // Controller���� Panel�� ������ �� ����ϴ� �޼ҵ�.
    public void DeleteYesOrNoMessagePanel()
    {
        MonoBehaviour.Destroy(yesOrNoMessageView);
    }
}
