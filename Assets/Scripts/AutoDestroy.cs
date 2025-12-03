using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    
    public float extraDelay = 0f;

    void Start()
    {
        
        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            
            
            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;

            
            Destroy(gameObject, animLength + extraDelay);
        }
        else
        {
            
            
            Destroy(gameObject, 1.0f + extraDelay);
        }
    }
}