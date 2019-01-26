using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishPropel : MonoBehaviour {

	public static List<FishPropel> FishList = new List<FishPropel>();

	public float maxSpeed;
	public Vector3 velocity;
	public Vector3 heading;
	public int MAX_NEIGHBOURS;
	
	public int FISH_MAX_DISTANCE =150;
	public int FISH_MIN_DISTANCE = 15;
	public int FISH_MAX_SPEED = 20;


	public bool Activated;

	public double FastAIInterval;
	public double SlowAIInterval;

	double SlowAITime;
	double FastAITime;

	public List<GameObject> neighbours;
	public List<GameObject> potentialNeighbours;

	void Awake()
	{
		FishList.Add(this);
	}

	// Use this for initialization
	void Start () 
	{
		SlowAITime = 5;
		FastAITime = FastAIInterval;
		neighbours = new List<GameObject>();
		potentialNeighbours = new List<GameObject>();

		velocity = new Vector3(0,0,0.1f);

	}
	
	// Update is called once per frame
	void Update () 
	{
		SlowAITime -= Time.deltaTime;
		FastAITime -= Time.deltaTime;

		if (FastAITime <0)
		{
			ProcessFastAi(Time.deltaTime);
			FastAITime = FastAIInterval;
		}

		if (SlowAITime <0)
		{
			ProcessSlowAI();
			SlowAITime = FastAIInterval;
		}

		//transform.LookAt ( new Vector3(heading.x,heading.y,heading.z));
		//velocity = heading;
		transform.position += velocity  *( Time.deltaTime) ;
		transform.forward=  velocity;

		if (transform.position.x < -500)
			velocity.x = velocity.x * -1;
		if (transform.position.x > 500)
			velocity.x = velocity.x * -1;
		if (transform.position.y < -500)
			velocity.y = velocity.y * -1;
		if (transform.position.y > 500)
			velocity.y = velocity.y * -1;
		if (transform.position.z < -500)
			velocity.z = velocity.z * -1;
		if (transform.position.z > 500)
			velocity.z = velocity.z * -1;
	}

	void ProcessFastAi(float interval)
	{
		Vector3 WA = Vector3.zero;
		Vector3 cohVect = Cohesion();
		Vector3 sepVect = Separation();
		Vector3 algnVect = Align();
		Vector3 targetVelocity = Vector3.zero;

		WA =  cohVect*0.2f + sepVect*0.5f + algnVect*0.3f;
		WA.Normalize();
		targetVelocity = WA * FISH_MAX_SPEED;

		Vector3 difference = targetVelocity - velocity;

		float accel = 10 * interval;

		if (difference.magnitude > accel)
		{
			difference = difference/difference.magnitude * accel;
		}

		velocity += difference;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "BaseFish")
		{
			if (potentialNeighbours.Contains(other.gameObject))
			    return;

			potentialNeighbours.Add(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "BaseFish")
		{
			potentialNeighbours.Remove(other.gameObject);
		}
	}	

	void ProcessSlowAI()
	{
		int i;
		float dist;
		SortedList<float,GameObject> tempList = new SortedList<float,GameObject>();

		for (i=0;i<potentialNeighbours.Count;i++)
		{
			dist = (potentialNeighbours[i].transform.position - transform.position).magnitude;
			tempList.Add(dist,potentialNeighbours[i]);
		}

		neighbours.Clear();

		for (i = 0;i < Mathf.Min (MAX_NEIGHBOURS, tempList.Count);i++)
		{
			neighbours.Add(tempList.Values[i]);
		}


		/*neighbours.Clear();
		for (i =0; i<FishList.Count;i++)
		{
			if ((FishList[i].transform.position - transform.position).magnitude < FISH_MAX_DISTANCE)
			{
				neighbours.Add(FishList[i].gameObject);
			}
		}*/

	}

	Vector3 Cohesion()
	{
		Vector3 temp = Vector3.zero;
		float tempDist ;
		Vector3 final = Vector3.zero;

		int i;

		for (i=0;i<neighbours.Count;i++)
		{
			if (neighbours[i].gameObject == this.gameObject)
				continue;

			temp =  neighbours[i].transform.position - transform.position;
			tempDist = temp.magnitude;
			if (tempDist <= FISH_MAX_DISTANCE)
			{
			 	final += ((FISH_MAX_DISTANCE-tempDist)/FISH_MAX_DISTANCE) * (neighbours[i].transform.position- transform.position)/tempDist;
			//((FISH_NEIGHBOR_MAX_DISTANCE - d)/FISH_NEIGHBOR_MAX_DISTANCE) * (school[neighbors[i]]->position - position)/d;
			}
		}
		return final;
	}

	Vector3 Separation()
	{
		Vector3 temp = Vector3.zero;
		float tempDist ;
		Vector3 final = Vector3.zero;
		
		int i;
		
		for (i=0;i<neighbours.Count;i++)
		{
			if (neighbours[i].gameObject == this.gameObject)
				continue;
			
			temp =  neighbours[i].transform.position - transform.position;
			tempDist = temp.magnitude;
			if (tempDist < FISH_MIN_DISTANCE )
			{
				final += -((FISH_MIN_DISTANCE-tempDist)/FISH_MIN_DISTANCE) * (neighbours[i].transform.position- transform.position)/tempDist;
			//((FISH_NEIGHBOR_MAX_DISTANCE - d)/FISH_NEIGHBOR_MAX_DISTANCE) * (school[neighbors[i]]->position - position)/d;
			}
		}
		return final;
	}

	Vector3 Align()
	{
		Vector3 temp = Vector3.zero;
		float tempDist ;
		Vector3 final = Vector3.zero;
		
		int i;
		
		for (i=0;i<neighbours.Count;i++)
		{
			if (neighbours[i].gameObject == this.gameObject)
				continue;
			
			temp =  neighbours[i].transform.position - transform.position;
			tempDist = temp.magnitude;
			if (tempDist <= FISH_MAX_DISTANCE)
			{
				final += ((FISH_MAX_DISTANCE-tempDist)/FISH_MAX_DISTANCE) * (neighbours[i].transform.forward- velocity)/tempDist;
				//((FISH_NEIGHBOR_MAX_DISTANCE - d)/FISH_NEIGHBOR_MAX_DISTANCE) * (school[neighbors[i]]->position - position)/d;
			}
		}
		return final;
	}
}
