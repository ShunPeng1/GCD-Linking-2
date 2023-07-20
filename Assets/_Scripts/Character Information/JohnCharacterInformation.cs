using UnityEngine;

namespace _Scripts.Cards.Character_Information
{
    [CreateAssetMenu(fileName = "Character Information", menuName = "Character Information/John Information")]
    public class JohnCharacterInformation : CharacterInformation
    {
        

        public override void MoveAbility()
        {
            Debug.Log("Ability 1");
        }

        public override void Ability2()
        {
            Debug.Log("Ability 2");
        }
    }
}