using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Start is called before the first frame update

    float collection_time = 0;
    //是否在地上
    public  bool isOnTheGround=true;

    GameObject c_bullet;
    //射击点
    public Transform shootPoint;

    public GunItem gunItem;

    float timer = 0;
    GunItem thisGunItem;

    public GunItem ThisGunItem
    {
        get { return thisGunItem; }
        set { thisGunItem = value; }
    }
    //枪械动画
    public Animator animator;

    public GameObject gunModel;


    [SerializeField]
    int cost = 1;
    protected void Awake()
    {
        thisGunItem = ScriptableObject.CreateInstance<GunItem>();
        thisGunItem.intial(gunItem);
        if (isOnTheGround)
            this.enabled = false;
        gunModel = this.gameObject;
    }

    public int GetCurrAmmo()
    {
        return thisGunItem.currAmmo;
    }

    public int GetMaxAmmo()
    {
        return thisGunItem.maxAmmo;
    }

    public AmmoKind GetAmmoKind()
    {
        return thisGunItem.ammoKind;
    }

    public Sprite GetGunSprite()
    {
        return thisGunItem.gunSprite;
    }

    public Sprite GetAmmoSprite()
    {
        return thisGunItem.ammoSprite;
    }
    public string GetGunName()
    {
        return thisGunItem.gunName;
    }
    public GunItem GetGunData()
    {
        return thisGunItem;
    }
    public virtual  void ChangeAmmo(int cost)
    {
        thisGunItem.currAmmo -= cost;
        if (thisGunItem.currAmmo < 0)
            thisGunItem.currAmmo = 0;
    }

    public virtual void ShootBullet()
    {
        if (thisGunItem.currAmmo == 0)
            return;
        //子弹发射方向
        Vector3 shootDirection = shootPoint.transform.forward;
        //计时器
        timer += Time.deltaTime;
        collection_time += Time.deltaTime;
        //子弹偏移量
        float OffSet_y = Random.Range(-0.1f, 0.1f);
        float OffSet_x = Random.Range(-0.1f, 0.1f);
        //开镜射
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && thisGunItem.currAmmo>= 0)
        {
            Shoot(shootDirection);
        }
        //腰射
        else if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && thisGunItem.currAmmo >= 0 )
        {
            shootDirection.x += OffSet_x;
            shootDirection.y += OffSet_y;
            Shoot(shootDirection);
        }
    }

    public virtual void Reload(int i)
    {
       
            thisGunItem.currAmmo = i;
            animator.SetTrigger("reload");
     
    }

    void Shoot(Vector3 direction)
    {
        if (timer >= thisGunItem.shootSpeed)
        {
            ChangeAmmo(cost);
            c_bullet = ObjectsPool.Instance.GetPooledObject("Bullet");
            if (c_bullet != null)
            {
                c_bullet.transform.position = shootPoint.transform.position;
                c_bullet.transform.rotation = Quaternion.identity;
                c_bullet.SetActive(true);
            }
            c_bullet.GetComponent<Rigidbody>().velocity = direction * 20;
            StartCoroutine(collection(c_bullet));
                timer = 0;
        }
    }

   
    IEnumerator collection(GameObject c)
    {
        yield return new WaitForSeconds(2);
        c.SetActive(false);
    }
}
