using UnityEngine;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class UltrahandObject : MonoBehaviour, IUltrahandable
    {
        [SerializeField] private Rigidbody body;
        //private FixedJoint joint;

        private const float skin = 0.08f;
        private const float halfSkin = skin * 0.5f;

        public bool IsInteracting
        {
            get => body.isKinematic;
            private set => body.isKinematic = value;
        }

        private void Reset() => body = GetComponent<Rigidbody>();

        public void Interact() => IsInteracting = true;
        public void CancelInteraction() => IsInteracting = false;

        public bool CanMove(Vector3 direction, float speed)
        {
            var position = transform.position;

            body.position = position - direction * halfSkin;

            var distance = speed + skin;
            var hasHit = body.SweepTest(direction, out RaycastHit hit, distance);
            var hasStaticCollision = hasHit && hit.transform.gameObject.isStatic;

            body.position = position;

            return !hasStaticCollision;
        }
    }
}