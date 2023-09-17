using UnityEngine;
using System.Collections;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    public sealed class Ultrahand : AbstractAbility<UltrahandSettings>
    {
        [field: SerializeField] public Transform Hand { get; private set; }
        [field: SerializeField] public Transform Holder { get; private set; }

        [SerializeField] private GameObject directionalIndicator;

        private static Coroutine rotateCoroutine;
        private static Quaternion desiredRotation;
        private static readonly WaitForEndOfFrame waitOneFrame = new();

        private void Update() => DrawLineBetweenHandAndObject();

        internal void AttachHolder(Transform ultrahandable)
        {
            Holder.position = ultrahandable.position;

            ultrahandable.parent = Holder;

            var localPos = Holder.localPosition;
            localPos.x = 0;
            Holder.localPosition = localPos;
        }

        internal void UnttachHolder(Transform ultrahandable)
        {
            ultrahandable.parent = null;
        }

        internal void EnableDirectionalIndicator(bool enable) =>
            directionalIndicator.SetActive(enable);

        private void DrawLineBetweenHandAndObject()
        {
            if (!settings.IsHolding()) return;

            Debug.DrawLine(
                Hand.position,
                Holder.position,
                Color.green
            );
        }

        public void RotateNextToAngle(Transform transform, float angle, float time)
        {
            var worldRotation = RemoveModule(transform.localEulerAngles, angle);
            var localRotation = Quaternion.Euler(worldRotation);

            rotateCoroutine = StartCoroutine(RotateRoutine(transform, localRotation, time));
        }

        public void Rotate(Vector2 input, Transform transform, float angle, float time)
        {
            CheckLastRotation(transform);

            var verticalRotation = Vector3.down * input.x;
            var horizontalRotation = Vector3.right * input.y;
            var worldAxis = verticalRotation + horizontalRotation;
            var worldAngle = worldAxis * angle;
            var worldRotation = RemoveModule(worldAngle, angle);
            var localRotation = Quaternion.Euler(worldRotation) * transform.localRotation;

            rotateCoroutine = StartCoroutine(RotateRoutine(transform, localRotation, time));
        }

        public void ResetRotation(Transform transform, float time) =>
            rotateCoroutine = StartCoroutine(RotateRoutine(transform, to: Quaternion.identity, time));

        private void CheckLastRotation(Transform transform)
        {
            if (rotateCoroutine != null)
            {
                StopCoroutine(rotateCoroutine);
                transform.localRotation = desiredRotation;
            }
        }

        private static IEnumerator RotateRoutine(Transform transform, Quaternion to, float time)
        {
            var t = 0f;
            var speed = 1F / time;
            var from = transform.localRotation;

            desiredRotation = to;

            do
            {
                transform.localRotation = Quaternion.Slerp(from, to, t);
                t += Time.deltaTime * speed;

                yield return waitOneFrame;
            } while (t < 1f);

            transform.localRotation = to;
            rotateCoroutine = null;
        }

        private static Vector3 RemoveModule(Vector3 value, float module)
        {
            return new(
                RemoveModule(value.x, module),
                RemoveModule(value.y, module),
                RemoveModule(value.z, module)
            );
        }

        private static float RemoveModule(float value, float module)
        {
            var over = value % module;
            return value - over;
        }
    }
}