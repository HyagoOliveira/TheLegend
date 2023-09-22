using UnityEngine;
using System.Collections;
using ActionCode.AwaitableCoroutines;
using System.Threading;

namespace TheLegend.Abilities
{
    /// <summary>
    /// This class contains all logic to rotate <see cref="IUltrahandable"/> objects.
    /// </summary>
    public static class UltrahandRotator
    {
        private static bool isRotating;
        private static Quaternion lastRotation;
        private static CancellationTokenSource cancelationSource = new();
        private static readonly WaitForEndOfFrame waitOneFrame = new();

        public static async void Rotate(Transform transform, float angle, float time)
        {
            StopLastRotation(transform);

            var worldRotation = RemoveOverModule(transform.localEulerAngles, angle);
            var localRotation = Quaternion.Euler(worldRotation);

            await AwaitableCoroutine.Run(Rotate(transform, localRotation, time));
        }

        public static async void Rotate(Vector2 input, Transform transform, float angle, float time)
        {
            StopLastRotation(transform);

            var verticalRotation = Vector3.down * input.x;
            var horizontalRotation = Vector3.right * input.y;
            var worldAxis = verticalRotation + horizontalRotation;
            var worldAngle = worldAxis * angle;
            var worldRotation = RemoveOverModule(worldAngle, angle);
            var localRotation = Quaternion.Euler(worldRotation) * transform.localRotation;

            await AwaitableCoroutine.Run(Rotate(transform, localRotation, time), cancelationSource.Token);
        }

        public static async void Reset(Transform transform, float time)
        {
            StopLastRotation(transform);
            await AwaitableCoroutine.Run(Rotate(transform, to: Quaternion.identity, time));
        }

        private static IEnumerator Rotate(Transform transform, Quaternion to, float time)
        {
            var t = 0f;
            var speed = 1F / time;
            var from = transform.localRotation;

            lastRotation = to;
            isRotating = true;

            do
            {
                transform.localRotation = Quaternion.Slerp(from, to, t);
                t += Time.deltaTime * speed;
                yield return waitOneFrame;

            } while (isRotating = t < 1f);

            transform.localRotation = to;
        }

        private static void StopLastRotation(Transform transform)
        {
            if (!isRotating) return;

            cancelationSource.Cancel();
            cancelationSource.Dispose();

            cancelationSource = new CancellationTokenSource();

            transform.localRotation = lastRotation;
        }

        private static Vector3 RemoveOverModule(Vector3 value, float module)
        {
            return new(
                RemoveOverModule(value.x, module),
                RemoveOverModule(value.y, module),
                RemoveOverModule(value.z, module)
            );
        }

        private static float RemoveOverModule(float value, float module)
        {
            var over = value % module;
            return value - over;
        }
    }
}