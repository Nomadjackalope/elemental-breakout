using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle : MonoBehaviour {

	public GameObject stunShot;

	public List<Hex> hexes = new List<Hex>();

	private WaitForSeconds timeBetweenShots = new WaitForSeconds(1.00f);//new WaitForSeconds(0.01f);

    private List<Group> groups = new List<Group>();

	private BossMovement movementType;

	// Use this for initialization
	void Start () {
		hexes = GameManage.instance.hexes;
		movementType = GameManage.instance.movementType;
		StartCoroutine(CheckShot());
		moveHexes();
	}

	IEnumerator CheckShot() {
		for(;;) {
			foreach (Hex hex in hexes)
			{
				if(hex != null) {
					if(DataTypes.IsElement(hex.getHexType())) {
						if(Random.value < 0.15f) {
							Instantiate(stunShot, hex.transform).transform.parent = null;
						}
					}
				}
			}

			yield return timeBetweenShots;
		}
	}

	void moveHexes() {

		switch (movementType)
		{
			case BossMovement.AllRightLeft:
				MoveAllRightLeft();
				break;
			case BossMovement.AllUpDown:
				MoveAllUpDown();
				break;
			case BossMovement.BreatheFromCenter:
				breathe();
				break;
			case BossMovement.ColumnsRightLeft:
				StartCoroutine(moveRightLeftColumns());
				break;
			case BossMovement.HorizontalWave:
				moveWaveHorizontal();
				break;
			case BossMovement.RandomUpDown:
				moveRandomUpDown();
				break;
			case BossMovement.RotateAroundElements:
				rotateAroundElements();
				break;
			case BossMovement.VerticalWave:
				moveWave();
				break;
			default:
				break;
		}
	}

	void MoveAllRightLeft() {
		Group group = new Group();
		group.hexes = hexes;

		moveRightLeft(group, 0, 2);
	}
	

	void MoveAllUpDown() {
		Group group = new Group();
		group.hexes = hexes;

		moveUpDown(group, 0, 2);
	}

	IEnumerator moveRightLeftColumns() {
		Dictionary<int, Group> columns = new Dictionary<int, Group>();

		// Create column groups
		foreach (Hex hex in hexes)
		{
			Vector3Int cellPos = GameManage.instance.gridInstance.GetComponent<Grid>().WorldToCell(hex.transform.position);

				Group thisGroup = null;

				if(columns.TryGetValue(cellPos.x, out thisGroup)) {
					thisGroup.hexes.Add(hex);
				} else {
					thisGroup = new Group();
					thisGroup.hexes.Add(hex);
					columns.Add(cellPos.x, thisGroup);
				}
		}
		
		List<int> sortedColumns = new List<int>(columns.Keys);
		sortedColumns.Sort();
	
		for(;;) {
		
			sortedColumns.Sort();
			foreach (int column in sortedColumns)
			{
				yield return StartCoroutine(columns[column].moveToPos(Vector2.left, 2));
			}

			sortedColumns.Reverse();
			foreach (int column in sortedColumns) {
				yield return StartCoroutine(columns[column].moveToPos(Vector2.right, 2));
			}

		}
	}

	void moveWave() {
		Dictionary<int, Group> columns = new Dictionary<int, Group>();

		// Create column groups
		foreach (Hex hex in hexes)
		{
			Vector3Int cellPos = GameManage.instance.gridInstance.GetComponent<Grid>().WorldToCell(hex.transform.position);
				Group thisGroup = null;

				if(columns.TryGetValue(cellPos.x, out thisGroup)) {
					thisGroup.hexes.Add(hex);
				} else {
					thisGroup = new Group();
					thisGroup.hexes.Add(hex);
					columns.Add(cellPos.x, thisGroup);
				}
		}
		
		List<int> sortedColumns = new List<int>(columns.Keys);
		sortedColumns.Sort();

		int i = 0;

		// start each column
		foreach (int column in sortedColumns) {
			StartCoroutine(moveUpDown(columns[column], i * 0.25f, 2));

			i++;
		}
	}

	void moveWaveHorizontal() {
		Dictionary<int, Group> columns = new Dictionary<int, Group>();

		// Create column groups
		foreach (Hex hex in hexes)
		{
			Vector3Int cellPos = GameManage.instance.gridInstance.GetComponent<Grid>().WorldToCell(hex.transform.position);
				Group thisGroup = null;

				if(columns.TryGetValue(cellPos.y, out thisGroup)) {
					thisGroup.hexes.Add(hex);
				} else {
					thisGroup = new Group();
					thisGroup.hexes.Add(hex);
					columns.Add(cellPos.y, thisGroup);
				}
		}
		
		List<int> sortedColumns = new List<int>(columns.Keys);
		sortedColumns.Sort();

		int i = 0;

		// start each column
		foreach (int column in sortedColumns) {
			StartCoroutine(moveUpDown(columns[column], i * 0.25f, 1));

			i++;
		}
	}

	void moveRandomUpDown() {
		Dictionary<int, Group> columns = new Dictionary<int, Group>();

		// Create column groups
		foreach (Hex hex in hexes)
		{
			Vector3Int cellPos = GameManage.instance.gridInstance.GetComponent<Grid>().WorldToCell(hex.transform.position);
			Group thisGroup = null;

			Vector3 halfCellWidth = new Vector3(GameManage.instance.gridInstance.GetComponent<Grid>().CellToWorld(Vector3Int.right).x / 2, 0);
			Vector3Int oneCellUp = new Vector3Int(cellPos.x, cellPos.y + 1, 0);

			Vector3 oneCellUpRight = GameManage.instance.gridInstance.GetComponent<Grid>().CellToWorld(oneCellUp) + halfCellWidth;
			Vector3 oneCellUpLeft = GameManage.instance.gridInstance.GetComponent<Grid>().CellToWorld(oneCellUp) - halfCellWidth;

			Collider2D colliderRight = Physics2D.Raycast(oneCellUpRight, Vector2.zero).collider;
			Collider2D colliderLeft = Physics2D.Raycast(oneCellUpLeft, Vector2.zero).collider;

			
			if((colliderLeft == null || !colliderLeft.CompareTag("Hex")) && (colliderRight == null || !colliderRight.CompareTag("Hex"))) {
				int randomGroupIndex = Mathf.RoundToInt(Random.Range(0, 4));
				if(columns.TryGetValue(randomGroupIndex, out thisGroup)) {
					thisGroup.hexes.Add(hex);
				} else {
					thisGroup = new Group();
					thisGroup.hexes.Add(hex);
					columns.Add(randomGroupIndex, thisGroup);
				}
			}
		}
		
		List<int> sortedColumns = new List<int>(columns.Keys);
		sortedColumns.Sort();

		int i = 0;

		// start each column
		foreach (int column in sortedColumns) {
			StartCoroutine(moveUpDown(columns[column], i * Random.value, 1));

			i++;
		}
	}

	void rotateAroundElements() {
		List<Group> groups = new List<Group>();

		foreach (Hex hex in hexes)
		{
			if(DataTypes.IsElement(hex.getHexType())) {
				Group group = new Group();
				group.hexes.Add(hex);

				List<Hex> surrounding = hex.getHexesFrom(Hex.getHexRingInWorldPos(GameManage.instance.gridInstance.GetComponent<Grid>().WorldToCell(hex.transform.position), 1));
				foreach (Hex adjacent in surrounding)
				{
					// adjacent.transform.parent = hex.transform;
					if(!DataTypes.IsElement(adjacent.getHexType())) {
						group.hexes.Add(adjacent);
					}
				}

				groups.Add(group);

			}
		}

		foreach (Group group in groups)
		{
			StartCoroutine(rotate(group, 0, 7));
		}
	}

	void breathe() {
		List<Group> groups = new List<Group>();
		foreach (Hex hex in hexes)
		{
			Group group = new Group();
			group.hexes.Add(hex);
			groups.Add(group);
		}

		foreach (Group group in groups)
		{
			StartCoroutine(moveRelativeToCenter(group, 2));
		}
	}


	// Movement patterns ---------------------------------

	IEnumerator moveRightLeft(Group group, float secondsDelay, float seconds) {
		yield return new WaitForSeconds(secondsDelay);
		for(;;) {
			yield return StartCoroutine(group.moveToPos(Vector2.right, seconds));
			yield return StartCoroutine(group.moveToPos(Vector2.left, seconds));

		}
	}

	IEnumerator moveUpDown(Group group, float secondsDelay, float seconds) {
		yield return new WaitForSeconds(secondsDelay);
		for(;;) {
			yield return StartCoroutine(group.moveToPos(Vector2.up, seconds));
			yield return StartCoroutine(group.moveToPos(Vector2.down, seconds));
		}
	}

	IEnumerator rotate(Group group, float secondsDelay, float seconds) {
		Vector3 rotateCenter = group.hexes[0].transform.position;

		yield return new WaitForSeconds(secondsDelay);
		for(;;) {
			yield return StartCoroutine(group.rotateAround(rotateCenter, seconds));
		}
	}

	IEnumerator moveRelativeToCenter(Group group, float seconds) {
		if(group.hexes.Count > 0) {

			Vector3 originalPos = new Vector3(group.hexes[0].transform.position.x, group.hexes[0].transform.position.y);

			while(group.hexes.Count > 0) {
				group.hexes.RemoveAll(x => x == null);
				if(group.hexes.Count > 0) {				
					yield return StartCoroutine(group.moveToPos(group.hexes[0].transform.position.normalized, seconds));
				}

				group.hexes.RemoveAll(x => x == null);
				if(group.hexes.Count > 0) {				
					yield return StartCoroutine(group.moveToPos(originalPos - group.hexes[0].transform.position, seconds));
				}
			}
		}
	}

	// A group is a bunch of hexes that all must move the same delta for the same time
	class Group {
		public List<Hex> hexes = new List<Hex>();

		Dictionary<Hex, Vector2> futurePos = new Dictionary<Hex, Vector2>();
		Dictionary<Hex, Vector2> originPos = new Dictionary<Hex, Vector2>();


		public bool isMoving;

		Vector2 force;
		Vector2 curVelocity;
		Vector2 lastPos;
		float timeLeft;

		public IEnumerator moveToPos(Vector2 deltaPos, float seconds) {
			isMoving = true;

			bool allHexesMoving = true;

			foreach (Hex hex in hexes)
			{

				if(hex != null) {
					futurePos[hex] = (Vector2)hex.transform.position + deltaPos;
					originPos[hex] = (Vector2)hex.transform.position;
				}
			}


			while(allHexesMoving) {

				hexes.RemoveAll(x => x == null);

				foreach (Hex hex in hexes)
				{
					if(hex.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic) {
						hex.transform.Translate((futurePos[hex] - originPos[hex]) / seconds * Time.deltaTime);
					}
				}

				if(hexes.Count == 0 || ((Vector2)hexes[0].transform.position - futurePos[hexes[0]]).magnitude < 0.1) {
					allHexesMoving = false;
				}

				// Make sure we don't overshoot
				// ?? How to make sure we do this

				if(allHexesMoving) {
					yield return null;
				}

			}

			isMoving = false;
		}

		public IEnumerator rotateAround(Vector3 center, float seconds) {
			hexes.RemoveAll(x => x == null);

			foreach (Hex hex in hexes)
			{
				if(hex.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Dynamic) {
					hex.transform.RotateAround(center, Vector3.forward, 360 / seconds * Time.deltaTime);
				}
			}
			yield return null;
		}
	}
}

public enum BossMovement
{
	AllRightLeft,
	AllUpDown,
	ColumnsRightLeft,
	RandomUpDown,
	VerticalWave,
	HorizontalWave,
	RotateAroundElements,
	BreatheFromCenter,
	None
}
