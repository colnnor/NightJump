using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MenuEnemyController : MonoBehaviour
{
    [SerializeField] private EnemyMovementValues enemyMovementValues;
    [SerializeField] private GridData gridData;

    [SerializeField] private Animator animator;


    Vector3 nextRotation;

    List<int> ints = new() { 0, 1, 2, 3 };

    int enemyJumpHash = Animator.StringToHash("Jump");

    private void Start()
    {
        InvokeRepeating(nameof(MoveOneSquare), 0, 2);
    }

    void MoveOneSquare()
    {
        Vector3 nextPosition = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            GetNextPositionAndRotation(ints[i], ref nextPosition);
            if (nextPosition.x >= 0 && nextPosition.x < gridData.Size && nextPosition.z >= 0 && nextPosition.z < gridData.Size)
            {
                animator.SetTrigger(enemyJumpHash);

                transform.DOMove(nextPosition, enemyMovementValues.TranslationTime).SetEase(Ease.Linear);
                transform.DORotate(nextRotation, enemyMovementValues.TranslationTime/2).SetEase(Ease.Linear);

                ints.Shuffle();
                break;
            }
        }
    }

    Vector3 GetNextPositionAndRotation(int random, ref Vector3 nextPosition)
    {
        nextPosition = Vector3Int.zero;

        switch (random)
        {
            case 0:
                nextPosition = transform.position.Add(z: 1);
                nextRotation = new Vector3(0, 0, 0);
                break;
            case 1:
                nextPosition = transform.position.Add(z: -1);
                nextRotation = new Vector3(0, 180, 0);
                break;
            case 2:
                nextPosition = transform.position.Add(x: -1);
                nextRotation = new Vector3(0, 270, 0);
                break;
            case 3:
                nextPosition = transform.position.Add(x: 1);
                nextRotation = new Vector3(0, 90, 0);
                break;
        }

        return nextPosition;
    }
}
