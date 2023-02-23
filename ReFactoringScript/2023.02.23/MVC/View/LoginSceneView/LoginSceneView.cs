using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LoginSceneView에서 Controller의 기능을 사용할 때 사용한다.
/// LoginSceneView을 사용할려면, Controller는 해당 인터페이스를 상속해야한다.
/// </summary>
public interface ILoginSceneViewController
{
    public void LoginAccountButtonOnClicked();
    public void RegisterAccountButtonOnClicked();
    public void WithdrawButtonOnClicked();
}

/// <summary>
/// Controller에서 LoginSceneView 기능을 사용할 때 사용한다.
/// </summary>
public interface ILoginSceneView
{
    /// <summary>
    /// 특정 Panle 하위의 마우스 입력을 활성화 시킨다.
    /// </summary>
    public void ActivateButton();
    /// <summary>
    /// 특정 Panle 하위의 마우스 입력을 비활성화 시킨다.
    /// </summary>
    public void DisableButton();
}

public class LoginSceneView : ILoginSceneView
{
    private ILoginSceneViewController controller;

    private RectTransform canvas;
    private Button loginButton;
    private Button withdrawButton;
    private Button registerButton;

    public LoginSceneView(ILoginSceneViewController controller)
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
