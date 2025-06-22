using UnityEngine;
using UnityEngine.Events;

namespace Core.Player.Events {
    [CreateAssetMenu(menuName = "Data/PlayerEvents", fileName = "PlayerEvents", order = 0)]
    public class PlayerEvents : ScriptableObject {
        public struct RocketFiredData {
            public Vector2 position;
            public Vector2 direction;
        }
        public UnityEvent<RocketFiredData> onRocketFireRequested = new();
        public UnityEvent<RocketFiredData> onRocketFired = new();
    }

}