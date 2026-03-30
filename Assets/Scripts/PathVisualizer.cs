using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    private LineRenderer totalPathRenderer;
    private LineRenderer segmentRenderer;

    private void Awake()
    {
        totalPathRenderer = CreateLineRenderer("TotalPath", new Color(0.2f, 0.6f, 1f, 0.6f), 0.5f);
        segmentRenderer = CreateLineRenderer("SegmentPath", new Color(0.2f, 0.6f, 1f, 0.6f), 0.5f);
    }

    private LineRenderer CreateLineRenderer(string name, Color color, float width)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;

        var lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        lr.material.color = color;
        lr.startColor = color;
        lr.endColor = color;
        lr.widthMultiplier = width;
        lr.useWorldSpace = true;
        lr.textureMode = LineTextureMode.Tile;
        lr.widthCurve = AnimationCurve.EaseInOut(0, width, 1, width);
        lr.sortingOrder = 10;
        lr.alignment = LineAlignment.TransformZ;
        lr.transform.forward = Vector3.up;
        return lr;
    }

    public void DrawPath(List<Tile> path)
    {
        if (path == null || path.Count == 0)
        {
            totalPathRenderer.positionCount = 0;
            return;
        }

        totalPathRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pos = path[i].transform.position + Vector3.up * 0.51f;
            totalPathRenderer.SetPosition(i, pos);
        }
    }

    public void DrawSegment(Vector3 from, Vector3 to)
    {
        segmentRenderer.positionCount = 2;
        segmentRenderer.SetPosition(0, from);
        segmentRenderer.SetPosition(1, to);
    }

    public void ClearPath()
    {
        totalPathRenderer.positionCount = 0;
        segmentRenderer.positionCount = 0;
    }

    public void SetColors(Color color)
    {
        if (totalPathRenderer != null)
        {
            totalPathRenderer.startColor = color;
            totalPathRenderer.endColor = color;
            totalPathRenderer.material.color = color;
        }

        if (segmentRenderer != null)
        {
            segmentRenderer.startColor = color;
            segmentRenderer.endColor = color;
            segmentRenderer.material.color = color;
        }
    }
}
