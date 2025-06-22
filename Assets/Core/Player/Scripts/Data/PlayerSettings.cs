using NaughtyAttributes;
using UnityEngine;

namespace Core.Player.Data {
    [CreateAssetMenu(menuName = "Data/"+nameof(PlayerSettings), fileName = nameof(PlayerSettings), order = 0)]
    public class PlayerSettings : ScriptableObject {
        [field:SerializeField] public ContactFilter2D GroundCheckContactFilter { get; private set; }
        
        [field:Foldout("Rocket Refill")] [field:SerializeField] public LayerMask PlatformLayerMask { get; private set; }
        [field:Foldout("Rocket Refill")] [field:SerializeField] public float SideRaycastDistance { get; private set; } = .35f;
        [field:Foldout("Rocket Refill")] [field:SerializeField] public float GroundCheckDistance { get; private set; }
        [field:Foldout("Rocket Refill")] [field:SerializeField] public int MaxRocketAmount { get; private set; }
        [field:Foldout("Rocket Refill")] [field:SerializeField] public int MinRocketAmountOnGrabAnchorPoint { get; private set; } = 1;
        [field:Foldout("Rocket Refill")] [field:SerializeField] public float RocketCooldown { get; private set; } = .1f;
        [field:Space]
        [field:Foldout("Rocket Inertia")] [field:SerializeField] public float SlingshotStrengthMultiplier { get; private set; } = 1;
        [field:Foldout("Rocket Inertia")] [field:SerializeField] public float RocketStrength { get; private set; }
        [field:Foldout("Rocket Inertia")] [field:SerializeField] public float BaseMomentumMultiplierOnFireIfSameDir { get; private set; } = .5f;
        [field:Foldout("Rocket Inertia")] [field:SerializeField] public float MomentumMultiplierOnFireSameDirection { get; private set; } = .25f;
        [field:Foldout("Rocket Inertia")] [field:SerializeField] public float MomentumMultiplierOnFireOppositeDirection { get; private set; } = .1f;
        
        [field:Space]
        [field:Foldout("RECOIL")][field:SerializeField] public float RecoilStrength { get; private set; } = .1f;
        [field:Foldout("RECOIL")][field:SerializeField] public float RecoilDuration { get; private set; } = .1f;
        [field:Foldout("RECOIL")][field:SerializeField] public float RecoilRecoverDuration { get; private set; } = .1f;
        [field:Foldout("RECOIL")][field:SerializeField] public float RecoilMaxDistance { get; private set; } = .2f;
    }
}