using UnityEngine;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트들이 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour
{
    public static readonly string moveXAxisName = "Horizontal";
    public static readonly string moveZAxisName = "Vertical";
    public static readonly string fireButtonName = "Fire1";

    // 값 할당은 내부에서만 가능
    public float moveX { get; private set; } // 감지된 움직임 입력값
    public float moveZ { get; private set; } // 감지된 회전 입력값
    public bool fire { get; private set; } // 감지된 발사 입력값

    // 매프레임 사용자 입력을 감지
    private void Update()
    {
        // 게임오버 상태에서는 사용자 입력을 감지하지 않는다
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            moveX = 0;
            moveZ = 0;
            fire = false;
            return;
        }

        // move에 관한 입력 감지
        moveX = Input.GetAxis(moveXAxisName);
        // rotate에 관한 입력 감지
        moveZ = Input.GetAxis(moveZAxisName);
        // fire에 관한 입력 감지
        fire = Input.GetButton(fireButtonName);
        // reload에 관한 입력 감지
    }
}