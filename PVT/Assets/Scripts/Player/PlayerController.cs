using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Text retryText;
    
    public enum KeyInput {
        GoLeft = 0,
        GoRight,
        GoDown,
        GoUp,
        Shoot,
        Restart,
        Count // should be last :>
    }

    public enum CharacterState {
        Normal,
        Dead,
        JustRespawned,
        FinalDead,
        Count
    };


    private CharacterState charState = CharacterState.Normal;

    public bool[] mInputs;
    public bool[] mPrevInputs;

    private float speed = 15.0f;

    private float currentShootingCooldown = 0.0f;
    private float shootingCooldown = 0.08f;
    private Vector3 bulletSpeed = new Vector3(0.0f, 0.20f, 0.0f);

    private float currentDeadTime = 0.0f;
    private float deadTime = 1.0f;


    private float currentJustRespawnedTime = 0.0f;
    private float justRespawnedTime = 2.0f;

    public float currentBlinkTime = 0.0f;
    private float blinkGap = 0.15f;
    private bool isBlinked = true;

    private int lives = 3;


    private Vector3 deadOffset = new Vector3(3000, 3000, 3000);

    private MeshRenderer renderer; 


    // Start is called before the first frame update
    void Start()
    {
        mInputs = new bool[(int)KeyInput.Count];
        mPrevInputs = new bool[(int)KeyInput.Count];

        renderer = GetComponent<MeshRenderer>();
        //retryText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentInputs();

        HandleStated();

        MovePlayerObject();
        HandleShootings();

        HandleRestart();

        UpdatePrevInputs();
    }

    private void HandleRestart()
    {
        if(charState == CharacterState.FinalDead)
        {
            if (Pressed(KeyInput.Restart)){
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void HandleStated()
    {
        // logic
        switch (charState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Dead:
                if (currentDeadTime < 0.01f) {
                    currentDeadTime = 0.0f;
                    currentJustRespawnedTime = justRespawnedTime;
                    charState = CharacterState.JustRespawned;
                    transform.position += deadOffset;
                }
                currentDeadTime -= Time.deltaTime;
                break;
            case CharacterState.JustRespawned:
                if (currentJustRespawnedTime < 0.01f)
                {
                    currentJustRespawnedTime = 0.0f;
                    charState = CharacterState.Normal;
                    renderer.enabled = true;
                }
                currentJustRespawnedTime -= Time.deltaTime;
                break;
        }

        // visuals
        switch (charState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Dead:
                break;
            case CharacterState.JustRespawned:

                currentBlinkTime -= Time.deltaTime;

                if(currentBlinkTime < 0.01f)
                {
                    currentBlinkTime = blinkGap;
                    isBlinked = !isBlinked;
                }

                if (isBlinked)
                {
                    renderer.enabled = false;
                }
                else {
                    renderer.enabled = true;
                }
                break;
        }
    }



    public void DamageMe() {
        if (charState != CharacterState.Normal) {
            return;
        }
        charState = CharacterState.Dead;
        currentDeadTime = deadTime;
        transform.position -= deadOffset;
        lives -= 1;

        if (lives == 0) {
            retryText.enabled = true;
            charState = CharacterState.FinalDead;
        }
    }



    private void HandleShootings()
    {
        if (charState == CharacterState.Dead || charState == CharacterState.FinalDead)
        {
            return;
        }

        currentShootingCooldown -= Time.deltaTime;

        if (currentShootingCooldown < 0.01f && KeyState(KeyInput.Shoot)) {
            currentShootingCooldown = shootingCooldown;

            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
            bulletBehaviour.speed = bulletSpeed;

            // sidebullets

            for (int i = 1; i < 2; i++) {
                var bullet2 = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                var bulletBehaviour2 = bullet2.GetComponent<BulletBehaviour>();
                bulletBehaviour2.speed = Quaternion.Euler(0, 0, 6 * i) * bulletSpeed;

                var bullet3 = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                var bulletBehaviour3 = bullet3.GetComponent<BulletBehaviour>();
                bulletBehaviour3.speed = Quaternion.Euler(0, 0, -6 * i) * bulletSpeed;
            }

        }

    }

    private void MovePlayerObject()
    {
        Vector3 movement = Vector3.zero;

        if (!(charState == CharacterState.Dead || charState == CharacterState.FinalDead))
        {
            movement = new Vector3(KeyStateAsInt(KeyInput.GoRight) - KeyStateAsInt(KeyInput.GoLeft), KeyStateAsInt(KeyInput.GoUp) - KeyStateAsInt(KeyInput.GoDown), 0.0f);
        }

        GetComponent<Rigidbody>().velocity = movement * speed;
    //    GetComponent<Rigidbody>().position += movement * speed;



    }

    private void GetCurrentInputs()
    {
        mInputs[(int)KeyInput.GoRight] = Input.GetKey(KeyCode.RightArrow);
        mInputs[(int)KeyInput.GoLeft] = Input.GetKey(KeyCode.LeftArrow);
        mInputs[(int)KeyInput.GoDown] = Input.GetKey(KeyCode.DownArrow);
        mInputs[(int)KeyInput.GoUp] = Input.GetKey(KeyCode.UpArrow);
        mInputs[(int)KeyInput.Shoot] = Input.GetKey(KeyCode.LeftControl);
        mInputs[(int)KeyInput.Restart] = Input.GetKey(KeyCode.Space);
    }

    void UpdatePrevInputs() {
        var count = (byte)KeyInput.Count;

        for (byte i = 0; i < count; ++i)
            mPrevInputs[i] = mInputs[i];
    }

    bool Released(KeyInput key) {
        return (!mInputs[(int)key] && mPrevInputs[(int)key]);
    }

    bool KeyState(KeyInput key) {
        return (mInputs[(int)key]);
    }

    bool Pressed(KeyInput key) {
        return (mInputs[(int)key] && !mPrevInputs[(int)key]);
    }


    int ReleasedAsInt(KeyInput key)
    {
        return Released(key) ? 1 : 0;
    }

    int KeyStateAsInt(KeyInput key)
    {
        return KeyState(key) ? 1 : 0;
    }

    int PressedAsInt(KeyInput key)
    {
        return Pressed(key) ? 1 : 0;
    }


}
