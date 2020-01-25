using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject bulletPrefab;
    
    public enum KeyInput {
        GoLeft = 0,
        GoRight,
        GoDown,
        GoUp,
        Shoot,
        Count // should be last :>
    }

    public enum CharacterState {
        Normal
    };


    public bool[] mInputs;
    public bool[] mPrevInputs;

    private float speed = 15.0f;

    private float currentShootingCooldown = 0.0f;
    private float shootingCooldown = 0.01f;
    private Vector3 bulletSpeed = new Vector3(0.0f, 0.7f, 0.0f);


    // Start is called before the first frame update
    void Start()
    {
        mInputs = new bool[(int)KeyInput.Count];
        mPrevInputs = new bool[(int)KeyInput.Count];
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentInputs();

        MovePlayerObject();
        HandleShootings();

        UpdatePrevInputs();
    }

    private void HandleShootings()
    {
        currentShootingCooldown -= Time.deltaTime;

        if (currentShootingCooldown < 0.01f && KeyState(KeyInput.Shoot)) {
            currentShootingCooldown = shootingCooldown;

            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
            bulletBehaviour.speed = bulletSpeed;

            // sidebullets

            for (int i = 1; i < 4; i++) {
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
        Vector3 movement = new Vector3(KeyStateAsInt(KeyInput.GoRight) - KeyStateAsInt(KeyInput.GoLeft), KeyStateAsInt(KeyInput.GoUp) - KeyStateAsInt(KeyInput.GoDown), 0.0f);
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
