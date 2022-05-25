using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerSword : MonoBehaviour
{
    public AnimatorOverrideController purpleSword, goldenSword;

    public void PurpleSword()
    {
        GetComponent<Animator>().runtimeAnimatorController = purpleSword as RuntimeAnimatorController;
    }

    public void GoldenSword()
    {
        GetComponent<Animator>().runtimeAnimatorController = goldenSword as RuntimeAnimatorController;
    }
}
