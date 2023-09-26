using UnityEngine;

public class MouseMove : MonoBehaviour
{
    private bool isMoving = false;  // ������Ʈ�� �����̰� �ִ��� Ȯ��
    private Vector3 offset;  // Ŭ���� ��ġ�� ������Ʈ�� �߽� ������ ������

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ����ĳ������ ����Ͽ� ���콺 ��ġ������ Ŭ���� ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)  // ������ ������Ʈ�� Ŭ���Ǿ����� Ȯ��
                {
                    isMoving = true;
                    offset = gameObject.transform.position - hit.point;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
        }

        if (isMoving)
        {
            // ���� ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            gameObject.transform.position = mousePosition + offset;
        }
    }
}
