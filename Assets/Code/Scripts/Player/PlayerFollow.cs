//using UnityEngine;
//using System.Collections.Generic;

//public class EnemyAttachToPlayer : MonoBehaviour
//{
//    public Vector3 offset = Vector3.zero;

//    private List<Transform> enemies = new List<Transform>();
//    private GrapplingHook grappling;

//    void Awake()
//    {
//        grappling = GetComponent<GrapplingHook>();
//    }

//    void LateUpdate()
//    {
//        if (!grappling.isAttach) return;

//        for (int i = 0; i < enemies.Count; i++)
//        {
//            if (enemies[i] == null) continue;

//            enemies[i].position = transform.position + offset;

//            GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
//            foreach (GameObject enemy in enemyObjs)
//            {
//                enemies.Add(enemy.transform);

//                Collider2D col = enemy.GetComponent<Collider2D>();
//                if (col != null)
//                    col.enabled = false;
//            }
//        }
//    }
//}
