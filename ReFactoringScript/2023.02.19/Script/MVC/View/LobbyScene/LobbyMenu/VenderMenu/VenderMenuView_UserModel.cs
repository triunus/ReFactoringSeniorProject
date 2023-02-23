using UnityEngine;

public interface IVenderMenuView_UserModelController { }

public interface IVenderMenuView_UserModel { }

public class VenderMenuView_UserModel : IVenderMenuView_UserModel, UserInformationObserver
{
    private IUserInformationObserver userInformationModel;

    private IVenderMenuView_UserModelController venderMenuView_UserModelController;
    
    private RectTransform canvas = null;

    private RectTransform coinPanel = null;

    private UserData userData = null;

    public VenderMenuView_UserModel(IUserModel model, IVenderMenuView_UserModelController controller)
    {
        this.userInformationModel = model;
        this.venderMenuView_UserModelController = controller;

        userInformationModel.RegisterUserInformationObserver(this);

        SetView();
    }

    private void SetView()
    {
        this.canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();

        coinPanel = canvas.GetChild(0).GetChild(1).GetChild(3).GetChild(0).GetChild(2).GetComponent<RectTransform>();
    }

    public void UpdateUserInformationObserver()
    {
        userData = userInformationModel.GetUserInformation();

        SetUserInformation();
    }

    private void SetUserInformation()
    {
        coinPanel.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(userData.Coin);
        coinPanel.GetChild(2).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(userData.PaidCoin);
    }
}
