using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LoginSceneView���� Controller�� ����� ����� �� ����Ѵ�.
/// LoginSceneView�� ����ҷ���, Controller�� �ش� �������̽��� ����ؾ��Ѵ�.
/// </summary>
public interface ILoginSceneViewController
{
    public void LoginAccountButtonOnClicked();
    public void RegisterAccountButtonOnClicked();
    public void WithdrawButtonOnClicked();
}

/// <summary>
/// Controller���� LoginSceneView ����� ����� �� ����Ѵ�.
/// </summary>
public interface ILoginSceneView
{
    /// <summary>
    /// Ư�� Panle ������ ���콺 �Է��� Ȱ��ȭ ��Ų��.
    /// </summary>
    public void ActivateButton();
    /// <summary>
    /// Ư�� Panle ������ ���콺 �Է��� ��Ȱ��ȭ ��Ų��.
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
