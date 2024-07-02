using UnityEngine;

public class UpDownJiggle : MonoBehaviour
{
    public float amplitude = 0.5f; // How far to jiggle up and down
    public float frequency = 1f;   // How fast to jiggle

    private Vector3 startPos;

    void Start() => startPos = transform.position;

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0, y, 0);
    }
}