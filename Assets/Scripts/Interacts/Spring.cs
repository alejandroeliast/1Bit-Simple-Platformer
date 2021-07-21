using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour, IInteractable
{
    
    [SerializeField] private float _force;

    private bool _isActive = true;
    private float _zRot;

    private void Awake()
    {
        _zRot = transform.rotation.eulerAngles.z;
    }

    public void Push(Rigidbody2D rb)
    {
        if (_isActive)
        {
            rb.velocity = Vector2.zero;

            if (_zRot == 0)
                rb.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
            else if (_zRot == Mathf.Abs(180))
                rb.AddForce(Vector2.down * _force, ForceMode2D.Impulse);
            else if (_zRot == 90)
                rb.AddForce(Vector2.left * _force, ForceMode2D.Impulse);
            else if(_zRot == 270)
                rb.AddForce(Vector2.right * _force, ForceMode2D.Impulse);

            if (_zRot == 90 || _zRot == 270)
                _isActive = false;
        }
    }

    public void Interact(GameObject interact)
    {
        Rigidbody2D rb = interact.GetComponent<Rigidbody2D>();
        PlayerController controller = interact.GetComponent<PlayerController>();

        if (rb != null)
        {
            Push(rb);
            if (controller != null)
            {
                controller.StartCoroutine(controller.DisableHitSpring());
            }
        }
    }
}
