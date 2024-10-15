using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public enum AttackType { Automatic, Manual, Unload }
    public AttackType typeOfInput;

    public enum AmoType { none, HP, light, heavy, explosive }
    public AmoType typeOfAmo;

    [Tooltip("Time it takes to reload clip and ready next round of attacks")] public float cooldown = 0.1f;
    [Range(1, 100)] [Tooltip("Number of attacks before cooldown time")] public int clipSize = 1;
    [Tooltip("Time between attacks within the same clip")] public float fireRate;
    [Range(1,100)] [Tooltip("Spawn multiple copies of this attack each time")] public int multiplier = 1;
    [Tooltip("All additional attacks from multiplier are evenly spaced angles between min and max spread")] public bool spreadEvenly;
    [Tooltip("Launch attack with extra directional force based on current velocity")] public bool momentumLaunch = false;
    [Range(0,1)] [Tooltip("Aim speed percent while attacking")] public float attackingAimSpeed = 1;
    [Tooltip("Use pre and mid attack movements")] public bool moveWhileAttacking;

    [Header("Pre-Attack")]
    [Tooltip("Time it takes before attacking")] public float windup = 0.1f;
    [Tooltip("Make move before the attack, x is flipped if facing left")] public float movementPreAttack;
    [Tooltip("")] public bool onlyHorizontalMovePre = true; //otherwise move in direction aiming
    public bool noGravityDuringWindup;

    [Header("Mid-Attack")]
    [Tooltip("Duration of attack movement")] public float attackDuration = 0.1f;
    [Tooltip("Make move during the attack, x is flipped if facing left")] public float movementMidAttack;
    [Tooltip("")] public bool onlyHorizontalMoveMid = true; //otherwise move in direction aiming
    public bool noGravityDuringAttack;

    [Header("Spread Stats")]
    [Range(-89f, 89f)] [Tooltip("Degrees to change every attack relative to aim, pre spread")] public float startAimAngleOffset;
    [Tooltip("Amount of time it takes to reach max spread while attacking repeatedly, ignore other spread stats if 0")] public float spreadTime = 0;
    [Tooltip("Amount of degrees the porjectile can potentially be off from original aim angle as you fire for longer")] public AnimationCurve spreadCurve;
    [Tooltip("Multiplier for spread angle amount on curve")] public float spreadIntensity = 1f;
    [Range(0,1f)][Tooltip("Percent of the spread Curve that the bottom range follows, if 1 follows curve exactly")] public float spreadRange = 1f;
    [Tooltip("Every other shot switches sides if spread should be +/-")] public bool alternateNegative = true;
    [Tooltip("If more than 0, repeat spread pattern a number of times per clip instead of fire time")] public int repeatPattern = 0;

    public AnimationClip attackAnim;
    public Vector3 startOffset;

    [HideInInspector] public Weapon myWeapon;
    public void OnDisable()
    {
        if (myWeapon) { myWeapon.EndAttack(); }
    }


}
