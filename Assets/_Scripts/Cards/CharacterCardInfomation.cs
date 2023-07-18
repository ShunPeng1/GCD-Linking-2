
using Shun_Card_System;
using UnityEngine;


[CreateAssetMenu(fileName = "Character Card Information", menuName = "Character Card Information/")]
public abstract class CharacterCardInfomation : BaseCardInformation
{
    public abstract void Init();

    public abstract void ExecuteAbility1();

    public abstract void ExecuteAbility2();
}
