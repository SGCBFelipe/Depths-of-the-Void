using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Tooltip("Se true, só gira no eixo Y (bom pra objetos que ficam 'em pé', como personagens). Se false, olha direto pra câmera em qualquer ângulo (bom pra partículas/peixes flutuando).")]
    [SerializeField] private bool travarEixoY = false;

    private Transform câmera;

    void Start()
    {
        câmera = Camera.main.transform;
    }

    // LateUpdate garante que a câmera já terminou de se mover nesse frame
    void LateUpdate()
    {
        if (câmera == null) return;

        if (travarEixoY)
        {
            Vector3 direcao = câmera.position - transform.position;
            direcao.y = 0f;

            if (direcao.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(-direcao);
        }
        else
        {
            transform.rotation = câmera.rotation;
        }
    }
}