using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlip : MonoBehaviour
{
    [Header("Customizables")]
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float animationTime = 1f;
    
    private bool anim;

    private float timer = 0f;
    private bool flipping;
    Vector3 goal;
    Vector3 current;
    float startZ;

    private void Awake() => anim = PlayerPrefs.GetInt("RotationAnimation") == 1;
    private void Update()
    {
        if (flipping)
        {
            timer += Time.deltaTime;
            if (timer > animationTime)
            {
                EndFlip();
                return;
            }
            current.z = Mathf.LerpAngle(
                startZ,
                goal.z,
                animationCurve.Evaluate(timer / animationTime)
                );
            
            transform.eulerAngles = current;
        }
    }
    public void AutoFlip(PieceColor pieceColor) => Flip(pieceColor);
    public void Flip() => Flip(PieceColor.None);
    private void Flip(PieceColor pieceColor)
    {
        if (!anim)
        {
            FlipNoAnim(pieceColor);
            return;
        }
        if (flipping)
            EndFlip();
        PrepareForFlip(pieceColor);
    }
    private void FlipNoAnim(PieceColor pieceColor)
    {
        goal = transform.eulerAngles;
        if (pieceColor == PieceColor.None)
            goal.z = goal.z == 0 ? 180 : 0;
        else
            goal.z = pieceColor == PieceColor.White ? 180 : 0;
        transform.eulerAngles = goal;
    }
    private void PrepareForFlip(PieceColor pieceColor)
    {
        flipping.SetTrue();
        timer = 0;
        goal = transform.eulerAngles;
        if (pieceColor != PieceColor.None)
            goal.z = pieceColor == PieceColor.White ? 0 : -180;
        transform.eulerAngles = goal;
        goal.z -= 180;
        current = transform.eulerAngles;
        startZ = transform.eulerAngles.z;
    }
    private void EndFlip()
    {
        flipping.SetFalse();
        if (goal.z != -180)
            goal.z = 0;
        transform.eulerAngles = goal;
    }
}
