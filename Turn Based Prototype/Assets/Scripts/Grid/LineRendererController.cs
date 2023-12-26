using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    
    public void Draw(Vector3 start, Vector3 end)
    {
        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
        }
        
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
        
        _lineRenderer.enabled = true;
    }
}