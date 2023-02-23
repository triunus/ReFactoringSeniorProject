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

    // LoginScene의 메인 화면은 생성과 동시 하나만 존재해야 한다.
    // LoginScene의 메인 화면을 출력하게 하는 생성자와, LoginScene 버튼 제어를 위한 생성자 2개를 사용했다.
    // 차후, 사라질 수도 있는 부분이다.
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
