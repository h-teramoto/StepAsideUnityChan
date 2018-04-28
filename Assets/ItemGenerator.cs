using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// アイテムを生成する
/// </summary>
public class ItemGenerator : MonoBehaviour
{

    //unitychanを入れる
    [SerializeField]
    private GameObject unitychan;

    //carPrefabを入れる
    [SerializeField]
    private GameObject carPrefab;

    //coinPrefabを入れる
    [SerializeField]
    private GameObject coinPrefab;

    //cornPrefabを入れる
    [SerializeField]
    private GameObject conePrefab;

    //スタート地点
    private int startPos = -160;

    //ゴール地点
    private int goalPos = 120;

    //アイテムを出すx方向の範囲
    private float posRange = 3.4f;

    //アイテムを置くラインの間隔
    private int itemIntervel = 15;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        StartCoroutine("ItemGenerateObserve");

    }

    /// <summary>
    /// アイテムを生成するタイミングを監視
    /// </summary>
    /// <returns></returns>
    private IEnumerator ItemGenerateObserve()
    {
        //すでに生成しているZ軸ポジションリスト
        List<int> createZPosList = new List<int>();

        while (true)
        {
            //unitychanの現在位置からアイテム列を生成するz軸を算出

            int createDistance = (int)(50 / itemIntervel) * itemIntervel;
            int realTargetZPos = (int)unitychan.transform.position.z + createDistance - startPos;
            int targetZIndex = (int)(realTargetZPos / itemIntervel);

            if (targetZIndex >= 0)
            {
                int targetZPos = startPos + (targetZIndex * itemIntervel);

                if (goalPos >= targetZPos && !createZPosList.Contains(targetZPos))
                { 
                    createZPosList.Add(targetZPos);
                    Debug.Log(targetZPos);
                    //アイテムを生成する
                    yield return StartCoroutine("CreateItemList", targetZPos);
                }
            }

            yield return null;
        }
    }



    /// <summary>
    /// アイテムを動的に生成する
    /// </summary>
    /// <param name="targetZPos">アイテム生成するZ座標</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator CreateItemList(int targetZPos)
    {
        //どのアイテムを出すのかをランダムに設定
        int num = Random.Range(0, 10);
        if (num <= 1)
        {
            //コーンをx軸方向に一直線に生成
            for (float j = -1; j <= 1; j += 0.4f)
            {
                GameObject cone = Instantiate(conePrefab) as GameObject;
                cone.transform.position = new Vector3(4 * j, cone.transform.position.y, targetZPos);
                StartCoroutine("ItemDestroyObserve", cone);
            }
        }
        else
        {
            //レーンごとにアイテムを生成
            for (int j = -1; j < 2; j++)
            {
                //アイテムの種類を決める
                int item = Random.Range(1, 11);
                //アイテムを置くZ座標のオフセットをランダムに設定
                int offsetZ = Random.Range(-5, 6);
                //60%コイン配置:30%車配置:10%何もなし
                if (1 <= item && item <= 6)
                {
                    //コインを生成
                    GameObject coin = Instantiate(coinPrefab) as GameObject;
                    coin.transform.position = new Vector3(posRange * j, coin.transform.position.y, targetZPos + offsetZ);
                    StartCoroutine("ItemDestroyObserve", coin);
                }
                else if (7 <= item && item <= 9)
                {
                    //車を生成
                    GameObject car = Instantiate(carPrefab) as GameObject;
                    car.transform.position = new Vector3(posRange * j, car.transform.position.y, targetZPos + offsetZ);
                    StartCoroutine("ItemDestroyObserve", car);
                }
            }
        }

        yield return null;

    }

    /// <summary>
    /// アイテムを破棄するタイミングを監視
    /// </summary>
    /// <param name="go">GameObject 破棄する対象</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator ItemDestroyObserve(GameObject go)
    {
        while (true)
        {
            //unitychanがアイテムを取得していると先に破棄されているため終了
            if (go == null)
            {
                yield break;
            }

            //unitychanの現在z位置
            int unitychanPosz = (int)unitychan.transform.position.z;

            //Renderの取得
            Renderer rend = go.GetComponent<Renderer>();

            //unitychanよりもアイテムが後ろにあり且つ画面の外に出ている場合
            if (unitychanPosz > go.transform.position.z + 5)
            {
                Debug.Log("破棄:" + go.transform.position.z);
                //破棄する
                Destroy(go.gameObject);
                
                yield break;
            }

            yield return null;
        }
    }
}




   