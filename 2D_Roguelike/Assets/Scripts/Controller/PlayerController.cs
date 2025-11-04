using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingleTon<PlayerController>
{
    [Header("플레이어 세팅")]
    public Character character;
    public float hp;
    public float mp;
    public float speed;
    public float exp;
    public int level;

    [SerializeField] private Character[] characterList;
    public Vector3 targetPoint;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPoint = transform.position;
    }

    void Update()
    {
        // ========= 임시 코드
        KeyCode key = KeyCode.None;
        if (Input.GetKeyDown(KeyCode.Alpha1)) key = KeyCode.Alpha1;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) key = KeyCode.Alpha2;

        switch (key)
        {
            case KeyCode.Alpha1:
                character = characterList[0];
                PlayerInit();
                break;

            case KeyCode.Alpha2:
                character = characterList[1];
                PlayerInit();
                break;
        }
        // =========



        if (Input.GetMouseButtonDown(1))
        {
            targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPoint.z = transform.position.z;

            if (transform.position.x < targetPoint.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
    }
    
    void PlayerInit()
    {
        spriteRenderer.sprite = character.sprite;
        hp = character.hp;
        mp = character.mp;
        speed = character.speed;
        exp = character.exp;
        level = character.level;
    }
}
