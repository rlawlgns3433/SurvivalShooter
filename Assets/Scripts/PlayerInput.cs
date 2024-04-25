using UnityEngine;

// �÷��̾� ĳ���͸� �����ϱ� ���� ����� �Է��� ����
// ������ �Է°��� �ٸ� ������Ʈ���� ����� �� �ֵ��� ����
public class PlayerInput : MonoBehaviour
{
    public static readonly string moveXAxisName = "Horizontal";
    public static readonly string moveZAxisName = "Vertical";
    public static readonly string fireButtonName = "Fire1";

    // �� �Ҵ��� ���ο����� ����
    public float moveX { get; private set; } // ������ ������ �Է°�
    public float moveZ { get; private set; } // ������ ȸ�� �Է°�
    public bool fire { get; private set; } // ������ �߻� �Է°�

    // �������� ����� �Է��� ����
    private void Update()
    {
        // ���ӿ��� ���¿����� ����� �Է��� �������� �ʴ´�
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            moveX = 0;
            moveZ = 0;
            fire = false;
            return;
        }

        // move�� ���� �Է� ����
        moveX = Input.GetAxis(moveXAxisName);
        // rotate�� ���� �Է� ����
        moveZ = Input.GetAxis(moveZAxisName);
        // fire�� ���� �Է� ����
        fire = Input.GetButton(fireButtonName);
        // reload�� ���� �Է� ����
    }
}