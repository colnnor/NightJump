
//TODO: delete this
/*

    IEnumerator MoveCharacter(Vector3 endPosition)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;

        isMoving = true;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;

            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;
        isMoving = false;
    }
*/

public interface IDeadly
{

}