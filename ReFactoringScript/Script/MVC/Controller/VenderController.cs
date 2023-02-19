
public interface IVenderController { }

public class VenderController : IVenderController, IVenderMenuView_VenderModelController, IConfirmMessageViewController, IYesOrNoMessageViewController
{
    private IVenderModel venderModel;

    private IVenderMenuView_VenderModel venderMenuView;

    private IConfirmMessageView confirmMessageView;
    private IYesOrNoMessageView yesOrNoMessageView;

    public VenderController(IVenderModel venderModel)
    {
        this.venderModel = venderModel;

        venderMenuView = new VenderMenuView_VenderModel(this.venderModel, this);

        confirmMessageView = new ConfirmMessageView(venderModel, this);
        yesOrNoMessageView = new YesOrNoMessageView(venderModel, this);
    }

    // 2. IVenderMenuView_VenderModelController�� ����
    public void UpdateButtonOnClicked()
    {
        venderMenuView.DisableButton();
        venderModel.UpdateButtonOnClicked();
    }

    public void BuyButtonOnClicked(int selectedVenderProductNumber)
    {
        venderMenuView.DisableButton();
        venderModel.BuyButtonOnClicked();
    }

    // VenderProductSlotClickEvent.cs���� ����ϴ� ����� �����Ѵ�.
    public void OnClickedVenderProductSlot(int productName)
    {
        venderMenuView.UnMarkSelectedProductSlot();
        venderModel.OnClickedVenderProductSlot(productName);
    }

    // 4. IYesOrNoMessageViewController�� ����.
    // Update ��ư�� Ŭ���� ��, Ȯ���� �� ���ȴ�.
    public void OnYesButtonClicked(int messageCode)
    {
        yesOrNoMessageView.DeleteYesOrNoMessagePanel();

        if (messageCode == 104) venderModel.UpdateYesButtonOnClicked();
    }

    public void OnNoButtonClicked(int messageCode)
    {
        yesOrNoMessageView.DeleteYesOrNoMessagePanel();
        venderMenuView.ActivateButton();
    }

    // 5. IConfirmMessageViewController�� ����
    public void OnConfirmButtonClicked(int messageCode)
    {
        confirmMessageView.DeleteConfirmMessagePanel();
        venderMenuView.ActivateButton();
    }
}
