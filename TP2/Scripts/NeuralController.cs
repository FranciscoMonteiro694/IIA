﻿using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;


[RequireComponent(typeof (CarController))]
public class NeuralController : MonoBehaviour
{
	public CarController m_Car; // the car controller we want to use
    public int player;
    public GameObject ball;
    public GameObject MyGoal;
    public GameObject AdversaryGoal;
    public GameObject Adversary;
    public GameObject ScoreSystem;


    [Header("Sensor")]
	public Vector3 frontSensorPosition = new Vector3(0, 1.0f, 2.0f);
	public float sensorLength = 50f;
	public float angle = 45f; 

    public int numberOfInputSensores { get; private set; }
    public float[] sensorsInput; 
	

    // Available Information 
    [Header("Environment  Information")]
    public float distanceToBall;
    public float distanceToMyGoal;
    public float distanceToAdversaryGoal;
    public float distanceToAdversary;
    public float distancefromBallToAdversaryGoal;
    public float distancefromBallToMyGoal;
    public float driveTime = 0;
	public float distanceTravelled = 0.0f;
	public float avgSpeed = 0.0f;
    public float maxSpeed = 0.0f;
    public float currentSpeed = 0.0f;
    public float currentDistance = 0.0f;
    public int hitTheBall; 
	//

	

    public float maxSimulTime = 1200;
    public bool GameFieldDebugMode = false;
    public bool gameOver = false;
    public bool running = false;

    private Vector3 startPos;
    private Vector3 previousPos;

    public int GoalsOnAdversaryGoal;
    public int GoalsOnMyGoal;

    public NeuralNetwork neuralController;

    private void Awake()
	{
		// get the car controller
		m_Car = GetComponent<CarController>();
        numberOfInputSensores = (int)(360 / angle) + 7;
        sensorsInput = new float[numberOfInputSensores];

		startPos = m_Car.transform.position;
		previousPos = startPos;
        //Debug.Log(this.neuralController);
        if (GameFieldDebugMode && this.neuralController.weights == null)
        {
            Debug.Log("creating nn..!! ONLY IN GameFieldDebug SCENE THIS SHOULD BE USED!");
            int[] top = { 15, 20, 2 }; // Estava 12,20,2 Mudámos para 15,20,2 para estar como no enunciado
            this.neuralController = new NeuralNetwork(top,0);

        }

    }


	private void FixedUpdate()
	{
		if (running) {
			// updating sensors
			SensorHandling ();
			// move the car
			float[] result = this.neuralController.process (sensorsInput);
			float h = result [0];//Erro aqui
			float v = result [1];
        
            m_Car.Move (h, v, v, 0);

			// updating race status
			updateGameStatus ();

            // check method
			if (endSimulationConditions()) {
				wrapUp ();
			}
		}
	}


    private bool endSimulationConditions()
    {
        // if we do not move for too long, we stop the simulation
        // or if we are simmulating for too long, we stop the simulation
        // You can modify this to change the length of the simulation of an individual before evaluating it.
        return (currentDistance / driveTime <= 0.1 && driveTime > 50) || driveTime > this.maxSimulTime;
    }

    public void SensorHandling()
    {
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;

        // distance to ball 
        sensorsInput[0] = Vector3.Distance(m_Car.transform.localPosition,  ball.transform.localPosition);
        //sensorsInput[0] = sensorsInput[0] * Vector3.Dot(m_Car.transform.forward, ball.transform.forward);
        //print(m_Car.transform.localPosition - ball.transform.localPosition);
        distanceToBall += sensorsInput[0];
        // distance to my goal
        sensorsInput[1] = Vector3.Distance(m_Car.transform.localPosition, MyGoal.transform.localPosition);
        //sensorsInput[1] = sensorsInput[1] * Vector3.Dot(m_Car.transform.forward, MyGoal.transform.forward);
        distanceToMyGoal += sensorsInput[1];
        // distance to adversary goal
        sensorsInput[2] = Vector3.Distance(m_Car.transform.localPosition, AdversaryGoal.transform.localPosition);
        //sensorsInput[2] = sensorsInput[2] * Vector3.Dot(m_Car.transform.forward, AdversaryGoal.transform.forward);
        distanceToAdversaryGoal += sensorsInput[2];
        // distance to adversary 
        if (Adversary != null) { 
            sensorsInput[3] = Vector3.Distance(m_Car.transform.localPosition, Adversary.transform.localPosition);
            distanceToAdversary += sensorsInput[3];
        }
        else
        {
            sensorsInput[3] = -1;// -1 == não existe
        }

        // distance of the ball to the my goal
        sensorsInput[4] = Vector3.Distance(ball.transform.localPosition, MyGoal.transform.localPosition);
        distancefromBallToMyGoal += sensorsInput[4];
        // distance of the ball to the adversary goal
        sensorsInput[5] = Vector3.Distance(ball.transform.localPosition, AdversaryGoal.transform.localPosition);
        distancefromBallToAdversaryGoal += sensorsInput[5];

        // sensor angle with the ball [-1,...,-1]
        sensorsInput[6] = (Vector3.SignedAngle(ball.transform.localPosition -m_Car.transform.localPosition, m_Car.transform.forward, Vector3.up)) / 180.0f;

        RaycastHit hit;
        // rays...
        for (int i = 0; i * angle < 360f; i++)
        {
            // sensor...
            // Quaternion.AngleAxis(-checkPointRayAngle * i, transform.up)
            if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-angle * i, transform.up) * transform.forward, out hit, Mathf.Infinity, (1 << 2)))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                sensorsInput[ i + 7 ] = (sensorStartPos - hit.point).magnitude;
                //Debug.Log(sensorsInput[i]);
                //break;
            }
        }



    }


    public void updateGameStatus()
	{   
        // This is the information you can use to build the fitness function. 
		driveTime += Time.deltaTime;
		currentDistance = Vector3.Distance(previousPos, m_Car.transform.localPosition);
		distanceTravelled += currentDistance * (m_Car.isReversing ? -1 : 1);
        previousPos = m_Car.transform.localPosition;

		// speed takes into account the direction of the car: if we are reversing it is negative
		avgSpeed += m_Car.CurrentSpeed * (m_Car.isReversing ? -1 : 1); 
		currentSpeed = m_Car.CurrentSpeed * (m_Car.isReversing ? -1 : 1);
		maxSpeed = (currentSpeed > maxSpeed ? currentSpeed : maxSpeed);


        // get my score
        GoalsOnMyGoal = (int) ScoreSystem.GetComponent<ScoreKeeper>().score[player];
        // get adversary score
        GoalsOnAdversaryGoal = (int)ScoreSystem.GetComponent<ScoreKeeper>().score[player == 0 ? 1 : 0];


    }


    public float GetScore()
    {
        int difGolos;
        float fitness;
        difGolos =  GoalsOnAdversaryGoal -  GoalsOnMyGoal;
        //Debug.Log("Valor do distance travelled" + distanceTravelled);

        //Diferença de golos
        // se estiver empatado
        if (difGolos == 0)
        {
            fitness = 1 / distancefromBallToAdversaryGoal;
            fitness = (float)Math.Pow(fitness,3);
            return fitness;
        }
        else
        {
            fitness = difGolos*2;
            fitness = (float)Math.Pow(fitness, 3)*driveTime;
            return fitness;
        }


    }

    // Em último caso vai esta
    //public float GetScore()
    //{
    //    int difGolos;
    //    float fitness;
    //    difGolos = GoalsOnAdversaryGoal - GoalsOnMyGoal;
    //    //Debug.Log("Valor do current distance" + currentDistance);
    //    //Debug.Log("Valor do distance travelled" + distanceTravelled);

    //    //Diferença de golos
    //    // Se for menor que 0
    //    if (difGolos < 0)
    //    {
    //        // Vamos dar um valor de aptidão baixo para este elemento
    //        fitness = difGolos;

    //    }
    //    else
    //    {
    //        // Caso de empate
    //        if (difGolos == 0)
    //        {
    //            fitness = 10;
    //        }
    //        // Caso esteja com vantagem
    //        fitness = difGolos * 100 * Math.Abs(avgSpeed);

    //    }
    //    return fitness;
    //}
    // Estávamos a usar este
    //public float GetScore()
    //{
    //    int fatorGolo = 10;
    //    float fitness;
    //    fitness =  GoalsOnAdversaryGoal -  GoalsOnMyGoal;
    //    if (fitness < 0)
    //    {
    //        return (float)Math.Pow(fitness, 2);
    //    }

    //    return fitness*fatorGolo*(1/(distanceToBall/10000)) * (1 / (distanceTravelled / 10000));
    //}





    //Original está em baixo
    //public float GetScore() {
    //       Debug.Log("Estou na função de fitness");
    //       Debug.Log("Distancia até à baliza adversária"+distancefromBallToAdversaryGoal);
    //       Debug.Log("Distancia até à bola" + distanceToBall);
    //       // Fitness function. The code to attribute fitness to individuals should be written here.  
    //       float fitness = 10 / ((distanceToBall / 1000) + (distanceToAdversaryGoal + distancefromBallToAdversaryGoal));

    //       if (hitTheBall>0)
    //       {
    //           fitness = fitness * 10;
    //       }

    //       return fitness;
    //}


    //public float GetScore()
    //{
    //    // Fitness function. The code to attribute fitness to individuals should be written here.  
    //    float fitness = driveTime * distanceTravelled;
    //    return fitness;
    //}




    public void wrapUp () {
		avgSpeed = avgSpeed / driveTime;
		gameOver = true;
		running = false;
        Debug.Log("Game Over..");       
	}		



}