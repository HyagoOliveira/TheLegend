using TheLegend.Players;

namespace Cinemachine
{
    public static class CinemachineExtension
    {
        public static void SetTarget(this CinemachineFreeLook virtualCamera, Player player)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.CameraPivot;
        }

        public static void Replace(
            this CinemachineVirtualCameraBase from,
            CinemachineVirtualCameraBase to
        )
        {
            if (from == to)
            {
                to.gameObject.SetActive(true);
                return;
            }

            to.transform.SetPositionAndRotation(
                from.transform.position,
                from.transform.rotation
            );

            to.gameObject.SetActive(true);
            from.gameObject.SetActive(false);
        }
    }
}
