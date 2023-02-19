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
        // View에 현재 배정되어 있는 MenuName을 사용하여 기존 View를 비활성화 시킨다.
        viewManagerGameLobbyView.DisableCurrentMenuPanel();

        viewManagerModel.ChangeMenuOperation(nextMenuName);
    }
}
