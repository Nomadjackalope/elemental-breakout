using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BouncePredictor : MonoBehaviour {

    static List<BouncePredictor> predictors = new List<BouncePredictor>();
    static Dictionary<GameObject, int> hexesHit = new Dictionary<GameObject, int>();

    bool isAvailable = true;
    private int numCollisions = 0;
    private int maxCollisions = 20;
    private Vector2 prevVelocity;

    public static BouncePredictor GetPredictor(Transform transformInScene) {
        BouncePredictor predictor = predictors.Find(x => x.isAvailable);

        if(predictor == null) {
            predictor = Instantiate(GameManage.instance.bouncePredictorPrefab, transformInScene).GetComponent<BouncePredictor>();
            predictor.transform.parent = null;
            predictors.Add(predictor);
            DontDestroyOnLoad(predictor);
        }

        predictor.isAvailable = false;

        predictor.gameObject.SetActive(true);

        return predictor;
    }

    public void Remove() {
        if(gameObject != null) {
            gameObject.SetActive(false);
            transform.parent = null;

            // Reset vars
            isAvailable = true;
            numCollisions = 0;
            hexesHit.Clear();
        }
    }

    public static void RemoveAll() {
        foreach (BouncePredictor predictor in predictors)
        {
            predictor.Remove();
        }
    }

    void FixedUpdate() {
        prevVelocity = GetComponent<Rigidbody2D>().velocity;
    }

    public void launch(Vector2 velocity, Transform origin, float radius) {
        transform.position = origin.position;
        GetComponent<Rigidbody2D>().AddForce(velocity * 10, ForceMode2D.Impulse);
        GetComponent<CircleCollider2D>().radius = radius;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        numCollisions++;

        if(collision.collider.tag == "DeathBase" || numCollisions > maxCollisions) {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

        if(collision.collider.tag == "Hex") {

            // Ignores hexes already hit
            if(collision.collider.GetComponent<Hex>().getHexType() == DataTypes.HexType.Water) {
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }

            if(hexesHit.ContainsKey(collision.collider.gameObject) && hexesHit[collision.collider.gameObject] <= 0) {
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
                GetComponent<Rigidbody2D>().velocity = prevVelocity;
            } else {

                if(hexesHit.ContainsKey(collision.collider.gameObject)) {
                    hexesHit[collision.collider.gameObject] -= 1;
                } else {
                    hexesHit[collision.collider.gameObject] = collision.collider.GetComponent<Hex>().getHealth() - 1;
                }

                if(hexesHit[collision.collider.gameObject] <= 0) {
                    Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
                    //GetComponent<Rigidbody2D>().velocity = prevVelocity;
                }
            }
        }
    }
}