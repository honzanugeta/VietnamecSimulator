using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationscntrolller : MonoBehaviour
{
   private Animator animotor;

    public CharacterController controler;
// Start is called before the first frame update
    void Start()
    {
        animotor = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
