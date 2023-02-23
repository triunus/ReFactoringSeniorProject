public interface IView
{
    /// <summary>
    /// View�� �ʱ� ����. Model�� ������ ���� �ʴ�, View �Ӽ����� �����Ѵ�.
    /// </summary>
//    private void SetView();

    /// <summary>
    /// Model�� ���� ������ �޴� View ���� ������ �����.
    /// </summary>
//    public void RemoveView();

    /// <summary>
    /// Model�� ���� ������ �޴� View ���� ������ �����Ѵ�.
    /// </summary>
    public void UpdateView();

    /// <summary>
    /// View�� ��ư Ŭ���� Ȱ��ȭ ��ų �� ����Ѵ�.
    /// </summary>
    public void ActivateButton();

    /// <summary>
    /// View�� ��ư Ŭ���� ��Ȱ��ȭ ��ų �� ����Ѵ�.
    /// </summary>
    public void DisableButton();

    /// <summary>
    /// View�� Ȱ��ȭ ��ų �� ����Ѵ�.
    /// </summary>
    public void ActivateView();

    /// <summary>
    /// View�� ��Ȱ��ȭ ��ų �� ����Ѵ�.
    /// </summary>
    public void DisableView();
}