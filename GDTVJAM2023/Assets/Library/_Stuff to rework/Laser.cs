using UnityEngine;

public class Laser : MonoBehaviour
{

    public int _maxBounce = 3;

    private int _count;
    private LineRenderer _laser;

    [SerializeField]
    private Vector3 _offSet;



    private void Start()
    {
        _laser = GetComponent<LineRenderer>();
    }



    private void Update()
    {
        _count = 0;
        castLaser(transform.position + _offSet, transform.up);
    }



    private void castLaser(Vector3 position, Vector3 direction)
    {
        _laser.SetPosition(0, transform.position + _offSet);

        for (int i = 0; i < _maxBounce; i++)
        {
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (_count < _maxBounce - 1)
                _count++;

            if (Physics.Raycast(ray, out hit, 300))
            {
                position = hit.point;
                direction = Vector3.Reflect(direction, hit.normal);
                _laser.SetPosition(_count, hit.point);

                if (hit.transform.tag != "Enemy")
                {
                    for (int j = (i + 1); j < _maxBounce; j++)
                    {
                        _laser.SetPosition(j, hit.point);
                    }
                    break;
                }
                else
                {
                    _laser.SetPosition(_count, hit.point);
                }
            }
            else
            {
                _laser.SetPosition(_count, ray.GetPoint(300));
            }
        }
    }
}