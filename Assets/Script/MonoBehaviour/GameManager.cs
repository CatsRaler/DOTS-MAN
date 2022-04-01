using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine.UI;
using Unity.Transforms;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool insaneMode;

    //在实体object世界中的prefab
    public GameObject ballPrefab;
    public GameObject cubePrefab;
    public Text scoreText;

    public int maxScore;
    public int cubesPerFrame;
    public float cubeSpeed = 3f;

    private int curScore;
    private Entity ballEntityPrefab;
    private Entity cubeEntityPrefab;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    //private bool insaneMode;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        //初始化EntityManager
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();

        //从object世界获得setting
        //即inspector中可以获取的prefab
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);

        //通过GameObjectConversionUtility的ConvertGameObjectHierarchy来把object变成entity
        //参数（GameObject root, World dstEntityWorld）
        ballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ballPrefab, settings);
        cubeEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(cubePrefab, settings);

    }

    private void OnDestroy()
    {
        //重置BlobAssetStore中的blobasset缓存，释放清空blobAssetStore
        blobAssetStore.Dispose();
    }

    private void Start()
    {
        curScore = 0;

        insaneMode = false;

        //显示分数，这个函数在每一帧都会调用
        DisplayScore();
        //创建初始球球
        SpawnBall();
    }

    private void Update()
    {
        //如果符合条件就开启insanemode疯狂造方块
        //if (!insaneMode && curScore >= maxScore)
        if (insaneMode)
        {
            //开启协程造方块
            //insaneMode = true;
            StartCoroutine(SpawnLotsOfCubes());
        }
    }

    //回调，造方块
    IEnumerator SpawnLotsOfCubes()
    {
        while (insaneMode)
        {
            //每一帧造cubesPerFrame量的方块
            for (int i = 0; i < cubesPerFrame; i++)
            {
                SpawnNewCube();
            }
            yield return null;
        }
    }


    void SpawnNewCube()
    {
        //使用entityManager造方块并且给予属性

        Entity newCubeEntity = entityManager.Instantiate(cubeEntityPrefab);

        Vector3 direction = Vector3.up;
        Vector3 speed = direction * cubeSpeed;

        PhysicsVelocity velocity = new PhysicsVelocity()
        {
            Linear = speed,
            Angular = float3.zero
        };

        //最后记得往entity添加component数据
        entityManager.AddComponentData(newCubeEntity, velocity);
    }

    public void IncreaseScore()
    {
        curScore++;
        DisplayScore();
    }

    private void DisplayScore()
    {
        scoreText.text = "Score: " + curScore;
    }

    //造第一个球
    void SpawnBall()
    {
        Entity newBallEntity = entityManager.Instantiate(ballEntityPrefab);

        Translation ballTrans = new Translation
        {
            //初始位置
            Value = new float3(0f, 0.5f, 0f)
        };

        //还是要记得添加component
        entityManager.AddComponentData(newBallEntity, ballTrans);
        //设置镜头跟随的对象
        CameraFollow.instance.ballEntity = newBallEntity;
    }
}
