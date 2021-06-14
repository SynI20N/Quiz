using UnityEngine.EventSystems;

public interface IBouncing : IPointerClickHandler
{
    void BounceIn();
    void BounceOut();
    void EaseInBounce();
}
public interface IFading : IPointerClickHandler
{
    void FadeIn();
    void FadeOut();
}
