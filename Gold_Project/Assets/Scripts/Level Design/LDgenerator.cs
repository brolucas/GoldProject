using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDgenerator : MonoBehaviour
{
    [SerializeField] private List<int> _LargeurLongueurTuile;
    [SerializeField] private float _tailleTuile = 1f;
    public GameObject _Tuile;

    [ContextMenu("Generator")]
    void BackgroudGenerator()
    {
        for (int i = 0; i < _LargeurLongueurTuile[0]; i++)
        {
            for (int j = 0; j < _LargeurLongueurTuile[1]; j++)
            {
                Vector3 position = new Vector3(transform.position.x + _tailleTuile * i, transform.position.y + _tailleTuile * j, transform.position.z);
                GameObject background = Instantiate(_Tuile, position, Quaternion.Euler(0, 0, 0));
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction_droite = transform.TransformDirection(Vector3.right) * _LargeurLongueurTuile[0]; 
        Vector3 direction_bas = transform.TransformDirection(Vector3.right) * _LargeurLongueurTuile[0];


        Gizmos.DrawRay(transform.position, direction_droite);
        Gizmos.DrawRay(new Vector2(transform.position.x,transform.position.y - _tailleTuile*_LargeurLongueurTuile[1]), direction_droite);

    }
    
}
