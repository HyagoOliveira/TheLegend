using UnityEngine;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class UltrahandObject : MonoBehaviour, IUltrahandable
    {
        [SerializeField] private Rigidbody body;

        public bool IsInteracting
        {
            get => body.isKinematic;
            private set => body.isKinematic = value;
        }

        //private FixedJoint joint;

        private void Reset() => body = GetComponent<Rigidbody>();

        public void Move(Vector3 velocity)
        {
            if (!IsInteracting) return;

            var moveDirection = velocity.normalized;
            var hasHit = body.SweepTest(moveDirection, out RaycastHit hit, maxDistance: 0.1F);
            var hasStaticCollision = hasHit && hit.transform.gameObject.isStatic;
            if (hasStaticCollision) velocity = Vector3.zero;

            body.position += velocity;
        }

        public void Interact() => IsInteracting = true;
        public void CancelInteraction() => IsInteracting = false;
    }
}