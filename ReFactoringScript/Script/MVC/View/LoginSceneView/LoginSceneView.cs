using UnityEngine;
using UnityEngine.UI;

public interface ILoginSceneView
{
    public void ActivateButton();
    public void DisableButton();
}

public class LoginSceneView : ILoginSceneView
{
    private ILoginSceneController controller;

    private RectTransform canvas;
    private Button loginButton;
    private Button withdrawButton;
    private Button registerButton;

    // LoginScene�� ���� ȭ���� ������ ���� �ϳ��� �����ؾ� �Ѵ�.
    // LoginScene�� ���� ȭ���� ����ϰ� �ϴ� �����ڿ�, LoginScene ��ư ��� ���� ������ 2���� ����ߴ�.
    // ����, ����� ���� �ִ� �κ��̴�.
    public LoginSceneView(ILoginSceneController controller)
    {
        this.controller = controller;

        canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();

        SetLoginSceneView();
    }

    private void SetLoginSceneView()
    {
        loginButton = canvas.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Button>();
        withdrawButton = canvas.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>();
        registerButton = canvas.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();

        loginButton.onClick.AddListener(delegate { controller.LoginAccountButtonOnClicked(); });
        withdrawButton.onClick.AddListener(delegate { controller.WithdrawButtonOnClicked(); });
        registerButton.onClick.AddListener(delegate { controller.RegisterAccountButtonOnClicked(); });
    }

    public void ActivateButton()
    {
        canvas.GetChild(0).GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void DisableButton()
    {
        canvas.GetChild(0).GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

}
