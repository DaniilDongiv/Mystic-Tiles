using UnityEngine;

public class AxisAligner : MonoBehaviour
{
    public int GridX { get; private set; }
    public int GridY { get; private set; }
    public string SquareTag { get; private set; }
    
    private const float _squareSpacing = 0.23f;
    private const float _offsetX = 0.0733539388f;
    private const float _offsetY = 0.0605170019f;

    private void Start()
    {
        SquareTag = gameObject.tag;
    }
    
    public void SetupTile(int x, int y)
    {
        GridX = x;
        GridY = y;
    }
    
    public void ShiftToCoordinates(int x, int y)
    {
        GridX = x;
        GridY = y;

        var newPosition = new Vector3(
            x * (_offsetX + _squareSpacing),
            y * (_offsetY + _squareSpacing),
            0);

        transform.position = newPosition;
    }
    
    private void OnMouseDown()
    {
        QuadraxPilot quadrantPilot = GetComponentInParent<QuadraxPilot>();
        if (quadrantPilot != null)
        {
            quadrantPilot.ProcessTileSelection(this);
        }
    }
}