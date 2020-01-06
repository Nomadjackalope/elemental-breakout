using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;


public class Hex : MonoBehaviour {
    private HexData hexData = new HexData();
    bool dying = false;

    public GameObject hexBreakAudio;

    public GameObject hexExplodeAnimPrefab;

    public GameObject essenceDrop;

    public SpriteRenderer hexShadow;

    // public GameObject vineOverlay;
    // public GameObject vineInstance;

    public bool kill = false;
    public bool hurt = false;

    bool earthKillOnContact = false;

    private GameObject lastBallToContact = null;

    // Earth effect
    public PhysicsMaterial2D earthPhysicsMaterial;

    // Growth effect
    Hex vineOriginHex;
    bool isVined = false;
    float vineChance = 1f; // 0 to 1;
    private List<Hex> prevVineLayer;
    int vineLayer = 0;
    int currentVineLayer = 0;

    public int pointValue = 50;
    public bool countTowardHexesDestroyed = true;

    private bool isDamagable = true;
    private bool hitPaddle = false;

    private bool timerDestroyed = false;

    private bool isPoisoned = false;
    public bool spinningBall = false;

    private bool readyToWormHole;
    public bool ignoreWormHole;

    // Audio
    public AudioClip onBreak;
    public AudioClip onDamage;
    public AudioClip waterSpinSound;

    // Coroutines
    Coroutine poisonDeath;
    Coroutine autoDeath;
    Coroutine dyingAnimation;

    // Use this for initialization
    void Start () {
       GameManage.instance.HexAutoEndEvent.AddListener(beginAutoDeath);
       GameManage.instance.GameFailedEvent.AddListener(stopAutoDeath);
	}
	
	// Update is called once per frame
	void Update () {
		if(kill) {
            removeHealth(hexData.health);
        }

        if(hurt) {
            hurt = false;
            if(hexData.hexType == DataTypes.HexType.Growth) {
                grow(vineChance, this);
            }
            removeHealth(1);

        }
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if(collision.collider.CompareTag("Ball")) {
            isDamagable = true;

            // Always do these
            int curHitCount = GameManage.instance.addBallHit();
            lastBallToContact = collision.collider.gameObject;
            GameManage.instance.removeShadow(transform.position);

            Ball ball = lastBallToContact.GetComponent<Ball>();


			if(ball.isPoisoned && hexData.hexType != DataTypes.HexType.Water) {
				poison(0);
			}

            if(isPoisoned && hexData.hexType != DataTypes.HexType.Earth) {
                isDamagable = false;
            }
            
            if(GameManage.instance.getNextHitSwitchHexFire() && hexData.hexType != DataTypes.HexType.Fire) {
                GameManage.instance.setNextHitSwitchHexFire(false);

                switchHexType(DataTypes.HexType.Fire);

                isDamagable = false;
            }

            if(GameManage.instance.getNextHitSwitchHexGrowth() && hexData.hexType != DataTypes.HexType.Growth) {
                GameManage.instance.setNextHitSwitchHexGrowth(false);

                switchHexType(DataTypes.HexType.Growth);

                isDamagable = false;
            }
            
            if(hexData.hexType == DataTypes.HexType.Poison) {
                lastBallToContact.GetComponent<Ball>().poison();
            }
             
            if(hexData.hexType == DataTypes.HexType.Growth) {
                grow(vineChance, this);
            }
            
            if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.EarthHeavyBall)) { 
                ball.passThroughStrengthLeft--;
				if(ball.passThroughStrengthLeft >= 0) {
                    removeHealth(hexData.health);
					ball.GetComponent<Rigidbody2D>().velocity = ball.prevVelocity * 0.8f;
                    isDamagable = false;
                    if(ball.passThroughStrengthLeft > 0) {
    				    GameManage.TriggerEvent(PowerUps.EarthHeavyBall, PowerUpState.Active, ball.passThroughStrengthLeft);
                    } else {
    				    GameManage.TriggerEvent(PowerUps.EarthHeavyBall, PowerUpState.CoolDown, 0);
                    }
				} else {
    				GameManage.TriggerEvent(PowerUps.EarthHeavyBall, PowerUpState.CoolDown, 0);
                }
            } 
            
            if(isDamagable) {
                int damage = 1;

                if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.FireBallDamageUp1)) {
                    damage++;
                }

                if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.FireBallDamageUp2)) {
                    damage++;
                }

                if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.PoisonBallDamageUp)) {
                    damage++;
                }

                removeHealth(damage);
            }
            

            if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.EarthQuakingHit)) {
                //&& GameManage.instance.QuakingHitAvailable()
                //GameManage.instance.quakingHit(
                    quakingHit(ball);
            }

            if(ball.isShadowBall) {
                ball.age++;

                if(ball.age >= ball.getShadowLifetime()) {
			        ball.onDeath();
		        }
            }

            if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.ShadowWormHole) && Hex.isElement(getHexType()) && hexData.health <= 1 && !ignoreWormHole) {
                // if(getHexType() == DataTypes.HexType.Earth) {
                    GameManage.instance.wormHoleBall(gameObject, lastBallToContact);
                // } else {
                //     lastBallToContactVelocity = lastBallToContact.GetComponent<Rigidbody2D>().velocity;
                //     lastBallToContact.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                // }
            }

            // Don't give these bonus points if the hex is dying
            if(getHealth() >= 1) {
                GameManage.instance.hexHit(transform.position);
            }
        }
	}

    

    public void poison(int attemptNumber) {
        attemptNumber++;

        if(!isPoisoned) {
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0.5f);
            poisonDeath = StartCoroutine(onDeathAfterPoison(3));//StartCoroutine(killAfterTime(false, 3));
            isPoisoned = true;
        } else if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.PoisonPoisonSpread)) {
            // don't do earth because it will be falling and grid.CellToWorld gives a stack overflow.
            if(hexData.hexType != DataTypes.HexType.Earth && attemptNumber <= 6) {
                spreadPoison(attemptNumber);
            }
        }
    }

    IEnumerator onDeathAfterPoison(float seconds) {
        yield return new WaitForSeconds(seconds);

        onDeath();

    }

    void switchHexType(DataTypes.HexType hexType) {
        hexData.hexType = hexType;
        gameObject.GetComponent<HexBuilder>().needsUpdate = true;
        if(hexType == DataTypes.HexType.Growth) {
            setHealth(3);
        }
    }

    public void removeHealth(int damage) {
        setHealth(getHealth() - damage);

        if(hexData.health >= 0) {
            GameManage.instance.increaseCurrentCombo();
        }

        if(isVined) {
            if(vineOriginHex != null) {
                vineOriginHex.removeHealth(1);
            }
        }        

        // just kill for now
        if(hexData.health <= 0) {
            onDeath();
        } else {
            if(onDamage != null) {
                MasterEffectsSound.instance.PlayOneShot(onDamage);
            }
        }
    }

	// Methods
	void onDeath() {

        if(!dying) {
            dying = true;

            // Do destruction animation
           startDeathAnimThenDestroy();
        }
	}

    // IEnumerator killAfterTime(bool hasEffect, float seconds) {
    //     yield return new WaitForSeconds(seconds);

    //     startDeathAnimThenDestroy(hasEffect);
    // }

    void startDeathAnimThenDestroy() {

        // play death sound
        if(onBreak != null) {
            MasterEffectsSound.instance.PlayOneShot(onBreak);
        }


            GameManage.instance.removeShadow(transform.position);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            
            hexShadow.enabled = false;

            SpawnDeathAnimation();
            SpawnHexDeathEffect();
            doDrop();
            givePoints();

            gameObject.SetActive(false);//Destroy(gameObject);
    }

    // Spawns animation if the hex is a biome
    private void SpawnDeathAnimation() {
        //if(!DataTypes.IsBiome(hexData.hexType)) return;
        

        GameObject deathAnimInstance;

            // PoolManager poolManager = GameManage.instance.GetComponent<PoolManager>();
			// deathAnimInstance = poolManager.GetObjectByName("HexExplode");
			// if(poolManager != null && poolManager.isActiveAndEnabled && deathAnimInstance != null) {
            //     deathAnimInstance.transform.SetPositionAndRotation(transform.position, transform.rotation);
			// } else {
				deathAnimInstance = Instantiate(hexExplodeAnimPrefab, transform.position, transform.rotation);
                //deathAnimInstance.GetComponentInChildren<HexDeathAnimation>().setHexData(hexData);
			// }

            if(DataTypes.IsBiome(hexData.hexType)) {
                deathAnimInstance.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;//DataTypes.GetBackgroundColorFrom(DataTypes.GetBiomeFrom(hexData.hexType));
                deathAnimInstance.transform.GetChild(0).GetComponent<SpriteRenderer>().color = transform.GetChild(1).GetComponent<SpriteRenderer>().color;//DataTypes.GetAccentColorFrom(DataTypes.GetBiomeFrom(hexData.hexType));
            } else {
                deathAnimInstance.GetComponent<SpriteRenderer>().color = DataTypes.GetAccentColorFrom(DataTypes.GetBiomeFrom(hexData.hexType));
                deathAnimInstance.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
            }


            
    }

    private void SpawnHexDeathEffect() {
        if(DataTypes.IsBiome(hexData.hexType)) return;

        GameObject effectObject = new GameObject("Death Effect");
        effectObject.transform.SetParent(transform, false);
        effectObject.transform.parent = null;

        // Get the right prefab by biome 
        switch (hexData.hexType)
        {
            case DataTypes.HexType.Fire:
                effectObject.AddComponent<FireDeathEffect>();
                break;
            case DataTypes.HexType.Water:
                effectObject.AddComponent<WaterDeathEffect>()
                    .setInits(lastBallToContact, waterSpinSound);
                break;
            case DataTypes.HexType.Earth:
                effectObject.AddComponent<EarthDeathEffect>()
                    .SetInits(earthPhysicsMaterial, GetComponent<SpriteRenderer>().sprite);
                break;
            case DataTypes.HexType.Shadow:
                effectObject.AddComponent<ShadowDeathEffect>()
                    .showShadowAfterTime(0.5f);
                break;
            default:
                break;
        }
    }

    private void doDrop() {
        GameObject drop = essenceDrop;

        if(drop != null && (!DataTypes.IsBiome(hexData.hexType) || isVined)) {
            spawnEssence(drop);
        }
        

        int gainCount = GainMoreEssenceCount();
        if(gainCount > 0) {
            if(drop != null) {
                float chance = DataTypes.IsBiome(hexData.hexType) ? 0.1f * gainCount : 0.9f * gainCount;

                if(Random.value < chance) {
                    spawnEssence(drop);
                }
            }
        }
    }

    private void givePoints() {
        int points = hexData.points;
        GameManage.instance.hexDestroyed(points, countTowardHexesDestroyed, transform.position);
    }

    private void spawnEssence(GameObject drop) {
        GameObject dropInstance = Instantiate(drop, transform);
        dropInstance.transform.parent = null;
        dropInstance.GetComponent<Drop>().setDropType(DataTypes.GetBiomeFrom(hexData.hexType));
    }

    private int GainMoreEssenceCount() {
        int count = 0;

        count += GameManage.instance.paddleData.runeIds.Contains(PowerUps.FireGainMoreEssence) ? 1 : 0;
        count += GameManage.instance.paddleData.runeIds.Contains(PowerUps.EarthGainMoreEssence) ? 1 : 0;
        count += GameManage.instance.paddleData.runeIds.Contains(PowerUps.WaterGainMoreEssence) ? 1 : 0;
        count += GameManage.instance.paddleData.runeIds.Contains(PowerUps.GrowthGainMoreEssence) ? 1 : 0;
        count += GameManage.instance.paddleData.runeIds.Contains(PowerUps.ShadowGainMoreEssence) ? 1 : 0;
        count += GameManage.instance.paddleData.runeIds.Contains(PowerUps.PoisonGainMoreEssence) ? 1 : 0;
        
        return count;
    }

    public IEnumerator startDeathCountDown(float seconds) {
        yield return new WaitForSeconds(seconds);
        if(this == null) {
            yield break;
        }

        timerDestroyed = true;

        removeHealth(hexData.health);
    }

    IEnumerator SendGameManageDeath(float seconds) {
        yield return new WaitForSeconds(seconds);

        GameManage.instance.hexDestroyed(0, countTowardHexesDestroyed, transform.position);
    }

    private static List<Vector3> getHexRingInWorldPos(int distanceAway, Vector3 localPos) {
        Vector3Int cellPos = cellPosFromLocal(localPos);

        return getHexRingInWorldPos(cellPos, distanceAway);
    }

    public static List<Vector3> getHexRingInWorldPos(Vector3Int cellPos, int distanceAway) {
        List<Vector3> returnableList = new List<Vector3>();
        List<Vector3Int> hexes = new List<Vector3Int>();

        if(distanceAway == 1) {
            
            if(cellPos.y % 2 == 0) {
                hexes.Add(cellPos + new Vector3Int(-1, 1, 0));
                hexes.Add(cellPos + new Vector3Int(0, 1, 0));
                hexes.Add(cellPos + new Vector3Int(-1, 0, 0));
                hexes.Add(cellPos + new Vector3Int(1, 0, 0));
                hexes.Add(cellPos + new Vector3Int(-1, -1, 0));
                hexes.Add(cellPos + new Vector3Int(0, -1, 0));
            } else {
                hexes.Add(cellPos + new Vector3Int(0, 1, 0));   // top left
                hexes.Add(cellPos + new Vector3Int(1, 1, 0));   // top right
                hexes.Add(cellPos + new Vector3Int(-1, 0, 0));  // left
                hexes.Add(cellPos + new Vector3Int(1, 0, 0));   // right
                hexes.Add(cellPos + new Vector3Int(0, -1, 0));  // bot left
                hexes.Add(cellPos + new Vector3Int(1, -1, 0));  // bot right
            }

        } else if(distanceAway == 2) { // NOT FULLY IMPLEMENTED
            if(cellPos.y % 2 == 0) {
                hexes.Add(cellPos + new Vector3Int(0, -2, 0));
                hexes.Add(cellPos + new Vector3Int(-1, -2, 0));
                hexes.Add(cellPos + new Vector3Int(1, -2, 0));

                hexes.Add(cellPos + new Vector3Int(-2, -1, 0));  
                hexes.Add(cellPos + new Vector3Int(1, -1, 0));

                hexes.Add(cellPos + new Vector3Int(-2, 0, 0));  // left
                hexes.Add(cellPos + new Vector3Int(2, 0, 0));   // right

                hexes.Add(cellPos + new Vector3Int(-2, 1, 0));  
                hexes.Add(cellPos + new Vector3Int(1, 1, 0));

                hexes.Add(cellPos + new Vector3Int(-1, 2, 0));
                hexes.Add(cellPos + new Vector3Int(0, 2, 0));
                hexes.Add(cellPos + new Vector3Int(1, 2, 0));
            } else {
                hexes.Add(cellPos + new Vector3Int(-1, -2, 0));
                hexes.Add(cellPos + new Vector3Int(0, -2, 0));
                hexes.Add(cellPos + new Vector3Int(1, -2, 0));

                hexes.Add(cellPos + new Vector3Int(-1, -1, 0));  
                hexes.Add(cellPos + new Vector3Int(2, -1, 0));

                hexes.Add(cellPos + new Vector3Int(-2, 0, 0));  // left
                hexes.Add(cellPos + new Vector3Int(2, 0, 0));   // right

                hexes.Add(cellPos + new Vector3Int(-1, 1, 0));  
                hexes.Add(cellPos + new Vector3Int(2, 1, 0));

                hexes.Add(cellPos + new Vector3Int(-1, 2, 0));
                hexes.Add(cellPos + new Vector3Int(0, 2, 0));
                hexes.Add(cellPos + new Vector3Int(1, 2, 0));
            }
        }

        foreach (Vector3Int cell in hexes) {
                returnableList.Add(worldPosOfCell(cell));
            }

        return returnableList;
    }

    public static Vector3Int cellPosFromLocal(Vector3 localPosition) {
        return GameManage.instance.gridInstance.
            GetComponent<Grid>().LocalToCell(localPosition);
    }

    public static Vector3 worldPosOfCell(Vector3Int cellPos) {
        Grid grid = GameManage.instance.gridInstance.GetComponent<Grid>();

        return grid.CellToWorld(cellPos);
    }

    public List<Hex> getHexesFrom(List<Vector3> worldPos) {
        List<Hex> hexes = new List<Hex>();

        foreach (Vector3 pos in worldPos)
        {
            Hex hex = getHexAt(pos);
            if(hex != null) {
                hexes.Add(hex);
            }
        }

        return hexes;
    }

    public static Hex getHexAt(Vector3 worldPos) {
        RaycastHit2D hits = Physics2D.Raycast(worldPos, Vector2.zero);
        if(hits.collider != null) {
            return hits.collider.gameObject.GetComponent<Hex>();
        }
        return null;
    }


    // Issues: doesn't grow out of other growth blocks' vines, 

    void grow(float percentChance, Hex vineOrigin) {

        if(vineOrigin == this) {
            currentVineLayer++;
        }
        
        List<Hex> addedVines = new List<Hex>();
        // Initialize list of all vines
        if(prevVineLayer == null) {
            prevVineLayer = new List<Hex>();

            addedVines.AddRange(growOnSurrounding(vineChance, this, currentVineLayer));
        } else {
            //print(currentVineLayer);

            
            // Run through current vines to try to expand
            foreach (Hex hex in prevVineLayer) {
                if(hex.vineLayer == currentVineLayer) {
                    addedVines.AddRange(hex.growOnSurrounding(vineChance, this, currentVineLayer));
                }
            }
        }

        // update the current vines
        prevVineLayer.AddRange(addedVines);
    }

    List<Hex> growOnSurrounding(float percentChance, Hex vineOrigin, int currentLayer) {
        List<Hex> grownOn = new List<Hex>();
        if(this == null) { return grownOn; }
        // Get this cell position
        Vector3Int cellPos = cellPosFromLocal(transform.localPosition);

        // Get all surrounding cell positions
        List<Vector3> cells = getHexRingInWorldPos(cellPos, 1);
        foreach (Vector3 cellWorldPos in cells)
        {
            // if a random value from 0 to 1 is within the chance of growing
            if(Random.value <= percentChance) 
            {
                Hex hex = getHexAt(cellWorldPos);
                if(hex != null) {
                    if(hex.checkCanGrow()) { 
                        hex.addVines(vineOrigin);
                        hex.vineLayer = currentLayer + 1;
                        grownOn.Add(hex);
                    }
                }
            }
        }

        return grownOn;
    }

    bool checkCanGrow() {
        return !(
            isVined ||
            isElement(getHexType())
        );
    }

    public static bool isElement(DataTypes.HexType type) {
        return type == DataTypes.HexType.Growth || 
        type == DataTypes.HexType.Earth || 
        type == DataTypes.HexType.Poison || 
        type == DataTypes.HexType.Fire || 
        type == DataTypes.HexType.Water || 
        type == DataTypes.HexType.Shadow;
    }

    public void addVines(Hex vineOrigin) {
        GetComponent<SpriteRenderer>().color = Color.green; //vineInstance = Instantiate(vineOverlay, transform);
        isVined = true;
        vineOriginHex = vineOrigin;
    }

    void spreadPoison(int attemptNumber) {
        List<Hex> ring = getHexesFrom(getHexRingInWorldPos(1, transform.localPosition));

        if(ring.Count == 0) {
            return;
        }

        int randIndex = Random.Range(0, ring.Count - 1);

        ring[randIndex].poison(attemptNumber);

    }

    public Sprite debugSprite;

    bool quakingHit(Ball ball) {
        Vector2 direction = getDirection(transform.position, ball.transform.position);

        float gridScaleMag = getGridScaleMag();

        // Uncomment to debug
        // print(gridScaleMag + ", " + direction);
        // GameObject newGO = new GameObject();
        // SpriteRenderer renderer = newGO.AddComponent<SpriteRenderer>();
        // renderer.sprite = debugSprite;
        // Instantiate(newGO, direction * gridScaleMag + (Vector2)transform.position, Quaternion.identity);
        
        RaycastHit2D hit = Physics2D.Raycast(direction * gridScaleMag + (Vector2)transform.position, Vector2.zero);

        if(hit.collider != null && hit.collider.tag == "Hex") {
            hit.collider.gameObject.GetComponent<Hex>().removeHealth(1);
            return true;
        }

        List<Vector3> surrounding = getHexRingInWorldPos(cellPosFromLocal(transform.localPosition), 1);
        List<Hex> foundHexes = new List<Hex>();

        foreach (Vector3 pos in surrounding)
        {
            hit = Physics2D.Raycast(pos, Vector2.zero);

            if(hit.collider != null && hit.collider.tag == "Hex") {
                foundHexes.Add(hit.collider.gameObject.GetComponent<Hex>());
            }

        }

        if(foundHexes.Count != 0) {
			// choose one of the surrounding options
			int randIndex = Random.Range(0, foundHexes.Count - 1);
            foundHexes[randIndex].removeHealth(1);
            return true;
		}

        return false;
    }

    

    // returns a normalized vector from b to a
    public static Vector2 getDirection(Vector2 a, Vector2 b) {
        return (a - b).normalized;
    }

    // Returns the distance of one unit of the grid scale in world space
    public static float getGridScaleMag() {
        Vector2 one = GameManage.instance.gridInstance.GetComponent<Grid>().CellToWorld(new Vector3Int(1, 0, 0));
        Vector2 zero = GameManage.instance.gridInstance.GetComponent<Grid>().CellToWorld(new Vector3Int(0, 0, 0));

        return (one - zero).magnitude;
    }

    private void beginAutoDeath() {
        // if(!isPoisoned) {
            autoDeath = StartCoroutine(blink(5));//StartCoroutine(ChangeShadowAlpha(1, 5));
        // }
    }

    // private IEnumerator ChangeShadowAlpha(float finalAlpha, float time)
	// {
	// 	float elapsedTime = 0;

    //     float numSteps = Time.deltaTime/time;

    //     Color growingColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
    //     growingColor.a = 0;
    //     Color addColor = new Color(numSteps/2, numSteps/2, numSteps/2, numSteps);

    //     Vector3 growingScale = transform.GetChild(0).localScale;
    //     Vector3 addScale = new Vector3(numSteps, numSteps, 0);

	// 	while (elapsedTime < time)
	// 	{
    //         growingColor += addColor;
    //         growingScale += addScale;
	// 		transform.GetChild(0).GetComponent<SpriteRenderer>().color = growingColor;
    //         transform.GetChild(0).localScale = growingScale;
	// 		elapsedTime += Time.deltaTime;
	// 		yield return null;
	// 	}

    //     // yield return StartCoroutine(tryRunDeathAnimation(2f));
	// }

    private IEnumerator blink(float time) {
        float endTime = Time.time + time;
        float timeLeft = time;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        while (timeLeft > 0) {
            Sequence seq = DOTween.Sequence();
            timeLeft = endTime - Time.time;

            foreach (SpriteRenderer renderer in renderers)
            {
                seq.Join(renderer.DOFade(0, (timeLeft) / 10.0f));
            }

            seq.AppendInterval(0.01f);

            foreach (SpriteRenderer renderer in renderers)
            {
                seq.Join(renderer.DOFade(1, (timeLeft) / 10.0f));
            }
            
            yield return seq.WaitForCompletion();
        }
    }

    private void stopAutoDeath() {
        if(autoDeath != null) {
            StopCoroutine(autoDeath);
        }
    }

    public bool isDying() {
        return dying;
    }

    public HexData getHexData() {
        return hexData;
    }

    public void setHexData(HexData hexData) {
        this.hexData = hexData;
        GetComponent<HexBuilder>().needsUpdate = true;
    }

    public int getHealth() {
        return hexData.health;
    }

    public void setHealth(int health) {
        hexData.health = health;
        GetComponent<HexBuilder>().needsUpdate = true;
    }

    public DataTypes.HexType getHexType() {
        return hexData.hexType;
    }
    
    public void setHexType(DataTypes.HexType type) {
        hexData.hexType = type;
        GetComponent<HexBuilder>().needsUpdate = true;
    }

    public int getPointValue() {
        return hexData.points;
    }

    public void setPointValue(int value) {
        hexData.points = value;
    }

    public bool getEarthKillOnContact() {
        return earthKillOnContact;
    }

    public bool getPoisoned() {
        return isPoisoned;
    }
}
