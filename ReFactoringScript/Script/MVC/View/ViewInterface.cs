public interface IView
{
    /// <summary>
    /// View의 초기 설정. Model에 영향을 받지 않는, View 속성들을 정의한다.
    /// </summary>
//    private void SetView();

    /// <summary>
    /// Model의 값에 영향을 받는 View 내의 값들을 지운다.
    /// </summary>
//    public void RemoveView();

    /// <summary>
    /// Model의 값에 영향을 받는 View 내의 값들을 설정한다.
    /// </summary>
    public void UpdateView();

    /// <summary>
    /// View의 버튼 클릭을 활성화 시킬 때 사용한다.
    /// </summary>
    public void ActivateButton();

    /// <summary>
    /// View의 버튼 클릭을 비활성화 시킬 때 사용한다.
    /// </summary>
    public void DisableButton();

    /// <summary>
    /// View를 활성화 시킬 때 사용한다.
    /// </summary>
    public void ActivateView();

    /// <summary>
    /// View를 비활성화 시킬 때 사용한다.
    /// </summary>
    public void DisableView();
}