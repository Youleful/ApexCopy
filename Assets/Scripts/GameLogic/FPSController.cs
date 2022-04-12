using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAction
{
    idle = 0,
    walk = 1,
    run = 2,
    slidingTakle = 3,
    crouch = 4,
    jump = 5,
    climb = 6,
    aim=7,
    useAbility=8,
    reloading=9,
    useHealthPackageOrBattery=10,
    shoot
}
public class FPSController : MonoBehaviour
{
    // Start is called before the first frame updat
    Rigidbody rigid;
    public float speed = 0;

    public float walk_speed = 5.0f;//移动速度

    private Vector3 moveDirection = Vector3.zero;//角色的方向

    public float run_speed = 10.0f;//跑步速度


    public float aim_speed = 2.0f;//开镜时移速
    public float crouch_speed = 1.0f;//下蹲速度
    //物体
    Transform m_transform;

    //当前位置
    Transform m_curTransform;
    //摩擦力
    [SerializeField]
    float friction = 0.2f;

    //相机
    public Transform m_camTransform;

    //跳跃高度
    [SerializeField]
    float jumpHeight = 0.0f;

    //当前物体的高度记录（用来和下一帧的高度进行对比）
    float y1 = 0.0f;

    [SerializeField]
    //下滑加速度
    float a_speedOfSlope = 0.0f;
    //最大下滑速度
    [SerializeField]
    float maxSpeedOfSlope = 15.0f;

    //摄像机动画组
    Animator ani_camera;

    //枪械动画
    //相机的旋转
    Vector3 m_camRot;

    bool isGround = false;


    //背包是否打开
    bool isBagOpen = false;

    bool isSlidingTakle = false;
    float moveScale = 0;

    public bool main_active = false;

    //武器模型实例化(主/副)

    //鼠标指向的射线
    Ray ray;
    //碰撞体信息
    RaycastHit hitInfo;
    public Transform bullet;

    GunItem gunItem;
    public Gun gun;
    //人物状态
    PlayerAction playerAction;
    //能拾取的物体
    Item item;
    public PlayerState playerState;

    //玩家武器池
    PlayerGunPool playerGunPool;
    //武器UI委托
   delegate void WeaponUIController();
   event WeaponUIController weaponUIcontroller;
    //玩家状态委托
    delegate void PlayerStateUIController();
    event PlayerStateUIController playerStateUIcontroller;
    //背包UI委托
    delegate void BagUIController();
    event BagUIController bagUIcontroller;

    bool isMain;
    GameObject BagSystem;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    

    private void Start()
    {
        playerState = ScriptableObject.CreateInstance<PlayerState>();
        playerState.Q_CD = 10;
        playerState.Z_CD = 10;
        playerState.maxLife = 100;
        playerState.life = 100;
        playerState.bag = new Bag();
        //BagSystem = GameObject.Find("BagSystem");
        //BagSystem.SetActive(false);
        m_transform = this.transform;
        m_camTransform = Camera.main.transform;
        m_camRot = m_camTransform.eulerAngles;
        m_camTransform.rotation = m_transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        ani_camera = Camera.main.GetComponent<Animator>();
        playerGunPool = GameObject.Find("Gun").GetComponent<PlayerGunPool>();
        WeaponStateManager.weaponStateManager_Instance.SetBag(playerState.bag);
        BagSystem = GameObject.Find("BagSystem");
        BagSystem.SetActive(false);
        playerStateUIcontroller += new PlayerStateUIController(PlayerStateManager.playerStateManager_instance.SetStateUI);
        BagManager.Instance.SetBag(playerState.bag);
        isMain = false;
    }

    private void Update()
    {
        GetState();
        GetDirection();
        UseQ();
        UseZ();
        //UseBag();
        //判断当前是哪把
        ChangeGun();
        PickUp();
        Shoot();
        UseBag();
        playerStateUIcontroller();
        //水平移动
        Vector3 v = Vector3.Project(rigid.velocity, transform.forward);
        float s = speed == 0 ? 0 : 1 - v.magnitude / speed;
        //rigid.AddForce(transform.forward * moveScale * s, ForceMode.VelocityChange);//第一种：力驱动，好处：比较真实具体移动交给物理系统；缺点：不好控制，容易滑动
        //rigid.velocity = transform.forward * 10 * moveScale + Vector3.up * rigid.velocity.y;//第二种：速度设置，好处：直接设置速度；缺点：直接干涉物理速度，抖动失真
        //if (moveScale == 1) rigid.MovePosition((rigid.position + transform.forward * moveSpeed * Time.deltaTime));//第三种：直接物理位置，好处：直接移动，缺点：目前描述不出来
    }

    private void FixedUpdate()
    {
        if (moveScale == 1)
            rigid.MovePosition((rigid.position + moveDirection * speed * Time.fixedDeltaTime));
        y1 = rigid.position.y;
    }

    //private void OnDestroy()
    //{
    //    BagManager.Instance.mainItem.cabinList.Clear();
    //}
    bool isSlope()
    {
        return y1 > rigid.position.y;
    }

    void GetState()
    {
        if (speed < 0)
            speed = 0;
        if (isSlope())
            a_speedOfSlope = 0.8f;
        else a_speedOfSlope = 0.0f;
        //跳跃：为刚体施加一个向上的力:该力是由向上的跳跃速度和在刚体原速度方向上的一个速度合成的
        //VelocityChange是一个瞬时速度
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            playerAction = PlayerAction.jump;
            Jump();
        }
        //下蹲
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (speed >= run_speed || isSlidingTakle || isSlope())
            {
                playerAction = PlayerAction.slidingTakle;
            }
            else
            {
                playerAction = PlayerAction.crouch;
            }
        }
        //加速跑
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            playerAction = PlayerAction.run;
        }
        //走路
        else
        {
            playerAction = PlayerAction.walk;
        }
        if (Input.GetMouseButton(1))
        {
            playerAction = PlayerAction.aim;
            ani_camera.SetBool("isAim", true);
        }
        else if (!Input.GetMouseButton(1))
        {
            ani_camera.SetBool("isAim", false);
        }
        if (Input.GetMouseButton(0))
        {
            playerAction = PlayerAction.aim;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerAction = PlayerAction.useHealthPackageOrBattery;
        }
        if (gun != null && ((Input.GetKeyDown(KeyCode.R) && gun.GetCurrAmmo() != gun.GetMaxAmmo())
            || gun.GetCurrAmmo() == 0))
        {
            playerAction = PlayerAction.reloading;
        }

            switch (playerAction)
        {
            case PlayerAction.walk:
            case PlayerAction.aim:
                Walk();
                break;
            case PlayerAction.run:
                Run();
                break;
            case PlayerAction.slidingTakle:
                SlidingTakle();
                break;
            case PlayerAction.crouch:
                Crouch();
                break;
            case PlayerAction.reloading:
                ReloadAmmo();
                break;
            case PlayerAction.climb:
                break;
            case PlayerAction.useHealthPackageOrBattery:
                break;
        }
    }

    void GetDirection()
    {
        moveScale = 0;

        float rv = Input.GetAxis("Mouse X");
        float rh = Input.GetAxis("Mouse Y");

        m_camRot.x -= rh;
        m_camRot.y += rv;

        m_camTransform.eulerAngles = m_camRot;
        Vector3 objectTran = m_camRot;
        objectTran.x = 0;
        objectTran.z = 0;


        //旋转，使用刚体的旋转
        Quaternion rot = Quaternion.Euler(objectTran);//目标旋转
        Quaternion currentRot = Quaternion.RotateTowards(rigid.rotation, rot, Time.deltaTime * 600);//中间插值旋转
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (playerAction == PlayerAction.run)
                ani_camera.SetBool("isRun", true);
            else
                ani_camera.SetBool("isRun", false);
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = m_transform.TransformDirection(moveDirection);
            moveScale = 1;
        }
        rigid.MoveRotation(currentRot);
    }

    public virtual void UseQ()
    {
        int cd = playerState.Q_CD;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerStateManager.playerStateManager_instance.Set_Q_cdTime(cd);
        }
    }

    public virtual void UseZ()
    {
        float cd = playerState.Z_CD;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerStateManager.playerStateManager_instance.Set_Z_cdTime(cd);
        }
    }
    public void UseBag()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            BagSystem.SetActive(true);
            isBagOpen = true;
        }
        if (isBagOpen)
        {
            StartCoroutine(OpenBag());
        }
    }

    IEnumerator OpenBag()
    {
        yield return new WaitForSeconds(1);
        if (Input.GetKeyDown(KeyCode.Tab) && isBagOpen)
        {
            BagSystem.SetActive(false);
            isBagOpen = false;
            yield break;
        }
    }

    void ReloadAmmo()
    {
        //换弹时子弹不能和最大子弹数相等且备用子弹不能为0
        
            
            int i = playerState.bag.GetAmmo(gun.GetAmmoKind());
            if (i == 0)
            {
                Debug.Log("背包已无该类型子弹");
                return;
            }
             if (i > gun.GetMaxAmmo())
                {
                    playerState.bag.ReduceAmmo(gun.GetAmmoKind(), gun.GetMaxAmmo() - gun.GetCurrAmmo());
                    gun.Reload(gun.GetMaxAmmo());
                }
            else
                {
                    playerState.bag.ReduceAmmo(gun.GetAmmoKind(), i);
            gun.Reload(i);
                }
             weaponUIcontroller();
            
    }

    //void ChangeGun() //切枪
    //{
    //    //当一把枪为可活动时，另一把枪禁止
    //    if (Input.GetKeyDown(KeyCode.F) && playerGun.gun_own[0] != null && playerGun.gun_own[1] != null)
    //    {
    //        if(playerGun.gun_own[0].activeInHierarchy)
    //        {
    //            playerGun.gun_own[0].SetActive(false);
    //            playerGun.gun_own[1].SetActive(true);
    //            gun = playerGun.gun_own[1].GetComponent<GunInfo>();
    //            ani_main = playerGun.gun_own[1].GetComponent<Animator>();
    //        }
    //        else
    //        {
    //            playerGun.gun_own[1].SetActive(false);
    //            playerGun.gun_own[0].SetActive(true);
    //            gun = playerGun.gun_own[0].GetComponent<GunInfo>();
    //            ani_main = playerGun.gun_own[0].GetComponent<Animator>();
    //        }
            
    //    }
        
    //}
    
    void PickUp()
    {
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
        if (Physics.Raycast(ray, out hitInfo))
        {
            //相机到物体的距离  
            if ((m_camTransform.position - hitInfo.transform.position).magnitude < 2.0f && Input.GetKeyDown(KeyCode.E))
            {
                //识别拾取的碰撞体的类型
                if (hitInfo.transform.CompareTag("Gun"))
                {
                    GameObject gunModel = playerGunPool.GetGun(hitInfo.transform.name);
                    //如果一开始没有武器
                    if (playerState.bag.isMainGunNull() || isMain)
                    {
                        //替换原有枪支后重新在地上生成
                        if (!playerState.bag.isMainGunNull())
                        {
                            //生成主武器模型和其信息；
                            GameObject newGun = ObjectsPool.Instance.
                                GetPooledObject_Item(playerState.bag.GetMainGun().GetGunName());
                            newGun.GetComponent<Gun>().ThisGunItem = playerState.bag.GetMainGun().ThisGunItem;
                            newGun.transform.position = m_transform.position;
                            newGun.transform.rotation = Quaternion.identity;
                            newGun.SetActive(true);
                            newGun.AddComponent<Rigidbody>();
                            newGun.AddComponent<BoxCollider>();
                             playerGunPool.GetActiveGun().SetActive(false);
                        }
                        //存储主武器模型  
                        gun = gunModel.GetComponent<Gun>();
                        //存入数据存储类Bag中
                        playerState.bag.SetGun(gun, 0);
                        gunModel.SetActive(true);
                        gunModel.GetComponent<Gun>().ThisGunItem = gun.ThisGunItem;
                        playerGunPool.gameObjects[0] = gunModel;
                    }
                    else if (playerState.bag.isSecondaryGunNull()||!isMain)
                       {
                          if (!playerState.bag.isSecondaryGunNull())
                         {
                            //可以依靠对象池
                            GameObject newGun = ObjectsPool.Instance.
                                GetPooledObject_Item(playerState.bag.GetSecondaryGun().GetGunName());
                            newGun.GetComponent<Gun>().ThisGunItem = playerState.bag.GetMainGun().ThisGunItem;
                            newGun.transform.position = m_transform.position;
                            newGun.transform.rotation = Quaternion.identity;
                            newGun.SetActive(true);
                            newGun.AddComponent<Rigidbody>();
                            newGun.AddComponent<BoxCollider>();
                        }        
                        gun =gunModel.GetComponent<Gun>();
                        playerGunPool.GetActiveGun().SetActive(false);
                        gunModel.SetActive(true);
                        playerState.bag.SetGun(gun, 1);
                        playerGunPool.gameObjects[1] = gunModel;
                    }
                    WeaponStateManager.weaponStateManager_Instance.SetGun(gun);
                    weaponUIcontroller += new WeaponUIController(WeaponStateManager.weaponStateManager_Instance.SetWeaponUI);
                    weaponUIcontroller();
                }
              else if (hitInfo.transform.CompareTag("Item"))
                {
                    ItemIDAndNum iDAndNum = hitInfo.transform.GetComponent<ItemIDAndNum>();
                    playerState.bag.AddItem(iDAndNum.num, (int)iDAndNum.ID);
                }
             else if(hitInfo.transform.CompareTag("Armor"))
                {
                    Armor newArmor = hitInfo.transform.GetComponent<Armor>();
                    if(playerState.bag.GetLevel()<newArmor.GetArmorData().GetLevel())
                    playerState.bag.SetArmor(newArmor.GetArmorData());
                   
                }
            else if(hitInfo.transform.CompareTag("Helmet"))
                {
                    Helmet newHelmet = hitInfo.transform.GetComponent<Helmet>();
                    if(playerState.bag.GetHelmetLevel()<newHelmet.GetHelmetLevel())
                    playerState.bag.SetHelmetLevel(newHelmet.GetHelmetLevel());
                }
                hitInfo.transform.gameObject.SetActive(false);
            }
        }

    }

        //防止连续跳跃
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject!= null&&collision.transform.position.y<=this.transform.position.y)
        {
            isGround = true;
        }
    }


    void Run()
    {
        speed = run_speed;
    }

    void SlidingTakle()
    {
        if (speed >= crouch_speed && speed <= maxSpeedOfSlope)
        {
            speed += ((-friction) + a_speedOfSlope);
            isSlidingTakle = true;
        }
        else
            isSlidingTakle = false;
    }

    void Crouch()
    {
        speed = crouch_speed;
    }

    void Walk()
    {
        speed = walk_speed;
    }

    void Jump()
    {
        rigid.AddForce(transform.up * jumpHeight /*+ rigid.velocity.normalized * directionalJumpFactor*/, ForceMode.VelocityChange);
        isGround = false;
    }

    void UseMedic()
    {
        
    }

    void UseBattery()
    {

    }
     
    void Shoot()
    {
        if(Input.GetMouseButton(0)&&gun!=null)
        {
            try
            {
                gun.ShootBullet();
            }
            catch
            {
                Debug.Log("射击有问题");
            }
        }
        
    }

    void ChangeGun()
    {
        if (playerGunPool.gameObjects[0] != null && playerGunPool.gameObjects[1] != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerGunPool.GetActiveGun().SetActive(false);
                playerGunPool.GetGun(playerState.bag.GetMainGun().GetGunName()).SetActive(true);
                gun = playerState.bag.GetMainGun();
                isMain = true;
                WeaponStateManager.weaponStateManager_Instance.SetGun(gun);
                weaponUIcontroller();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                isMain = false;
                playerGunPool.GetActiveGun().SetActive(false);
                playerGunPool.GetGun(playerState.bag.GetSecondaryGun().GetGunName()).SetActive(true);
                gun = playerState.bag.GetSecondaryGun();
                WeaponStateManager.weaponStateManager_Instance.SetGun(gun);
                weaponUIcontroller();
            }
            Debug.Log(isMain);
        }
    }
 
}
