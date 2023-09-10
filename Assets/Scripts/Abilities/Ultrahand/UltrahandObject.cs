using UnityEngine;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class UltrahandObject : MonoBehaviour, IUltrahandable
    {
        [SerializeField] private Rigidbody body;
        //private FixedJoint joint;

        private bool lockRotation;

        public bool IsInteracting
        {
            get => body.isKinematic;
            private set => body.isKinematic = value;
        }

        private void Reset() => body = GetComponent<Rigidbody>();

        public void Interact() => IsInteracting = true;
        public void CancelInteraction() => IsInteracting = false;

        public void Rotate(Vector3 angle, float time)
        {
            if (lockRotation) return;

            lockRotation = true;

            var seq = LeanTween.sequence();
            var rotation = transform.localEulerAngles + angle;

            seq.append(LeanTween.rotateLocal(gameObject, rotation, time).setEaseInOutBack());
            seq.append(() => lockRotation = false);
        }

        /*public void Move(Transform holder)
        {
            if (!IsInteracting) return;

            var direction = (holder.position - body.position).normalized;
            var hasHit = body.SweepTest(direction, out RaycastHit hit, maxDistance: 0.1F);
            var hasStaticCollision = hasHit && hit.transform.gameObject.isStatic;
            var position = hasStaticCollision ? hit.point : holder.position;

            body.Move(position, holder.rotation);
        }*/
    }
}