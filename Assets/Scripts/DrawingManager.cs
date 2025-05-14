using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.EventSystems; // Add this at the top of your file


public class DrawingManager : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.1f;

    private LineRenderer currentLine;
    private List<Vector3> points = new List<Vector3>();
    private List<GameObject> drawnLines = new List<GameObject>();
    public CursorSphereController cursorSphere;
    private Color currentColor = Color.black;
    private float brushSize = 0.1f; // also used for line width

    private bool is3DMode = false;
    public bool isDrawingEnabled = true;

    public TMP_Dropdown colorDropdown;
    public TMP_Dropdown sizeDropdown;

    void Update()
    {
        if (!isDrawingEnabled) return;

        // Handle color & size update
        cursorSphere.SetColor(currentColor);
        cursorSphere.SetSize(lineWidth);

        // Prevent drawing when clicking on UI
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Keep on 2D plane

            //if (Vector3.Distance(mousePos, points[points.Count - 1]) > 0.1f)
            if (points.Count == 0 || Vector3.Distance(mousePos, points[points.Count - 1]) > 0.1f)
            {
                AddPoint(mousePos);
            }
        }
    }

    void CreateLine()
    {
        GameObject lineObj = new GameObject("Line");
        currentLine = lineObj.AddComponent<LineRenderer>();
        currentLine.material = lineMaterial;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", currentColor);
        currentLine.SetPropertyBlock(mpb);



        currentLine.widthMultiplier = lineWidth;
        currentLine.positionCount = 0;
        currentLine.useWorldSpace = true;
        currentLine.numCapVertices = 5;
        currentLine.tag = "Line";
        


        points.Clear();
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPoint.z = 0;
        AddPoint(startPoint);
        drawnLines.Add(lineObj);
    }

    void AddPoint(Vector3 point)
    {
        points.Add(point);
        currentLine.positionCount = points.Count;
        currentLine.SetPosition(points.Count - 1, point);
    }

    public void SetMode2D()
    {
        is3DMode = false;
        // Later: Disable 3D scripts or switch scenes
    }

    public void SetMode3D()
    {
        is3DMode = true;
        // Later: Enable 3D drawing logic
    }

    public void OnColorDropdownChanged(TMP_Dropdown dropdown)
    {
        string selectedColor = dropdown.options[dropdown.value].text.ToLower();

        switch (selectedColor)
        {
            case "black":
                SetColor(Color.black);
                break;
            case "red":
                SetColor(Color.red);
                break;
            case "blue":
                SetColor(Color.blue);
                break;
            case "green":
                SetColor(Color.green);
                break;
            default:
                SetColor(Color.black);
                break;
        }
    }

    public void SetColor(Color newColor)
    {
        currentColor = newColor;
        lineMaterial.color = newColor;
    }

    public void OnSizeDropdownChanged(TMP_Dropdown dropdown)
    {
        string selectedColor = dropdown.options[dropdown.value].text.ToLower();

        switch (selectedColor)
        {
            case "size 1":
                SetBrushSize(0.1f);
                break;
            case "size 2":
                SetBrushSize(0.25f);
                break;
            case "size 3":
                SetBrushSize(0.5f);
                break;
            case "size 4":
                SetBrushSize(0.75f);
                break;
            default:
                SetBrushSize(0.1f);
                break;
        }

        // Also update lineWidth so it's used in CreateLine
        lineWidth = brushSize;
        cursorSphere.SetSize(lineWidth);
    }

    public void SetBrushSize(float newSize)
    {
        brushSize = newSize;
    }
   

    public void UndoCanvas()
    {
        if (drawnLines.Count > 0)
        {
            isDrawingEnabled = false;

            GameObject lastLine = drawnLines[drawnLines.Count - 1];
            drawnLines.RemoveAt(drawnLines.Count - 1);
            Destroy(lastLine);
            Debug.Log($"Lines remaining: {drawnLines.Count}");

            StartCoroutine(EnableDrawingNextFrame());
        }
    }

    public void ClearCanvas()
    {
        isDrawingEnabled = false; // Disable drawing just before clearing
        foreach (var line in GameObject.FindGameObjectsWithTag("Line"))
        {
            Destroy(line);
        }
        StartCoroutine(EnableDrawingNextFrame());
    }

    private IEnumerator EnableDrawingNextFrame()
    {
        yield return null; // Wait 1 frame
        isDrawingEnabled = true;
    }


}
