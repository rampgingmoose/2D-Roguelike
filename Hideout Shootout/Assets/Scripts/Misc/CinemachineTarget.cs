using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    #region Tooltip
    [Tooltip("Populate with the CursorTarget gameobject")]
    #endregion
    [SerializeField] private Transform cursorTarget;
    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCineMachineTragetGroup();
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }

    private void SetCineMachineTragetGroup()
    {
        //Create target group for cinemachine camera to follow - this group will include the player and the screen cursor
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target { weight = 1f, radius = 2.5f, 
            target = GameManager.Instance.GetPlayer().transform};

        CinemachineTargetGroup.Target cinmachineGroupTarget_cursor = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f,
            target = cursorTarget};

        CinemachineTargetGroup.Target[] cinemachineTargetGroupArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player,
            cinmachineGroupTarget_cursor};
        
        cinemachineTargetGroup.m_Targets = cinemachineTargetGroupArray;
    }
}
