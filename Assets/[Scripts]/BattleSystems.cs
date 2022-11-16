using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystems : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject playerFlameIcon;
    public GameObject playerIceIcon;
    public GameObject enemyFlameIcon;
    public GameObject enemyIceIcon;
    public GameObject playerWindIcon;
    public GameObject enemyWindIcon;

    public Transform playerPlatform;
    public Transform enemyPlatform;

    Unit playerUnit;
    Unit enemyUnit;

    public TMP_Text encounterTxt;

    public BattlePlatform playerHud;
    public BattlePlatform enemyHud;

    public BattleState states;

    // Start is called before the first frame update
    void Start()
    {
        states = BattleState.START;
        StartCoroutine(BattleSetup());
        playerUnit.defendedDmg = playerUnit.dmg / 2;
        enemyUnit.defendedDmg = enemyUnit.dmg / 2;
    }


    IEnumerator BattleSetup()
    {
        GameObject player = Instantiate(playerPrefab, playerPlatform);
        playerUnit = player.GetComponent<Unit>();
        GameObject enemy = Instantiate(enemyPrefab, enemyPlatform);
        enemyUnit = enemy.GetComponent<Unit>();

        // Create More of these and randomize it
        encounterTxt.text = "A Sneaky " + enemyUnit.Unitname + " Has Apreared!";

        playerHud.HudSet(playerUnit);
        enemyHud.HudSet(enemyUnit);

        yield return new WaitForSeconds(2.0f);

        states = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    // Player Menu Coroutines
    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.dmg);

        enemyHud.HpSet(enemyUnit.currentHp);
        encounterTxt.text = "The Attack Was Successfull!";

        yield return new WaitForSeconds(2.0f);

        if (isDead)
        {
            states = BattleState.WON;
            EndBattle();
        }
        else
        {
            states = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5);

        playerHud.HpSet(playerUnit.currentHp);
        encounterTxt.text = "The Player Healed!";

        yield return new WaitForSeconds(2.0f);

        states= BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator PlayerDefend()
    {
        playerUnit.isDefending = true;
        encounterTxt.text = "Player Gets Into A Defencive Position!";
        yield return new WaitForSeconds(2.0f);
        states = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator PlayerFireBlast()
    {

        bool isDead = enemyUnit.TakeDamage(playerUnit.fireBlastDmg);

        enemyUnit.isOnFire = true;
        enemyFlameIcon.SetActive(true);
        enemyUnit.burningTurnsLeft = 2;
        enemyHud.HpSet(enemyUnit.currentHp);
        encounterTxt.text = "The Fire Blast Was Successfull!";

        yield return new WaitForSeconds(2.0f);

        if (isDead)
        {
            states = BattleState.WON;
            EndBattle();
        }
        else
        {
            states = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerWindCannon()
    {
        encounterTxt.text = "The Wind Cannon Was Successfull!";
        enemyWindIcon.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        states = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator PlayerIcePistol()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.icePistolDmg);

        enemyUnit.isFrozen = true;
        enemyIceIcon.SetActive(true);
        enemyUnit.freezingTurnsLeft = 2;
        enemyHud.HpSet(enemyUnit.currentHp);
        encounterTxt.text = "The Ice Pistol Was Successfull!";

        yield return new WaitForSeconds(2.0f);

        if (isDead)
        {
            states = BattleState.WON;
            EndBattle();
        }
        else
        {
            states = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    // Enemy Menu Coroutines
    IEnumerator EnemyTurn()
    {
        // 1. Pre Round - Enemy Status Check
        if (!enemyUnit.isBlownAway) // no damage, but cant attack for one turn
        {


            if (enemyUnit.isOnFire) // where the enemy takes damage from being on fire
            {
                encounterTxt.text = enemyUnit.Unitname + " is being burnt by fire";
                enemyUnit.TakeDamage(playerUnit.burningDmg);
                if (enemyUnit.burningTurnsLeft > 0)
                {
                    enemyUnit.burningTurnsLeft--;
                    if (enemyUnit.burningTurnsLeft == 0)
                    {
                        enemyUnit.isOnFire = false;
                        enemyFlameIcon.SetActive(false);
                    }
                }
            }

            if (enemyUnit.isFrozen) // where the enemy takes damage from being frozen
                {
                    encounterTxt.text = enemyUnit.Unitname + " is being Frozen By Ice";
                    enemyUnit.TakeDamage(playerUnit.freezingDmg);

                    yield return new WaitForSeconds(2.0f);

                    if (enemyUnit.freezingTurnsLeft > 0)
                    {
                        enemyUnit.freezingTurnsLeft--;
                        if (enemyUnit.freezingTurnsLeft == 0)
                        {
                            enemyUnit.isFrozen = false;
                            enemyIceIcon.SetActive(false);
                        }
                    }
                }

            // 2. Main Round - Enemy Logic, (Only Enemy Attack Section RN)
            encounterTxt.text = enemyUnit.Unitname + " Attacks";

            yield return new WaitForSeconds(1.0f);

            bool isDead = playerUnit.TakeDamage(enemyUnit.dmg);

            playerHud.HpSet(playerUnit.currentHp);

            yield return new WaitForSeconds(1.0f);

            encounterTxt.text = "Please Choose An Action:";

            if (isDead)
            {
                states = BattleState.LOST;
                EndBattle();
            }
        }

        encounterTxt.text = enemyUnit.Unitname + " Has Being Blown Over, They Cannot Attack";
        enemyUnit.isBlownAway = false;
        enemyWindIcon.SetActive(false);
        // 3. Changeing to players turn
        states = BattleState.PLAYERTURN;
        PlayerTurn();

        
    }

    void PlayerTurn()
    {
        encounterTxt.text = "Please Choose An Action:";
    }

    void EndBattle()
    {
        if (states == BattleState.WON)
        {
            encounterTxt.text = "You Have Defeated The Enemy";
        }
        else if (states == BattleState.LOST)
        {
            encounterTxt.text = "The Enemy Has Defeated You";
        }
    }


    // Action Buttons
    public void OnAttackButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }
    public void OnHealButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
    public void OnDefendButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerDefend());
    }

    // Ability Buttons
    public void OnFireBlastButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerFireBlast());
    }
    public void OnWindCannonButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerWindCannon());
    }
    public void OnIcePistolButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerIcePistol());
    }

}
