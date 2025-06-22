using NaughtyAttributes;
using UniRx;
using UnityEngine;

namespace Core.Player.Data {
    [CreateAssetMenu(menuName = "Data/PlayerData", fileName = "PlayerData", order = 0)]
    public class PlayerData : ScriptableObject {
        public Vector2ReactiveProperty currentAimDirection = new (Vector2.left);
    }
}