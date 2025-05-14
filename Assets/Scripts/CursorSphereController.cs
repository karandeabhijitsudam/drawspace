using UnityEngine;

public class CursorSphereController : MonoBehaviour
{
    private Camera mainCam;
    private Renderer sphereRenderer;

    private void Start()
    {
        mainCam = Camera.main;
        sphereRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    public void SetColor(Color color)
    {
        if (sphereRenderer != null)
            sphereRenderer.material.color = color;
    }

    public void SetSize(float size)
    {
        transform.localScale = Vector3.one * size;
    }

    public void ToggleVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
