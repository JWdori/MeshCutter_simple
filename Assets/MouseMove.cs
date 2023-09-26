using UnityEngine;

public class MouseMove : MonoBehaviour
{
    private bool isMoving = false;  // 오브젝트가 움직이고 있는지 확인
    private Vector3 offset;  // 클릭한 위치와 오브젝트의 중심 사이의 오프셋

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 레이캐스팅을 사용하여 마우스 위치에서의 클릭을 감지
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)  // 나이프 오브젝트가 클릭되었는지 확인
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
            // 현재 마우스 위치를 월드 좌표로 변환
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            gameObject.transform.position = mousePosition + offset;
        }
    }
}
