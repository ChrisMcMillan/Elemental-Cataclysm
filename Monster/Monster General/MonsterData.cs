using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls all data about a monster and modifications to that data.
public class MonsterData {

    public const int MAX_ABILITIES = 3;
    public enum MonsterIdentity {VenomHound ,FlameArchon, IceWraith, Griffin};

    object _identity;
    protected Elemental.ElementalIdentity _monsterElementalType;

    bool _isEnemyMonster;
    bool _isDead;
    int _healingReductionStacks;
    int _selfImolationBuffStacks;
    float _curStunResistance;
    float _baseStunResistance;

    string _name;
    int _health;
    int _maxHealth;

    int _level;
    int _exp;
    int _maxExp;

    int _baseStrength;
    int _baseAgility;
    int _baseSpirit;
    int _baseSpeed;

    int _curStrength;
    int _curAgility;
    int _curSpirit;
    int _curSpeed;

    protected Ability[] _abilities;

    List<StatusAilment> _ailments;

    MonsterAnimator _myMosAnimatorRef;

    public MonsterAnimator MyMosAnimatorRef {
        get {
            return _myMosAnimatorRef;
        }

        set {
            _myMosAnimatorRef = value;
            if (_myMosAnimatorRef != null) {
                InitHealthBar();
                _myMosAnimatorRef.ailementDisplay.UpdateStatusAilmentDisplayIcons(_ailments);
            }
        }
    }

    public Ability[] Abilities {
        get {
            return _abilities;
        }
    }

    public bool IsDead {
        get {
            return _isDead;
        }
    }

    public object Identity {
        get {
            return _identity;
        }
    }

    public int MaxHealth {
        get {
            return _maxHealth;
        }
    }

    public int Health {
        get {
            return _health;
        }
    }

    public Elemental.ElementalIdentity MonsterElementalType {
        get {
            return _monsterElementalType;
        }
    }

    public int Level {
        get {
            return _level;
        }
    }

    public int CurStrength {
        get {
            return _curStrength;
        }
    }

    public int CurAgility {
        get {
            return _curAgility;
        }
    }

    public int CurSpirit {
        get {
            return _curSpirit;
        }
    }

    public int CurSpeed {
        get {
            return _curSpeed;
        }
    }

    public string Name {
        get {
            return _name;
        }

        set {
            _name = value;
        }
    }

    public int SelfImolationBuffStacks {
        get {
            return _selfImolationBuffStacks;
        }

        set {
            _selfImolationBuffStacks = value;
        }
    }

    public int HealingReductionStacks {
        get {
            return _healingReductionStacks;
        }
    }

    public float CurStunResistance {
        get {
            return _curStunResistance;
        }
    }

    public bool IsEnemyMonster {
        get {
            return _isEnemyMonster;
        }
    }

    //Creates a string base on a monster's identiy to be use in combat messages.
    public static string GetMonsterIdentityString(object arg) {
        string monsterIdentityString = arg.ToString();
        string temp = "";
        int count = 0;

        foreach (char c in monsterIdentityString) {          
            if (char.IsUpper(c) && count > 0) temp += " ";
            
            temp += c;
            count++;
        }

        return temp;
    }

    //Returns a random monster identiy from a list of all monster identities.
    public static MonsterIdentity PickRandomMonsterIdenity() {

        int RNG;
        List<MonsterIdentity> monsterIdenityList = new List<MonsterIdentity>();

        foreach (var value in MonsterIdentity.GetValues(typeof(MonsterIdentity))) {
           
            monsterIdenityList.Add((MonsterIdentity)value); 
        }

        RNG = UnityEngine.Random.Range(0, monsterIdenityList.Count);

        return monsterIdenityList[RNG];
    }

    //Picks a random monster of a certain element. 
    public static MonsterIdentity PickRandomMonsterIdenityOfElement(Elemental.ElementalIdentity arg) {
        List<MonsterIdentity> monsterIdenityList = new List<MonsterIdentity>();
        int RNG;

        foreach (MonsterIdentity value in MonsterIdentity.GetValues(typeof(MonsterIdentity))) {
            if (FindMonsterElementalType(value) == arg) {
                monsterIdenityList.Add(value);
            }
        }

        RNG = UnityEngine.Random.Range(0, monsterIdenityList.Count);

        return monsterIdenityList[RNG];
    }

    //Finds the element type associated with an monster identity. 
    public static Elemental.ElementalIdentity FindMonsterElementalType(object arg) {

        string errorMessage = "Error: " + arg + " has not been assigned a elemental type.";

        switch ((MonsterIdentity)arg) {
            case MonsterIdentity.VenomHound: return Elemental.ElementalIdentity.Earth;
            case MonsterIdentity.FlameArchon: return Elemental.ElementalIdentity.Fire;
            case MonsterIdentity.IceWraith: return Elemental.ElementalIdentity.Water;
            case MonsterIdentity.Griffin: return Elemental.ElementalIdentity.Wind;
            default: Debug.LogError(errorMessage); return Elemental.ElementalIdentity.NONE;
        }
    }

    public void InitData(object arg, bool gainWisdom = false, bool isEnemyMonsterArg = false){
        _identity = arg;
        _isEnemyMonster = isEnemyMonsterArg;
        _name = RadomNameGenerator.RandomNames[UnityEngine.Random.Range(0,RadomNameGenerator.RandomNames.Count)];
        _isDead = false;
        _abilities = new Ability[MAX_ABILITIES];
        _ailments = new List<StatusAilment>();
        _level = 1;
        CaculateMaxExp();
        _exp = 0;
        _healingReductionStacks = 0;
        _baseStunResistance = 0.25f;
        _curStunResistance = _baseStunResistance;

        SetBaseIdentityAttributes(_identity);

        if (arg.GetType() == typeof(MonsterIdentity)) _monsterElementalType = FindMonsterElementalType(_identity);
        else _monsterElementalType = UniqueBossData.FindUniqueBosElementalType(_identity);

        if (gainWisdom == true) GainWisdom(_monsterElementalType);
        else PickMonsterAbilities();
    }

    //Set a monster's base attributes base on its identity. 
    protected virtual void SetBaseIdentityAttributes(object arg) {
        //stregth: 5, agility: 5, spirit: 5, speed: 5;
        switch ((MonsterIdentity)arg) {
            case MonsterIdentity.VenomHound: ApplyAttributes(3, 7, 3, 7); break;
            case MonsterIdentity.FlameArchon: ApplyAttributes(7, 3, 7, 3); break;
            case MonsterIdentity.IceWraith: ApplyAttributes(2, 5, 9, 4); break;
            case MonsterIdentity.Griffin: ApplyAttributes(8, 2, 2, 8); break;
            default: Debug.LogError("Error: " + arg + " has not been assigned attributes."); break;
        }
    }

    //Set a monster's base attributes to the agruments. 
    protected void ApplyAttributes(int strength,int agility,int spirit,int speed) {
        
        _baseStrength = strength;
        _baseAgility = agility;
        _baseSpirit = spirit;
        _baseSpeed = speed;

        CaculateMaxHealth();
        _health = _maxHealth;

        ResetAttributes();
    }

    /*Gives a monster random abilities with no repeats. Makes sure monsters get at least one
     offensive ability so they are always able to deal damage to another monster.*/
    protected virtual void PickMonsterAbilities() {
        List<Ability> poissibleAbilitiesList = new List<Ability>();
        List<Ability> offsiveAbilitesList = new List<Ability>();
        Ability newAbility;
        Ability offsiveAbility;
        int RNG;

        foreach (Ability.AbilityIdentity item in Ability.AbilityIdentity.GetValues(typeof(Ability.AbilityIdentity))) {
            newAbility = Ability.ObatainAbilityData(item);
            poissibleAbilitiesList.Add(newAbility);
            if (newAbility.IsOffensiveAbility) offsiveAbilitesList.Add(newAbility);
        }
        
        //Need to make sure we always have at lest 1 offsive ability
        RNG = UnityEngine.Random.Range(0, offsiveAbilitesList.Count);
        offsiveAbility = offsiveAbilitesList[RNG];
        _abilities[0] = offsiveAbility;
        //_abilities[0] = Ability.ObatainAbilityData(Ability.AbilityIdentity.Stun_Breaker);
        poissibleAbilitiesList.Remove(offsiveAbility);
        offsiveAbilitesList.Remove(offsiveAbility);

        for (int i = 1; i < MAX_ABILITIES; i++) {
            RNG = UnityEngine.Random.Range(0, poissibleAbilitiesList.Count);
            _abilities[i] = poissibleAbilitiesList[RNG];
            poissibleAbilitiesList.RemoveAt(RNG);
        }
    }

    /*Turns an enemy monster into a boss by giving them health bonus, attributes bonus and
    increases their size.*/
    public void BigBossUpgrade() {

        float dugeonCompletionPercent = GameManager.instance.DugeonData.ObtainDungeonCompletionPercent();
        float addtionalCurveHealthBonus = 1.0f * dugeonCompletionPercent;
        float bossHealthBonus = 2.0f + addtionalCurveHealthBonus;

        Debug.Log("Boss addtionalCurveHealthBonus: " + addtionalCurveHealthBonus);
        _maxHealth = Mathf.RoundToInt(_maxHealth * bossHealthBonus);
        _health = _maxHealth;
        int attributeBuff = _level;

        Debug.Log("Boss attributeBuff: " + attributeBuff);
        _baseStrength += attributeBuff;
        _baseAgility += attributeBuff;
        _baseSpirit += attributeBuff;
        _baseSpeed += attributeBuff;

        InitHealthBar();
        _myMosAnimatorRef.ChangeSkeletonSize(1.2f);
    }

    /*Set the attributes counts used during combat back to
     the base values. We have special abilities that temporarily change
     attributes counts and want to reset those after combat or after their
     duration is up.*/
    public void ResetAttributes() {
        _curStrength = _baseStrength;
        _curAgility = _baseAgility;
        _curSpeed = _baseSpeed;
        _curSpirit = _baseSpirit;
    }

    /*Grants temporary attribute bonus to a monster when they use certain 
    abilities.*/
    public string GrantTempAttributeBonus(Ability ability) {
        if (CheckForMatchingAilment(ability.AbilityAlimentType)) return "";

        int attributeBonus;
        string annocementMessage = "+";

        if (_level <= 3) attributeBonus = 3;
        else attributeBonus = _level;

        annocementMessage += attributeBonus.ToString() + " ";

        switch (ability.AbilityElmentalType) {
            case Elemental.ElementalIdentity.Fire: _curStrength += attributeBonus; annocementMessage += "Strength"; break;
            case Elemental.ElementalIdentity.Water: _curSpirit += attributeBonus; annocementMessage += "Spirt"; break;
            case Elemental.ElementalIdentity.Wind: _curSpeed += attributeBonus; annocementMessage += "Speed"; break;
            case Elemental.ElementalIdentity.Earth: _curAgility += attributeBonus; annocementMessage += "Agility"; break;
            default: Debug.LogError("Error: Attribute bonus not define for " + ability.AbilityElmentalType); break;
        }

        return annocementMessage;
    }

    /*Used after combat to reset monster attributes.*/
    public void SoftReset() {
        _myMosAnimatorRef = null;
        _healingReductionStacks = 0;
        _selfImolationBuffStacks = 0;
        _curStunResistance = _baseStunResistance;
        ResetAttributes();
        RemoveAllAilmentEffects();
    }

    /*Rewards a monster exp points. If the monster get more exp
     then their max exp, then they level up. The extra exp is store
     in overExp. overExp is then pass into RewardEXP() in a 
     recursive call.*/
    public void RewardEXP(int expReward) {
        int overExp;
        _exp += expReward;

        if(_exp < _maxExp) {
            return;
        }
        else {
            overExp = _exp - _maxExp;
            LevelUp();
            RewardEXP(overExp);
        }
    }

    /*Used to manually level up a monster.*/
    public void ControlledLevelUp(int newLevel) {
        int levelDifferent = newLevel - _level;

        if (levelDifferent <= 0) return;

        for (int i = 0; i < levelDifferent; i++) {
            LevelUp();
        }
    }

    void CaculateMaxExp() {
        _maxExp = _level * 2;
    }

    void CaculateMaxHealth() {
        _maxHealth = 70 + (_level * 30) + (_baseStrength * 3);
        //Debug.Log(_identity + " Level: " + _level + " Max Health: " + _maxHealth);
    }

    /*Levels up a monster. Depending on their elemental type, they will bonus
     attribute point for a particular attribute. For exapmple, if they are a fire
     type monster, then they will get 3 points to strength and 2 to everything else.*/
    public void LevelUp() {

        int matchBonus = 3;
        int noMatchBonus = 2;

        _level++;
        CaculateMaxExp();
        _exp = 0;

        _baseStrength += (_monsterElementalType == Elemental.ElementalIdentity.Fire) ? matchBonus : noMatchBonus;
        _baseAgility += (_monsterElementalType == Elemental.ElementalIdentity.Earth) ? matchBonus : noMatchBonus;
        _baseSpirit += (_monsterElementalType == Elemental.ElementalIdentity.Water) ? matchBonus : noMatchBonus;
        _baseSpeed += (_monsterElementalType == Elemental.ElementalIdentity.Wind) ? matchBonus : noMatchBonus;

        CaculateMaxHealth();
        _health = _maxHealth;
        ResetAttributes();
    }

    /*Check if there ailment of the same type. If there is, it replaces it
     with the new ailment. Else, it just add to the monster's ailment list.*/
    public void AddStatusAilment(StatusAilment newAilment) {

        bool foundMatchingAilment = false;

        if(newAilment.AlimentType == StatusAilment.StatusAilmentType.Stun) {
            if (RollStunChance() == false) return;
        }

        for (int i = 0; i < _ailments.Count; i++) {
            if(_ailments[i].AlimentType == newAilment.AlimentType) {
                _ailments[i] = newAilment;
                foundMatchingAilment = true;
                break;
            }
        }

        if (foundMatchingAilment == false) _ailments.Add(newAilment);

        if (_myMosAnimatorRef != null) _myMosAnimatorRef.ailementDisplay.UpdateStatusAilmentDisplayIcons(_ailments);
    }

    /*Decide if a monster will be stuned by a stun ability. The more a monster
     has beeen stuned in a battle, the harder is to stun them again.*/
    bool RollStunChance() {
        float RNG = UnityEngine.Random.Range(0.0f, 1.0f);
        CombatAnnouncementText.AnnouncementTextInfo temp;

        if (RNG > _curStunResistance) {
            //Debug.Log("Stun applied RNG: " + RNG.ToString("F2") + " _curStunResistance: " + _curStunResistance);
            _curStunResistance += 0.25f;
            return true;
        }
        
        if(_myMosAnimatorRef != null) {
            temp = new CombatAnnouncementText.AnnouncementTextInfo("Stun resisted", Color.white);
            _myMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
        }

        //Debug.Log("Stun failed RNG: " + RNG.ToString("F2") + " _curStunResistance: " + _curStunResistance);
        return false;
    }

    public bool CheckForMatchingAilment(StatusAilment.StatusAilmentType newAilmentType) {
        for (int i = 0; i < _ailments.Count; i++) {
            if (_ailments[i].AlimentType == newAilmentType) return true;
        }

        return false;
    }

    /*Loop through the monster's ailment list and applies their effects. If an 
     ailment has used up all their stacks, then it is removed from the monster's ailment list.*/
    public bool AppplyAilmentEffects() {

        if (_ailments.Count == 0) return false;

        bool haveDamagingAliment = false;
        int counter = _ailments.Count - 1;

        while(counter >= 0) {

            if(_ailments.Count > 0) {

                if (_ailments[counter].AlimentType == StatusAilment.StatusAilmentType.Burning ||
                 _ailments[counter].AlimentType == StatusAilment.StatusAilmentType.Poison ||
                 _ailments[counter].AlimentType == StatusAilment.StatusAilmentType.Stun) {
                    haveDamagingAliment = true;
                }

                if (_ailments[counter].ApplyEffect(this)) {
                    if (_ailments.Count > 0)  _ailments.RemoveAt(counter);
                }
            }

            counter--;
        }

        if (_myMosAnimatorRef != null) _myMosAnimatorRef.ailementDisplay.UpdateStatusAilmentDisplayIcons(_ailments);

        /*If we have a damaging aliment, then we want the monster to play
         the hit animation.*/
        if (haveDamagingAliment) {
            DecideDamageAnimation();
            return true;
        }

        return false;
    }

    //Remove all alilments from the monster.
    public int RemoveAllAilmentEffects() {
        int count = _ailments.Count - 1;
        int removedAilemntsCount = 0;

        while (count >= 0) {       
            _ailments.RemoveAt(count);
            removedAilemntsCount++;
            count--;
        }

        ResetAttributes();
        if (_myMosAnimatorRef != null) _myMosAnimatorRef.ailementDisplay.UpdateStatusAilmentDisplayIcons(_ailments);

        return removedAilemntsCount;
    }

    /*We use the monster's speed to decide if they will attack first or not. 
     We use random number genertion so monsters with higher speed tend to attack 
     frist but not always. This maintains a degree of predictability but still want
     to use random number generation to keep it interesting. Note: We probaly 
     want to make it so speedVariance scales with floor level.*/
    public int ObatainSpeedRoll(){
        int speedRoll;
        int speedVariance = 3;

        speedRoll = UnityEngine.Random.Range(_curSpeed - speedVariance, _curSpeed + speedVariance + 1);
        if (speedRoll < 1) speedRoll = 1;
        return speedRoll;
    }

    /*This function decides if an ability will have a critical effect and how
     much bonus effect it will have. If a critical effect fails to happen, then this
     fucntion will return 0.0f to indicate so.*/
    public float ObatainCriMod() {
        float criMod = 0.0f;
        int chanceToCri = 5 + _curAgility;
        int RNG = UnityEngine.Random.Range(1, 101);

        if(RNG <= chanceToCri) {
            //Debug.Log("Critacal! RNG: " + RNG + " chanceToCri: " + chanceToCri);
            criMod = 1.1f + (_curSpirit * 0.05f);
        }

        return criMod;
    }

    //Used during beast master scene to determine how much a monster should cost. 
    public int FindMonsterRuneCost() {
        return Mathf.CeilToInt((float)_level / 2.0f);
    }

    /*All damage to the monster is filtered through this function so the _health
     value does not become a negative value. Negative values can lead to the monster's 
     health bar being rendered backwards.*/
    public void SubatractHealth(int arg) {
        _health -= arg;

        if(_health <= 0) {
            RemoveAllAilmentEffects();
            _health = 0;
            _isDead = true;
        }

        if (_myMosAnimatorRef != null) _myMosAnimatorRef.healthSlider.value = _health;
    }

    /*If the monster is dead, then we want to play death animation. Otherwise,
     we want to play the hit animation.*/ 
    void DecideDamageAnimation() {
        if(_isDead) _myMosAnimatorRef.MyAnimatorRef.SetTrigger("Death");
        else _myMosAnimatorRef.MyAnimatorRef.SetTrigger("Hit");
    }

    public void TakeDamage(int damageAmount, Ability abilityData, float elementalDamageMod = 1.0f){

        SpeicalTakeDamgeEvent(abilityData);

        SubatractHealth(damageAmount);

        DecideDamageAnimation();

        PlayHealOrDamageText(damageAmount, "-", elementalDamageMod);
    }

    //Some unique bosses trigger special events when they take damage.
    protected virtual void SpeicalTakeDamgeEvent(Ability abilityData) {

    }

    //Some unique bosses trigger special events before they use an ability.
    public virtual float PreAbilityUseEvent(MonsterData user, MonsterData target, Ability.AbilityEffectData effectData) {
        return effectData.modifiedEffect;
    }

    //Some unique bosses trigger special events after they use an ability.
    public virtual void AfterAbilityUseEvent() {

    }

    //Called when a monster uses a healing ability.
    public void IncramentHealingReductionStack() {
        _healingReductionStacks++;
    }

    /*All healing to the monster is filtered through this function. This function makes sure that monster's health
     does not become greater then their max health. This function also applies the healing reduction effect. Whem a monster
     uses a healing ability they gain a healing reduction stack. The more stacks the monster has, the less effective healing will
     be on them. This is ment to prevent battles from going on for too long. This mechanic does not apply to healing over time effects.*/
    public void ApplyHealing(int healingAmount,bool playStandardText = true, float elementalDamageMod = 1.0f,bool regenHealing = false) {
        float healingReduciton;

        if(_healingReductionStacks > 0 && regenHealing == false) {
            //Debug.Log("Healing before reduction: " + healingAmount);
            healingReduciton = _healingReductionStacks * 0.2f;
            healingAmount -= Mathf.RoundToInt(healingAmount * healingReduciton);
            //Debug.Log("Healing after reduction: " + healingAmount);
        }

        if (healingAmount < 0) healingAmount = 0;
        _health += healingAmount;
        if (_health > _maxHealth) _health = _maxHealth;

        if(playStandardText) PlayHealOrDamageText(healingAmount, "+", elementalDamageMod);
        if (_myMosAnimatorRef != null) _myMosAnimatorRef.healthSlider.value = _health;
    }

    /*Depending what kind of effect an ability deals, this function will display a different colored text.
     Green for a healing effect or red for a damage effect.*/
    void PlayHealOrDamageText(int effectAmount, string effectStringMod, float elementalDamageMod = 1.0f) {
        if (_myMosAnimatorRef == null) return;

        CombatAnnouncementText.AnnouncementTextInfo temp;
        Color textColor;
        string message;

        message = Elemental.FindElementalModAnnouncementTextInfo(elementalDamageMod) + " "+ effectStringMod + effectAmount.ToString();

        if (effectStringMod == "+") textColor = Color.green;
        else if (effectStringMod == "-") textColor = Color.red;
        else textColor = Color.white;

        temp = new CombatAnnouncementText.AnnouncementTextInfo(message , textColor);
        _myMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
    }

    /*This function has the monster play a differnt animation depending on if
     the ability is a attack or a spell.*/
    public void PlayAbilityAnimation(bool isSpell, Ability abilityData){
        if (isSpell) _myMosAnimatorRef.MyAnimatorRef.SetTrigger("Cast");
        else _myMosAnimatorRef.MyAnimatorRef.SetTrigger("Attack");

        CombatAnnouncementText.AnnouncementTextInfo temp =
            new CombatAnnouncementText.AnnouncementTextInfo(Ability.GetAbilityIdentityString(abilityData.MyAbilityIdentity), 
            Elemental.FindElementalColor(abilityData.AbilityElmentalType));

        _myMosAnimatorRef.combatAnnouncementText.AddToAnnouncementQueue(temp);
    }

    void InitHealthBar() {
        _myMosAnimatorRef.healthSlider.maxValue = _maxHealth;
        _myMosAnimatorRef.healthSlider.value = _health;
    }

    //Updates a monster panel to display this monster's data.
    public void UpdateMonsterInfoPanel(MonsterInfoPanel arg) {
        arg.monsterPicture.sprite = GameManager.instance.GameResorcesRef.FindMonsterIcon((MonsterIdentity)_identity);
        arg.monsterName.text = _name;
        arg.monsterLevel.text = "Level: " + _level;
        arg.healthSlider.maxValue = _maxHealth;
        arg.healthSlider.value = _health;
        arg.expSlider.maxValue = _maxExp;
        arg.expSlider.value = _exp;
        arg.strText.text = "Str: " + _baseStrength;
        arg.agiText.text = "Agi: " + _baseAgility;
        arg.spriText.text = "Spri: " + _baseSpirit;
        arg.speedText.text = "Speed: " + _baseSpeed;
        arg.panelBackgroundImage.sprite = GameManager.instance.GameResorcesRef.FindMonsterInfoPanelBackgroundImage(_monsterElementalType);

        for (int i = 0; i < _abilities.Length; i++) {
            arg.abilityIcons[i].sprite = GameManager.instance.GameResorcesRef.FindAbilityIcons((Ability.AbilityIdentity)_abilities[i].MyAbilityIdentity);
            arg.abilityIcons[i].gameObject.SetActive(true);
        }
    }

    /*Update a combat button to display a monster's health and abilities.*/
    public void UpdateCombatButton(CombatButton arg) {
        arg.monsterName.text = _name;
        arg.monsterName.gameObject.SetActive(true);

        arg.monsterHealthSilder.maxValue = _maxHealth;
        arg.monsterHealthSilder.value = _health;
        arg.monsterHealthSilder.gameObject.SetActive(true);

        arg.ImageRef.sprite = GameManager.instance.GameResorcesRef.FindMonsterIcon((MonsterIdentity)_identity);

        for (int i = 0; i < _abilities.Length; i++) {
            arg.abilityIcons[i].sprite = GameManager.instance.GameResorcesRef.FindAbilityIcons((Ability.AbilityIdentity)_abilities[i].MyAbilityIdentity);
            arg.abilityIcons[i].gameObject.SetActive(true);
        }
    }

    /*Gives a monster two random abilities of a specific elemental type and one random one
    of any elemental type, with no repeats.*/
    public void GainWisdom(Elemental.ElementalIdentity roomElement) {
        List<Ability> allPossibleAbilites = new List<Ability>();
        List<Ability> abilitesOfRoomElement = new List<Ability>();
        List<Ability> newAbilites = new List<Ability>();
        Ability abilityTemp;
        int RNG;

        foreach (Ability.AbilityIdentity item in Ability.AbilityIdentity.GetValues(typeof(Ability.AbilityIdentity))) {
            abilityTemp = Ability.ObatainAbilityData(item);
            allPossibleAbilites.Add(abilityTemp);
            if (abilityTemp.AbilityElmentalType == roomElement) abilitesOfRoomElement.Add(abilityTemp);
        }

        for (int i = 0; i < MAX_ABILITIES - 1; i++) {
            RNG = UnityEngine.Random.Range(0, abilitesOfRoomElement.Count);
            abilityTemp = abilitesOfRoomElement[RNG];
            newAbilites.Add(abilityTemp);
            allPossibleAbilites.Remove(abilityTemp);
            abilitesOfRoomElement.Remove(abilityTemp);
        }

        RNG = UnityEngine.Random.Range(0, allPossibleAbilites.Count);
        newAbilites.Add(allPossibleAbilites[RNG]);

        for (int i = 0; i < newAbilites.Count; i++) {
            _abilities[i] = newAbilites[i];
        }
    }

    //Pick a set of 3 abilities of a specific elemental type for the monster.
    public void PickRandomAbilitiesOfElement(Elemental.ElementalIdentity arg) {
        List<Ability> possibleAbilities = new List<Ability>();
        List<Ability> offensiveAbilites = new List<Ability>();
        List<Ability> newAbilites = new List<Ability>();
        Ability temp;
        int RNG;

        foreach (Ability.AbilityIdentity item in Ability.AbilityIdentity.GetValues(typeof(Ability.AbilityIdentity))) {
            temp = Ability.ObatainAbilityData(item);
            if(temp.AbilityElmentalType == arg) {
                possibleAbilities.Add(temp);
                if (temp.IsOffensiveAbility == true) offensiveAbilites.Add(temp);
            }
        }

        RNG = UnityEngine.Random.Range(0, offensiveAbilites.Count);
        temp = offensiveAbilites[RNG];
        newAbilites.Add(temp);
        possibleAbilities.Remove(temp);

        for (int i = 0; i < MAX_ABILITIES - 1; i++) {
            RNG = UnityEngine.Random.Range(0, possibleAbilities.Count);
            temp = possibleAbilities[RNG];
            newAbilites.Add(temp);
            possibleAbilities.RemoveAt(RNG);
        }

        for (int k = 0; k < newAbilites.Count; k++) {
            _abilities[k] = newAbilites[k];
        }
    }

    /*Gives the monster a permanent attribute bonus base on the elemental type of the room.*/
    public void GainPower(Elemental.ElementalIdentity roomElement) {
        int floorLevel = GameManager.instance.DugeonData.CurrentFloor.FloorLevel;
        int statRoll = UnityEngine.Random.Range(floorLevel - 2, floorLevel + 3);
        if (statRoll < 1) statRoll = 1;

        switch (roomElement) {
            case Elemental.ElementalIdentity.Earth: _baseAgility += statRoll; break;
            case Elemental.ElementalIdentity.Fire: _baseStrength += statRoll; break;
            case Elemental.ElementalIdentity.Water: _baseSpirit += statRoll; break;
            case Elemental.ElementalIdentity.Wind: _baseSpeed += statRoll; break;
            default: Debug.LogError("Error: Stat Bonus not define for " + roomElement); break;
        }

        ResetAttributes();
        CaculateMaxHealth();
    }
}
