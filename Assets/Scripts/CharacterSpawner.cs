using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public static CharacterSpawner Instance;
    
    [SerializeField] TextMeshProUGUI textPersons;
    [SerializeField] TextMeshProUGUI textAverageDistance;
    [SerializeField] GameObject[] pfCharacters;
    [SerializeField] GameObject[] spawnPositions;
    [SerializeField] float secondsBetweenSpawns;
    private int counterPersons;
    private int currentDepth;
    private bool isLearningPath;
    private List<double> currentAngles = new List<double>() { -80, -80, -80, -80, -80, -80, -80, -80, -80, -80, -80, -80 };
    private int currentDirectionChanges;
    private double totalDistanceTravelled;
    public int CounterPersons { get => counterPersons; set => counterPersons = value; }

    public void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        if (isLearningPath)
        {
            spawnCharacter();
        }
        else
        {
            StartCoroutine(spawnCharacters());
        }
    }

    private void FixedUpdate()
    {
        textPersons.text = counterPersons.ToString();
        textAverageDistance.text = (totalDistanceTravelled / counterPersons).ToString("N3");
    }

    private IEnumerator spawnCharacters()
    {
        while (true)
        {
            int SpawnPosition = Random.Range(0, spawnPositions.Length);
            GameObject newCharacter = Instantiate(pfCharacters[Random.Range(0, pfCharacters.Length)], 
                spawnPositions[SpawnPosition].transform.position, 
                Quaternion.identity);

            newCharacter.GetComponent<Character>().MakeMovementPlan(Game.Instance.GetPlanFromNeuralNetwork(SpawnPosition));
            CounterPersons++;
            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
    }

    private void spawnCharacter()
    {
        int SpawnPosition = Random.Range(0, spawnPositions.Length);
        GameObject newCharacter = Instantiate(pfCharacters[Random.Range(0, pfCharacters.Length)],
            spawnPositions[SpawnPosition].transform.position,
            Quaternion.identity);
        newCharacter.GetComponent<Character>().MakeMovementPlan(currentAngles);
        CounterPersons++;
    }

    public void AddPersonWalkResult(float totalDistanceTravelled, int numberOfDirectionChanges)
    {
        this.totalDistanceTravelled += totalDistanceTravelled;

        if (isLearningPath)
        {
            if (numberOfDirectionChanges > currentDepth + 1)
            {
                // found a possible road
                currentDepth++;
                //currentAngle[currentDepth] = -80;
            }
            else
            {
                currentAngles[currentDepth] += 10;

                if (currentAngles[currentDepth] > 90)
                {
                    currentDepth--;
                }
            }

            // next try
            spawnCharacter();
        }
    }

}
