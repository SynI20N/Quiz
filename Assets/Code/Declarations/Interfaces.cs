using UnityEngine.EventSystems;

public interface IBounceable : IPointerClickHandler
{
    void BounceIn();
    void BounceOut();
    void EaseInBounce();
}
public interface IFadeable : IPointerClickHandler
{
    void FadeIn();
    void FadeOut();
}
