public class CameraFollowPoint : Singleton<CameraFollowPoint>
{
    public void CallTurn(bool isFacingRight) => LeanTween.rotateY(gameObject, DetermineEndRotation(isFacingRight), 0.5f).setEaseInOutSine();

    float DetermineEndRotation(bool isFacingRight)
    {
        if(isFacingRight)
            return 180f;
        else
            return 0f;
    }
}
