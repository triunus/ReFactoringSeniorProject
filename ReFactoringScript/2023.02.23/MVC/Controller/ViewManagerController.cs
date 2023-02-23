public interface IViewManagerController : IViewManagerGameLobbyViewController { }

public class ViewManagerController : IViewManagerController
{
    private IViewManagerModel viewManagerModel;
    private IViewManagerGameLobbyView viewManagerGameLobbyView;

    public ViewManagerController(IViewManagerModel model)
    {
        this.viewManagerModel = model;

        viewManagerGameLobbyView = new ViewManagerGameLobbyView(model, this);
    }

    public void OnClickedChangeMenuButton(MenuName nextMenuName)
    {
        // View�� ���� �����Ǿ� �ִ� MenuName�� ����Ͽ� ���� View�� ��Ȱ��ȭ ��Ų��.
        viewManagerGameLobbyView.DisableCurrentMenuPanel();

        viewManagerModel.ChangeMenuOperation(nextMenuName);
    }
}
