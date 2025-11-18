using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-100)]// 얘가 젤 먼저 실행되야함
public class PlayerManager : SingleTon<PlayerManager> 
{
    [Header("캐릭터 목록")]
    [SerializeField] private Character[] _characterList;

    [Header("Controller")]
    public PlayerController playerController;
    public Aim aim;

    [Header("Model")]
    public GameObject player;
    public BattleSystem battleSystem;
    public LevelSystem levelSystem; 
    public PlayerStat playerStat;
    public StatCalculator statCalculator;
    public Drain drain;

    
    [Header("컴퍼넌트")]
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;


    [Header("현재 선택된 캐릭터")]
    public Character character; // 얘가 중요한거임 일단.

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // 2. 그 객체의 컴퍼넌트를 담는다.
        playerController = player.GetComponent<PlayerController>();
        aim = player.GetComponent<Aim>();

        battleSystem = player.GetComponent<BattleSystem>();
        levelSystem = player.GetComponent<LevelSystem>();
        playerStat = player.GetComponent<PlayerStat>();
        statCalculator = player.GetComponent<StatCalculator>();
        drain = player.GetComponentInChildren<Drain>();
        
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        rb = player.GetComponent<Rigidbody2D>();
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
                character = _characterList[0];
                InitPlayer(character);
                break;

            case KeyCode.Alpha2:
                character = _characterList[1];
                InitPlayer(character);
                break;
        }
        // =========
    }

    public void InitPlayer(Character character)
    {
        spriteRenderer.sprite = character.sprite; // 캐릭터 이미지 세팅하고

        SkillRuntimeView.Instance.SetSkill(character); // 스킬 세팅
        statCalculator.DefaultCalculate(); // 기본 스펙 세팅
        battleSystem.aaPool.SetAAPool(character);  // aapool 만들어서 총알장전
        playerController.OnApplyVital(playerStat.stat); // 최초 체력이랑 마나 세팅
        character.weapon.InitWeapon(player);
        StartCoroutine(character.weapon.WeaponController(player));
    }
}
