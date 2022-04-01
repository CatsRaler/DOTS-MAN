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

    //��ʵ��object�����е�prefab
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

        //��ʼ��EntityManager
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();

        //��object������setting
        //��inspector�п��Ի�ȡ��prefab
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);

        //ͨ��GameObjectConversionUtility��ConvertGameObjectHierarchy����object���entity
        //������GameObject root, World dstEntityWorld��
        ballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ballPrefab, settings);
        cubeEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(cubePrefab, settings);

    }

    private void OnDestroy()
    {
        //����BlobAssetStore�е�blobasset���棬�ͷ����blobAssetStore
        blobAssetStore.Dispose();
    }

    private void Start()
    {
        curScore = 0;

        insaneMode = false;

        //��ʾ���������������ÿһ֡�������
        DisplayScore();
        //������ʼ����
        SpawnBall();
    }

    private void Update()
    {
        //������������Ϳ���insanemode����췽��
        //if (!insaneMode && curScore >= maxScore)
        if (insaneMode)
        {
            //����Э���췽��
            //insaneMode = true;
            StartCoroutine(SpawnLotsOfCubes());
        }
    }

    //�ص����췽��
    IEnumerator SpawnLotsOfCubes()
    {
        while (insaneMode)
        {
            //ÿһ֡��cubesPerFrame���ķ���
            for (int i = 0; i < cubesPerFrame; i++)
            {
                SpawnNewCube();
            }
            yield return null;
        }
    }


    void SpawnNewCube()
    {
        //ʹ��entityManager�췽�鲢�Ҹ�������

        Entity newCubeEntity = entityManager.Instantiate(cubeEntityPrefab);

        Vector3 direction = Vector3.up;
        Vector3 speed = direction * cubeSpeed;

        PhysicsVelocity velocity = new PhysicsVelocity()
        {
            Linear = speed,
            Angular = float3.zero
        };

        //���ǵ���entity���component����
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

    //���һ����
    void SpawnBall()
    {
        Entity newBallEntity = entityManager.Instantiate(ballEntityPrefab);

        Translation ballTrans = new Translation
        {
            //��ʼλ��
            Value = new float3(0f, 0.5f, 0f)
        };

        //����Ҫ�ǵ����component
        entityManager.AddComponentData(newBallEntity, ballTrans);
        //���þ�ͷ����Ķ���
        CameraFollow.instance.ballEntity = newBallEntity;
    }
}
