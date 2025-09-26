using UnityEngine;

[CreateAssetMenu(fileName = "New PhaseData", menuName = "Game/Phase Data")]
public class PhaseData : ScriptableObject
{
    public string phaseName;
    public Sprite backgroundImage;
}