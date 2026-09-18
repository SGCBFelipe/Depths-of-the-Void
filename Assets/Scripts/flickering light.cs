using UnityEngine;


public class FlickeringLight : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Light _light;

    [Header("Settings")]
    [SerializeField] private float _minIntensity = 0.2f;
    [SerializeField] private float _maxIntensity = 1.0f;
    [SerializeField] private float _flickerSpeed = 0.1f;

    private float _targetIntensity;
    private float _timer;

    private void Start()
    {
        if (_light == null)
        {
            _light = GetComponent<Light>();
        }

        // Define a intensidade inicial de destino
        _targetIntensity = Random.Range(_minIntensity, _maxIntensity);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        // Suplica/interpola a intensidade atual até o valor alvo
        _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, Time.deltaTime * 10f);

        // Quando o tempo limite for atingido, escolhe uma nova intensidade aleatória
        if (_timer >= _flickerSpeed)
        {
            _targetIntensity = Random.Range(_minIntensity, _maxIntensity);
            _timer = 0f;
        }
    }
}
