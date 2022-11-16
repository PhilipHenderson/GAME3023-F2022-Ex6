using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Unit Info")]
    public string Unitname;
    public int Unitlvl;
    public int dmg;
    public int defendedDmg;
    public int maxHp;
    public int currentHp;

    public bool isDefending = false;

    public bool TakeDamage(int dmg)
    {
        if (!isDefending)
        {
            currentHp -= dmg;

            if (currentHp <= 0) return true;
            else return false;
        }
        else
        {
            currentHp -= defendedDmg;
            isDefending = false;

            if (currentHp <= 0) return true;
            else return false;
        }
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if(currentHp > maxHp)
            currentHp = maxHp;
    }
}
