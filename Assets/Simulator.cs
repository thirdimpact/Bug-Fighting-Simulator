using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Simulator : MonoBehaviour
{
    [SerializeField] private TMP_InputField battleLogTextField;

    [Header("Bug 1 Connectors")]
    [SerializeField] private TMP_Dropdown bug1TypeDropDown;
    [SerializeField] private TMP_InputField bug1NameTextField;
    [SerializeField] private TMP_InputField bug1STRTextField;
    [SerializeField] private TMP_InputField bug1CONTextField;
    [SerializeField] private TMP_InputField bug1DEXTextField;

    [Header("Bug 2 Connectors")]
    [SerializeField] private TMP_Dropdown bug2TypeDropDown;
    [SerializeField] private TMP_InputField bug2NameTextField;
    [SerializeField] private TMP_InputField bug2STRTextField;
    [SerializeField] private TMP_InputField bug2CONTextField;
    [SerializeField] private TMP_InputField bug2DEXTextField;

    private string battleLog;
    int bug1Init;
    int bug2Init;
    int bug1InitRoll;
    int bug2InitRoll;
    int first = 2;
    int second = 2;
    string deadBug = "None";
    string winnerBug = "None";


    Bug[] bug = new Bug[2];

    private void Start()
    {
        bug[0] = new Bug();
        bug[1] = new Bug();
    }

    public void StartSimulation()
    {
        int round = 1;

        // Bug 1 Stats

        bug[0].name = bug1NameTextField.text;
        bug[0].type = CheckType(bug1TypeDropDown.value);
        bug[0].str = int.Parse(bug1STRTextField.text);
        bug[0].strMod = CheckModifier(bug[0].str);
        bug[0].con = int.Parse(bug1CONTextField.text);
        bug[0].conMod = CheckModifier(bug[0].con);
        bug[0].dex = int.Parse(bug1DEXTextField.text);
        bug[0].dexMod = CheckModifier(bug[0].dex);
        bug[0].dmg = CheckDamage(bug[0].str);
        bug[0].hp = CheckHP(bug[0].con);
        bug[0].spd = CheckSpeed(bug[0].dex);
        bug[0].eff = CheckEffective(bug1TypeDropDown.value, bug2TypeDropDown.value);

        // Bug 2 Stats

        bug[1].name = bug2NameTextField.text;
        bug[1].type = CheckType(bug2TypeDropDown.value);
        bug[1].str = int.Parse(bug2STRTextField.text);
        bug[1].strMod = CheckModifier(bug[1].str);
        bug[1].con = int.Parse(bug2CONTextField.text);
        bug[1].conMod = CheckModifier(bug[1].con);
        bug[1].dex = int.Parse(bug2DEXTextField.text);
        bug[1].dexMod = CheckModifier(bug[1].dex);
        bug[1].dmg = CheckDamage(bug[1].str);
        bug[1].hp = CheckHP(bug[1].con);
        bug[1].spd = CheckSpeed(bug[1].dex);
        bug[1].eff = CheckEffective(bug2TypeDropDown.value, bug1TypeDropDown.value);

        // Into message
        battleLog = "Fight log.";
        AddSpaceBattleLog();

        UpdateBattleLog(bug[0].name);
        UpdateBattleLog("Str " + bug[0].str);
        UpdateBattleLog("Con " + bug[0].con);
        UpdateBattleLog("Dex " + bug[0].dex);
        UpdateBattleLog("Type " + bug[0].type);

        AddSpaceBattleLog();
        UpdateBattleLog("VS");
        AddSpaceBattleLog();
        
        UpdateBattleLog(bug[1].name);
        UpdateBattleLog("Str " + bug[1].str);
        UpdateBattleLog("Con " + bug[1].con);
        UpdateBattleLog("Dex " + bug[1].dex);
        UpdateBattleLog("Type " + bug[1].type);

        AddSpaceBattleLog();
        UpdateBattleLog("Welcome to the Zoo Bugaloo!");
        AddSpaceBattleLog();
        UpdateBattleLog(bug[0].name + " VS. " + bug[1].name + "!");
        AddSpaceBattleLog();
        UpdateBattleLog(bug[0].name + " " + bug[0].hp + " HP");
        UpdateBattleLog(bug[1].name + " " + bug[1].hp + " HP");
        AddSpaceBattleLog();

        // Loop combat until one bug dies
        while (bug[0].hp != 0 && bug[1].hp != 0)
        {
            AddSpaceBattleLog();
            UpdateBattleLog("Round " + round);
            Debug.Log("Round is " + round);
            round++;
            AddSpaceBattleLog();

            Combat();
        }

        AddSpaceBattleLog();
        UpdateBattleLog(deadBug + " is knocked out.");
        UpdateBattleLog(winnerBug + " is victorious!");
    }

    void Combat()
    {
        CheckInitiativeOrder();

        Attack(first, second);

        AddSpaceBattleLog();
        UpdateBattleLog(bug[0].name + " " + bug[0].hp + " HP");
        UpdateBattleLog(bug[1].name + " " + bug[1].hp + " HP");
        AddSpaceBattleLog();

        if (bug[second].hp != 0)
        {
            FollowUpUpdate();

            Attack(second, first);

            AddSpaceBattleLog();
            UpdateBattleLog(bug[0].name + " " + bug[0].hp + " HP");
            UpdateBattleLog(bug[1].name + " " + bug[1].hp + " HP");
            AddSpaceBattleLog();
        }
    }

    void InitiativeRoll()
    {
        bug1InitRoll = (Random.Range(1, 20));
        bug2InitRoll = (Random.Range(1, 20));
        bug1Init = bug1InitRoll + bug[0].spd;
        bug2Init = bug2InitRoll + bug[1].spd;
    }

    bool SetOrderByInitiative()
    {
        bool reroll = false;
        if (bug1Init > bug2Init)
        {
            UpdateBattleLog(bug[0].name + " takes initiative. [" + bug1InitRoll + "] " + ModifierAsText(bug[0].spd) + " [Speed]");
            first = 0;
            second = 1;
        }
        if (bug2Init > bug1Init)
        {
            UpdateBattleLog(bug[1].name + " takes initiative. [" + bug2InitRoll + "] " + ModifierAsText(bug[1].spd) + " [Speed]");
            first = 1;
            second = 0;
        }
        if (bug1Init == bug2Init)
        {
            if (bug[0].dex > bug[1].dex)
            {
                UpdateBattleLog(bug[0].name + " takes initiative. [" + bug1InitRoll + "] " + ModifierAsText(bug[0].spd) + " [Speed]");
                first = 0;
                second = 1;
            }
            if (bug[1].dex > bug[0].dex)
            {
                UpdateBattleLog(bug[1].name + " takes initiative. [" + bug2InitRoll + "] " + ModifierAsText(bug[1].spd) + " [Speed]");
                first = 1;
                second = 0;
            }
            if (bug[0].dex == bug[1].dex)
            {
                UpdateBattleLog("Their speed are a complete match! Both rolled [" + bug1InitRoll + "] and have " + bug[0].dex + " [Dex] points. Re-rolling initiative!");
                reroll = true;
            }
        }
        //Debug.Log("Bug 1 rolled " + bug1InitRoll + " and got an initiative of " + bug1Init + " from +" + bug[0].spd + " speed.");
        //Debug.Log("Bug 2 rolled " + bug2InitRoll + " and got an initiative of " + bug2Init + " from +" + bug[1].spd + " speed.");
        //Debug.Log("var first is set to " + first);
        //Debug.Log("var second is set to " + second);
        return reroll;
    }

    void CheckInitiativeOrder()
    {
        bool reroll = false;
        InitiativeRoll();
        reroll = SetOrderByInitiative();
        while (reroll == true)
        {
            reroll = false;
            InitiativeRoll();
            reroll = SetOrderByInitiative();
        }
    }

    void FollowUpUpdate()
    {
        if (bug1Init > bug2Init)
        {
            UpdateBattleLog(bug[1].name + " follows up. [" + bug2InitRoll + "] " + ModifierAsText(bug[1].spd) + " [Speed]");
        }
        if (bug2Init > bug1Init)
        {
            UpdateBattleLog(bug[0].name + " follows up. [" + bug1InitRoll + "] " + ModifierAsText(bug[0].spd) + " [Speed]");
        }
        if (bug1Init == bug2Init)
        {
            if (bug[0].dex > bug[1].dex)
            {
                UpdateBattleLog(bug[1].name + " follows up. [" + bug1InitRoll + "] " + ModifierAsText(bug[1].spd) + " [Speed]");
            }
            if (bug[1].dex > bug[0].dex)
            {
                UpdateBattleLog(bug[0].name + " follows up. [" + bug2InitRoll + "] " + ModifierAsText(bug[0].spd) + " [Speed]");
            }
        }
    }


    // For future reference, first and second doesnt make sense in a standalone function, first should be attacker, second should be defender.
    void Attack(int first, int second)
    {
        Debug.Log("---------------------------------------------------");
        Debug.Log(bug[first].name + " attacking");

        int firstAttackRoll = (Random.Range(1, 20));

        Debug.Log(bug[first].name + " rolled " + firstAttackRoll);

        bool critical = false;
        if (firstAttackRoll == 20)
        {
            critical = true;
        }
        int firstAttack;
        int firstFullDamage = 0;

        // 1 = STR, 2 = CON, 3 = DEX
        int contestType = Random.Range(1, 3);

        Debug.Log("Contest type is " + contestType);
        Debug.Log("Current eff situation is:");
        Debug.Log(bug[first].name + " effective: " + bug[first].eff);
        Debug.Log(bug[second].name + " effective: " + bug[second].eff);

        if (contestType == 1) // ------------- Strength contest
        {
            Debug.Log("Contest type " + contestType + " activated.");
            firstAttack = firstAttackRoll + bug[first].strMod;
            if (firstAttack >= bug[second].str)
            {
                UpdateBattleLog("Hits with a Bash attack! [" + firstAttackRoll + "] " + ModifierAsText(bug[first].strMod) + " [Str]");

                int firstDamageRoll = Random.Range(1, bug[first].dmg);
                if (bug[first].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll + 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Effective] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll + 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Effective]");
                    }
                }
                if (!bug[first].eff && !bug[second].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "]");
                    }
                }
                if (!bug[first].eff && bug[second].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll - 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] -1 [Not effective] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll - 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] -1 [Not effective]");
                    }
                }
            }
            if (firstAttack < bug[second].str)
            {
                UpdateBattleLog("Misses with a Bash attack! [" + firstAttackRoll + "] " + ModifierAsText(bug[first].strMod) + " [Str]");
                UpdateBattleLog(bug[second].name + " is unharmed.");
            }
        }
        if (contestType == 2) // ------------- Constitution contest
        {
            Debug.Log("Contest type " + contestType + " activated.");
            firstAttack = firstAttackRoll + bug[first].conMod;
            if (firstAttack >= bug[second].con)
            {
                UpdateBattleLog("Hits with a Lash attack! [" + firstAttackRoll + "] " + ModifierAsText(bug[first].conMod) + " [Con]");

                int firstDamageRoll = Random.Range(1, bug[first].dmg);
                if (bug[first].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll + 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Effective] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll + 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Effective]");
                    }
                }
                if (!bug[first].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "]");
                    }
                }
                if (!bug[first].eff && bug[second].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll - 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] -1 [Not effective] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll - 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] -1 [Not effective]");
                    }
                }
            }
            if (firstAttack < bug[second].con)
            {
                UpdateBattleLog("Misses with a Lash attack! [" + firstAttackRoll + "] " + ModifierAsText(bug[first].conMod) + " [Con]");
                UpdateBattleLog(bug[second].name + " is unharmed.");
            }
        }
        if (contestType == 3) // ------------- Dexterity contest
        {
            firstAttack = firstAttackRoll + bug[first].dexMod;
            if (firstAttack >= bug[second].dex)
            {
                UpdateBattleLog("Hits with a Dash attack! [" + firstAttackRoll + "] " + ModifierAsText(bug[first].dexMod) + " [Dex]");

                int firstDamageRoll = Random.Range(1, bug[first].dmg);
                if (bug[first].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll + 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Effective] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll + 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Effective]");
                    }
                }
                if (!bug[first].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "]");
                    }
                }
                if (!bug[first].eff && bug[second].eff)
                {
                    if (critical)
                    {
                        firstFullDamage += 1;
                        UpdateBattleLog("It's a critical hit! (+1 to damage)");
                        firstFullDamage += firstDamageRoll - 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] -1 [Not effective] +1 [Crit]");
                    }
                    if (!critical)
                    {
                        firstFullDamage += firstDamageRoll - 1;
                        SubtractHP(first, second, firstFullDamage);
                        UpdateBattleLog(bug[second].name + " HP decreased by [" + firstDamageRoll + "] -1 [Not effective]");
                    }
                }
            }
            if (firstAttack < bug[second].dex)
            {
                UpdateBattleLog("Misses with a Dash attack! [" + firstAttackRoll + "] " + ModifierAsText(bug[first].dexMod) + " [Dex]");
                UpdateBattleLog(bug[second].name + " is unharmed.");
            }
        }
        Debug.Log("---------------------------------------------------");
    }

    public class Bug
    {
        public string name;
        public string type;
        public int str;
        public int strMod;
        public int con;
        public int conMod;
        public int dex;
        public int dexMod;
        public int dmg;
        public int hp;
        public int spd;
        public bool eff;
    }

    void SubtractHP(int first, int second, int firstFullDamage)
    {
        bug[second].hp = bug[second].hp - firstFullDamage;
        if (0 >= bug[second].hp)
        {
            bug[second].hp = 0;
            deadBug = bug[second].name;
            winnerBug = bug[first].name;
        }
    }
    

    int CheckDamage(int bugSTR)
    {
        int damage = 0;
        if(IsBetween(bugSTR, 1, 4))
        {
            damage = 4;
        }
        if (IsBetween(bugSTR, 5, 8))
        {
            damage = 6;
        }
        if (IsBetween(bugSTR, 9, 12))
        {
            damage = 8;
        }
        if (IsBetween(bugSTR, 13, 16))
        {
            damage = 10;
        }
        if (IsBetween(bugSTR, 17, 20))
        {
            damage = 12;
        }
        return damage;
    }

    int CheckHP(int bugCON)
    {
        int hp = 15;
        if (bugCON == 20)
        {
            hp = 20;
        }
        if (IsBetween(bugCON, 18, 19))
        {
            hp = 19;
        }
        if (IsBetween(bugCON, 16, 17))
        {
            hp = 18;
        }
        if (IsBetween(bugCON, 14, 15))
        {
            hp = 17;
        }
        if (IsBetween(bugCON, 12, 13))
        {
            hp = 16;
        }
        if (IsBetween(bugCON, 7, 8))
        {
            hp = 14;
        }
        if (IsBetween(bugCON, 5, 6))
        {
            hp = 13;
        }
        if (IsBetween(bugCON, 3, 4))
        {
            hp = 12;
        }
        if (IsBetween(bugCON, 1, 2))
        {
            hp = 11;
        }
        return hp;
    }

    int CheckSpeed(int bugDEX)
    {
        int speed = 0;
        if(bugDEX < 10)
        {
            speed -= (10 - bugDEX);
        }
        if(bugDEX > 10)
        {
            speed += (bugDEX - 10);
        }
        return speed;
    }

    string CheckType(int bugType)
    {
        // 0 = Earth, 1 = Water and 2 = Wind
        string typeString = "Not Set";
        if (bugType == 0)
        {
            typeString = "Earth";
        }
        if (bugType == 1)
        {
            typeString = "Water";
        }
        if (bugType == 2)
        {
            typeString = "Wind";
        }
        return typeString;
    }

    bool CheckEffective(int bugType, int opponentBugType)
    {
        bool effective = false;

        if(bugType == 0 && opponentBugType == 1)
        {
            effective = true;
        }
        if (bugType == 1 && opponentBugType == 2)
        {
            effective = true;
        }
        if (bugType == 2 && opponentBugType == 0)
        {
            effective = true;
        }

        return effective;
    }

    int CheckModifier(int typeValue)
    {
        int modifier = 0;
        if (typeValue >= 20)
        {
            modifier = 5;
        }
        if (IsBetween(typeValue, 18, 19))
        {
            modifier = 4;
        }
        if (IsBetween(typeValue, 16, 17))
        {
            modifier = 3;
        }
        if (IsBetween(typeValue, 14, 15))
        {
            modifier = 2;
        }
        if (IsBetween(typeValue, 12, 13))
        {
            modifier = 1;
        }
        if (IsBetween(typeValue, 10, 11))
        {
            modifier = 0;
        }
        if (IsBetween(typeValue, 8, 9))
        {
            modifier = -1;
        }
        if (IsBetween(typeValue, 6, 7))
        {
            modifier = -2;
        }
        if (IsBetween(typeValue, 4, 5))
        {
            modifier = -3;
        }
        if (IsBetween(typeValue, 2, 3))
        {
            modifier = -4;
        }
        if (IsBetween(typeValue, 0, 1))
        {
            modifier = -5;
        }
        return modifier;
    }

    string ModifierAsText(int modValue)
    {
        string asText = "+0";
        if(modValue > 0)
        {
            asText = "+" + modValue;
        }
        if(modValue < 0)
        {
            asText = modValue.ToString();
        }
        return asText;
    }

    void UpdateBattleLog(string newInput)
    {
        battleLog += "\n" + newInput;
        battleLogTextField.text = battleLog;
    }

    void AddSpaceBattleLog()
    {
        battleLog += "\n";
        battleLogTextField.text = battleLog;
    }

    bool IsBetween(int testValue, int bound1, int bound2)
    {
        return (testValue >= Mathf.Min(bound1, bound2) && testValue <= Mathf.Max(bound1, bound2));
    }
}