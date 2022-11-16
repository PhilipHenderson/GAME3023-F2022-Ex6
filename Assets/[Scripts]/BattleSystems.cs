using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystems : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerPlatform;
    public Transform enemyPlatform;

    Unit playerUnit;
    Unit enemyUnit;

    public Text encounterTxt;

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

    // Attack Menu Coroutines
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

    IEnumerator EnemyTurn()
    {
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
        else
        {
            states = BattleState.PLAYERTURN;
            PlayerTurn();
        }
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

    void PlayerTurn()
    {
        encounterTxt.text = "Please Choose An Action:";
    }

    // Buttons
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


}
