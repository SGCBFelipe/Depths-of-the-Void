using UnityEngine;
using static UnityEditor.PlayerSettings;

public class HeadbobSystem : MonoBehaviour
{
    [Range(0.001f, 0.01f)]
    public float amount = 0.002f;
    [Range(1f, 30f)]

    public float frequency = 10f;

    [Range(10f, 100f)]
    public float Smooth = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        checkForHeadbobTrigger();
    }

    private void checkForHeadbobTrigger()
    {
        float InputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;

        if (InputMagnitude > 0) 
        {
            StartHeadBob();
        }
    }

    private void StartHeadBob()
    {

        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, Smooth * Time.deltaTime );
        pos.x = Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f ,Smooth * Time.deltaTime );
        transform.localPosition += pos;

        return pos;
    }
}
