using UnityEngine;

public interface IUserController { }

public class UserController : IUserController, IGameLobbyViewController, ICharacterMenuViewController, IInventoryMenuViewController, IVenderMenuView_UserModelController
{
    private IUserModel userModel;

    private IGameLobbyView gameLobbyView;
    private ICharacterMenuView characterMenuView;
    private IInventoryMenuView inventoryMenuView;
    private IVenderMenuView_UserModel venderMenuView_UserModel;

    public UserController(IUserModel userModel)
    {
        this.userModel = userModel;

        gameLobbyView = new GameLobbyView(this.userModel, this);
        characterMenuView = new CharacterMenuView(this.userModel, this);
        inventoryMenuView = new InventoryMenuView(this.userModel, this);
        venderMenuView_UserModel = new VenderMenuView_UserModel(this.userModel, this);
    }

    // 1. IGameLobbyViewController 何盒.
    public void OnShowSelectionInformationButtonClicked()
    {
        gameLobbyView.ControllerSelectionInformationPanel();
    }

    // IGameLobbyViewController 备泅 何盒.
    public void OnClickedCharacterSlot(int selectedCharacterNumber)
    {
        characterMenuView.UnMarkSelectedCharacterSlot();
        userModel.UpdateSelectedCharacterNumber(selectedCharacterNumber);
    }

    // IInventoryMenuViewController 备泅 何盒.
    public void OnClickedAllButton()
    {
        inventoryMenuView.DisplayAllSkill();
    }
    public void OnClickedNotNFTButton()
    {
        inventoryMenuView.DisplayNotNFTSkill();
    }
    public void OnClickedNFTButton()
    {
        inventoryMenuView.DisplayNFTSkill();
    }
    
    public void OnClickedSelectedSkillSlot(string skillUniqueNumber)
    {
        inventoryMenuView.UnMarkSelectedSkillSlot();
        userModel.UpdateSelectedSkillNumber_Pop(skillUniqueNumber);
    }

    public void OnClickedUnSelectedSkillSlot(string skillUniqueNumber)
    {
        userModel.UpdateSelectedSkillNumber_Add(skillUniqueNumber);
    }
}
