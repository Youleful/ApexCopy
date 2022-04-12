using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager playerStateManager_instance = null;


    PlayerState playerState;
    public Dictionary<int, Color> levelColor = new Dictionary<int, Color>();

    //文字控件
    Text Q_cdTime;
    Text Z_cdTime;
    Text Damage;

    //图片控件
    Image Q_ablity;
    Image Z_ability;
    Image I_armor;
    Image helmet;
    Image Person;

    //生命条滑动
    Slider m_lifeBar;
    Slider m_armorBar;



    private void Awake()
    {
        playerStateManager_instance = this;
        levelColor.Add(1, Color.white);
        levelColor.Add(2, Color.blue);
        levelColor.Add(3, Color.green);
        levelColor.Add(4, Color.yellow);
    }

    public void SetPlayerState(PlayerState p)
    {
        playerState = p;
    }
    void Start()
    {
        foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("Final_cd") == 0)
            {
                Z_cdTime = t.GetComponent<Text>();
            }
            else if (t.name.CompareTo("Final_Skill") == 0)
            {
                Z_ability = t.GetComponent<Image>();
            }
            else if (t.name.CompareTo("Q_Skill") == 0)
            {
                Q_ablity = t.GetComponent<Image>();
            }
            else if (t.name.CompareTo("Q_cd") == 0)
            {
                Q_cdTime = t.GetComponent<Text>();
            }
            else if (t.name.CompareTo("Shield") == 0)
            {
                m_armorBar = t.GetComponent<Slider>();
            }
            else if (t.name.CompareTo("Life") == 0)
            {
                m_lifeBar = t.GetComponent<Slider>();
            }
            else if (t.name.CompareTo("Shield_level") == 0)
            {
                I_armor = t.GetComponent<Image>();
            }
            else if (t.name.CompareTo("Helmet") == 0)
            {
                helmet = t.GetComponent<Image>();
            }
            else if (t.name.CompareTo("Damage") == 0)
            {
                Damage = t.GetComponent<Text>();
            }
            
        }
        playerState = GameObject.Find("Body").GetComponent<FPSController>().playerState;
        Damage.text = string.Format("升级所需的伤害：{0}", playerState.bag.GetArmorItem().needDamage-
            playerState.bag.GetArmorItem().GetHasDoDamage());
        Q_cdTime.text = "";
        Z_cdTime.text = "";
    }
    public void SetStateUI()
    {
        I_armor.color = levelColor[playerState.bag.GetArmorItem().GetLevel()];
        helmet.color = levelColor[playerState.bag.GetHelmetLevel()];
        SetSliderBar_armor();
        SetSliderBar_life();
    }

    public Image GetHelmet()
    {
        return helmet;
    }
    public Image GetArmor()
    {
        return I_armor;
    }

    public void GetDamage(int damage)
    {
        playerState.bag.GetArmorItem().GetDamage(damage);
        if (playerState.bag.GetArmorItem().GetCurrArmor() < damage)
            playerState.life = playerState.life - (damage - playerState.bag.GetArmorItem().GetHasDoDamage());
    }

     void SetSliderBar_life()
    {
        m_lifeBar.value = playerState.life / playerState.maxLife;
    }

     void SetSliderBar_armor()
    {
        m_armorBar.value = playerState.bag.GetArmorItem().GetCurrArmor()/ playerState.bag.GetArmorItem().maxArmor;
    }
    public void UpdateArmor()
    {
        playerState.bag.GetArmorItem().UpdateLevel();
        Damage.text = string.Format("升级所需的伤害：{0}", playerState.bag.GetArmorItem().needDamage -
            playerState.bag.GetArmorItem().GetHasDoDamage());
    }


    void UseObject()
    {

    }

    public void Set_Q_cdTime(int cd)
    {
        if (Q_cdTime.text.Equals(""))
            StartCoroutine(countDown_Q(cd));
    }

    public void Set_Z_cdTime(float cd)
    {
        if(Z_cdTime.text.Equals(""))
        StartCoroutine(countDown_Z(cd));
    }


    IEnumerator countDown_Q(int time)
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            Q_cdTime.text = string.Format("{0}s", time);
        }
        Q_cdTime.text = "";
        yield break;
    }

    IEnumerator countDown_Z(float time)
    {
        float curr_time = 0;
        while (curr_time < time)
        {
            yield return new WaitForSeconds(1);
            curr_time++;
            Z_cdTime.text = (curr_time / time).ToString("P0");
        }
        Z_cdTime.text = "";
        yield break;
    }
}

