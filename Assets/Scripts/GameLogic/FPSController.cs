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

    public float walk_speed = 5.0f;//�ƶ��ٶ�

    private Vector3 moveDirection = Vector3.zero;//��ɫ�ķ���

    public float run_speed = 10.0f;//�ܲ��ٶ�


    public float aim_speed = 2.0f;//����ʱ����
    public float crouch_speed = 1.0f;//�¶��ٶ�
    //����
    Transform m_transform;

    //��ǰλ��
    Transform m_curTransform;
    //Ħ����
    [SerializeField]
    float friction = 0.2f;

    //���
    public Transform m_camTransform;

    //��Ծ�߶�
    [SerializeField]
    float jumpHeight = 0.0f;

    //��ǰ����ĸ߶ȼ�¼����������һ֡�ĸ߶Ƚ��жԱȣ�
    float y1 = 0.0f;

    [SerializeField]
    //�»����ٶ�
    float a_speedOfSlope = 0.0f;
    //����»��ٶ�
    [SerializeField]
    float maxSpeedOfSlope = 15.0f;

    //�����������
    Animator ani_camera;

    //ǹе����
    //�������ת
    Vector3 m_camRot;

    bool isGround = false;


    //�����Ƿ��
    bool isBagOpen = false;

    bool isSlidingTakle = false;
    float moveScale = 0;

    public bool main_active = false;

    //����ģ��ʵ����(��/��)

    //���ָ�������
    Ray ray;
    //��ײ����Ϣ
    RaycastHit hitInfo;
    public Transform bullet;

    GunItem gunItem;
    public Gun gun;
    //����״̬
    PlayerAction playerAction;
    //��ʰȡ������
    Item item;
    public PlayerState playerState;

    //���������
    PlayerGunPool playerGunPool;
    //����UIί��
   delegate void WeaponUIController();
   event WeaponUIController weaponUIcontroller;
    //���״̬ί��
    delegate void PlayerStateUIController();
    event PlayerStateUIController playerStateUIcontroller;
    //����UIί��
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
        //�жϵ�ǰ���İ�
        ChangeGun();
        PickUp();
        Shoot();
        UseBag();
        playerStateUIcontroller();
        //ˮƽ�ƶ�
        Vector3 v = Vector3.Project(rigid.velocity, transform.forward);
        float s = speed == 0 ? 0 : 1 - v.magnitude / speed;
        //rigid.AddForce(transform.forward * moveScale * s, ForceMode.VelocityChange);//��һ�֣����������ô����Ƚ���ʵ�����ƶ���������ϵͳ��ȱ�㣺���ÿ��ƣ����׻���
        //rigid.velocity = transform.forward * 10 * moveScale + Vector3.up * rigid.velocity.y;//�ڶ��֣��ٶ����ã��ô���ֱ�������ٶȣ�ȱ�㣺ֱ�Ӹ��������ٶȣ�����ʧ��
        //if (moveScale == 1) rigid.MovePosition((rigid.position + transform.forward * moveSpeed * Time.deltaTime));//�����֣�ֱ������λ�ã��ô���ֱ���ƶ���ȱ�㣺Ŀǰ����������
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
        //��Ծ��Ϊ����ʩ��һ�����ϵ���:�����������ϵ���Ծ�ٶȺ��ڸ���ԭ�ٶȷ����ϵ�һ���ٶȺϳɵ�
        //VelocityChange��һ��˲ʱ�ٶ�
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            playerAction = PlayerAction.jump;
            Jump();
        }
        //�¶�
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
        //������
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            playerAction = PlayerAction.run;
        }
        //��·
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


        //��ת��ʹ�ø������ת
        Quaternion rot = Quaternion.Euler(objectTran);//Ŀ����ת
        Quaternion currentRot = Quaternion.RotateTowards(rigid.rotation, rot, Time.deltaTime * 600);//�м��ֵ��ת
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
        //����ʱ�ӵ����ܺ�����ӵ�������ұ����ӵ�����Ϊ0
        
            
            int i = playerState.bag.GetAmmo(gun.GetAmmoKind());
            if (i == 0)
            {
                Debug.Log("�������޸������ӵ�");
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

    //void ChangeGun() //��ǹ
    //{
    //    //��һ��ǹΪ�ɻʱ����һ��ǹ��ֹ
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
            //���������ľ���  
            if ((m_camTransform.position - hitInfo.transform.position).magnitude < 2.0f && Input.GetKeyDown(KeyCode.E))
            {
                //ʶ��ʰȡ����ײ�������
                if (hitInfo.transform.CompareTag("Gun"))
                {
                    GameObject gunModel = playerGunPool.GetGun(hitInfo.transform.name);
                    //���һ��ʼû������
                    if (playerState.bag.isMainGunNull() || isMain)
                    {
                        //�滻ԭ��ǹ֧�������ڵ�������
                        if (!playerState.bag.isMainGunNull())
                        {
                            //����������ģ�ͺ�����Ϣ��
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
                        //�洢������ģ��  
                        gun = gunModel.GetComponent<Gun>();
                        //�������ݴ洢��Bag��
                        playerState.bag.SetGun(gun, 0);
                        gunModel.SetActive(true);
                        gunModel.GetComponent<Gun>().ThisGunItem = gun.ThisGunItem;
                        playerGunPool.gameObjects[0] = gunModel;
                    }
                    else if (playerState.bag.isSecondaryGunNull()||!isMain)
                       {
                          if (!playerState.bag.isSecondaryGunNull())
                         {
                            //�������������
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

        //��ֹ������Ծ
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
                Debug.Log("���������");
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
