using UnityEngine;
using System.Collections;

public class MeshCutter : MonoBehaviour
{
    public GameObject KnifeObject;

    private Mesh mesh;
    private Vector3[] initialVertices;
    private MeshCollider meshCollider;
    private Vector3 previousKnifePosition;

    private Vector3 lastValidDirection = Vector3.zero; // Last known direction
    private bool hasContactedMesh = false;  // Tracking if the mesh is in contact
    private float lastContactTime = 0f;  // Tracking the last contact time
    private bool isUpdatingDirection = false;  // To check if a new direction is being updated

    private Vector3 highestYVertex;
    private Vector3 lowestYVertex;
    private Vector3 highestXVertex;
    private Vector3 lowestXVertex;
    private Vector3 highestZVertex;
    private Vector3 lowestZVertex;



    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        initialVertices = mesh.vertices;
        meshCollider = GetComponent<MeshCollider>();
        if (!meshCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = mesh;

        if (KnifeObject)
        {
            previousKnifePosition = KnifeObject.transform.position;
        }
    }




    void Update()
    {
        if (KnifeObject == null) return;

        Collider knifeCollider = KnifeObject.GetComponent<Collider>();
        if (!knifeCollider) return;

        Vector3 direction = (KnifeObject.transform.position - previousKnifePosition).normalized;

        if (direction != Vector3.zero)
        {
            if (!hasContactedMesh || (hasContactedMesh && Time.time - lastContactTime > 1f))
            {
                //StartCoroutine(UpdateDirectionAfterDelay(direction, 0.1f));
                lastValidDirection = direction;
            }
            else if (hasContactedMesh && !isUpdatingDirection)
            {
                StartCoroutine(UpdateDirectionAfterDelay(direction, 2.0f));
            }
        }

        if (meshCollider.bounds.Intersects(knifeCollider.bounds))
        {
            hasContactedMesh = true;  // Update the contact state
            lastContactTime = Time.time;  // Update the last contact time
            Vector3 knifePosition = KnifeObject.transform.position;
            Vector3[] meshVertices = mesh.vertices;

            // Initializing the highest and lowest y values
            highestYVertex = new Vector3(0, float.MinValue, 0);
            lowestYVertex = new Vector3(0, float.MaxValue, 0);
            highestXVertex = new Vector3(0, float.MinValue, 0);
            lowestXVertex = new Vector3(0, float.MaxValue, 0);
            highestZVertex = new Vector3(0, float.MinValue, 0);
            lowestZVertex = new Vector3(0, float.MaxValue, 0);
            // Finding the highest and lowest y values
            foreach (Vector3 vertex in meshVertices)
            {
                Vector3 worldVertex = transform.TransformPoint(vertex);
                if (worldVertex.y > highestYVertex.y)
                    highestYVertex = worldVertex;

                if (worldVertex.y < lowestYVertex.y)
                    lowestYVertex = worldVertex;
                if (worldVertex.x > highestYVertex.x)
                    highestXVertex = worldVertex;

                if (worldVertex.x < lowestYVertex.x)
                    lowestXVertex = worldVertex;
                if (worldVertex.z > highestYVertex.z)
                    highestZVertex = worldVertex;

                if (worldVertex.z < lowestYVertex.z)
                    lowestZVertex = worldVertex;
            }
            Debug.Log(lastValidDirection);
            for (int i = 0; i < meshVertices.Length; i++)
            {
                Vector3 worldVertex = transform.TransformPoint(meshVertices[i]);
                Vector3 initialWorldVertex = transform.TransformPoint(initialVertices[i]);

                // Y Axis: Moving from top to bottom
                if (worldVertex.y > knifePosition.y && lastValidDirection.y < 0)
                {
                    //worldVertex.y = knifePosition.y;
                    worldVertex.y = Mathf.Clamp(worldVertex.y, initialWorldVertex.y, knifePosition.y);
                    if (worldVertex.y < lowestYVertex.y)
                    {
                        Destroy(gameObject);
                    }
                }
                // Y Axis: Moving from bottom to top
                else if (worldVertex.y < knifePosition.y && lastValidDirection.y > 0)
                {
                    worldVertex.y = Mathf.Clamp(worldVertex.y, knifePosition.y, initialWorldVertex.y);
                    if (worldVertex.y > highestYVertex.y)
                    {
                        Destroy(gameObject);
                    }
                }

                // X Axis
                if (worldVertex.x > knifePosition.x && lastValidDirection.x < 0)
                {
                    worldVertex.x = Mathf.Clamp(worldVertex.x, initialWorldVertex.x, knifePosition.x);
                    if (worldVertex.x < lowestXVertex.x)
                    {
                        Destroy(gameObject);
                    }
                }
                else if (worldVertex.x < knifePosition.x && lastValidDirection.x > 0)
                {
                    worldVertex.x = Mathf.Clamp(worldVertex.x, knifePosition.x, initialWorldVertex.x);
                    if (worldVertex.x > highestXVertex.x)
                    {
                        Destroy(gameObject);
                    }
                }

                // Z Axis
                if (worldVertex.z > knifePosition.z && lastValidDirection.z < 0)
                {
                    worldVertex.z = Mathf.Clamp(worldVertex.z, initialWorldVertex.z, knifePosition.z);
                    if (worldVertex.z < lowestZVertex.z)
                    {
                        Destroy(gameObject);
                    }
                }
                else if (worldVertex.z < knifePosition.z && lastValidDirection.z > 0)
                {
                    worldVertex.z = Mathf.Clamp(worldVertex.z, knifePosition.z, initialWorldVertex.z);
                    if (worldVertex.z > highestZVertex.z)
                    {
                        Destroy(gameObject);
                    }
                }

                meshVertices[i] = transform.InverseTransformPoint(worldVertex);
            }
            mesh.vertices = meshVertices;
            mesh.RecalculateBounds();
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
        else
        {
            hasContactedMesh = false;  // Reset contact state
        }

        previousKnifePosition = KnifeObject.transform.position;
    }

    IEnumerator UpdateDirectionAfterDelay(Vector3 newDirection, float delay)
    {

        isUpdatingDirection = true;  // Start updating
        yield return new WaitForSeconds(delay);  // Wait for 2 seconds
        lastValidDirection = newDirection;  // Update direction value
        isUpdatingDirection = false;  // Updating complete
    
    }

}
