using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishPropel : MonoBehaviour {

	public static List<FishPropel> FishList = new List<FishPropel>();
	public BaseFish meFish;

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
	private SortedList<float,GameObject> tempList;

	void Awake()
	{
		FishList.Add(this);
	}

	// Use this for initialization
	void Start () 
	{
		SlowAITime = Random.value*5;
		FastAITime = FastAIInterval;
		neighbours = new List<GameObject>();
		potentialNeighbours = new List<GameObject>();
		tempList = new SortedList<float, GameObject> ();

		velocity = new Vector3(((Random.value*50)/50)-0.5f,((Random.value*50)/50)-0.5f,((Random.value*50)/50)-0.5f);
		if (velocity.magnitude == 0) 
		{
			velocity = new Vector3(.5f,.5f,.5f);
		}
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
			SlowAITime = SlowAIInterval;
		}

		//transform.LookAt ( new Vector3(heading.x,heading.y,heading.z));
		//velocity = heading;
		if (velocity.magnitude >0)
		{
			transform.position += velocity  *( 0.01f +Time.deltaTime) ;
			transform.forward=  velocity;
		}

		if (transform.position.x < 0)
			velocity.x = Mathf.Abs (velocity.x) * 1;
		if (transform.position.x > 1000)
			velocity.x = Mathf.Abs (velocity.x) * -1;
		if (transform.position.y < 0)
			velocity.y = Mathf.Abs (velocity.y) * 1;
		if (transform.position.y > 1000)
			velocity.y = Mathf.Abs (velocity.y) * -1;
		if (transform.position.z < 0)
			velocity.z = Mathf.Abs (velocity.z) * 1;
		if (transform.position.z > 1000)
			velocity.z = Mathf.Abs (velocity.z) * -1;
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

		float accel = 50  * interval;

		if (difference.magnitude > accel)
		{
			difference = (difference/difference.magnitude) * accel;
		}
		if (difference.magnitude > 0.5f) 
		{
			velocity += difference;
				
		} 
		else 
		{
			velocity += transform.forward*.1f;				
		}

	}
	/*
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
	}	*/

	void ProcessSlowAI()
	{
		int i;
		float dist;
		tempList.Clear();

		GameObject N1 = null;
		GameObject N2 = null;
		GameObject N3 = null;
		GameObject N4 = null;

		float n1dist = float.MaxValue;
		float n2dist = float.MaxValue;
		float n3dist = float.MaxValue;
		float n4dist = float.MaxValue;

		for (i=0;i<FishList.Count;i++)
		{
			if ((FishList[i].gameObject == this)||(!meFish.isAlive))
				continue;
			dist = (FishList[i].transform.position - transform.position).magnitude;
			//tempList.Add(dist,FishList[i].gameObject);
			if (dist < n4dist)
			{
				if (dist < n3dist)
				{
					if(dist < n2dist) 
					{
						if (dist < n1dist) 
						{
							n4dist = n3dist;
							N4 = N3;

							n3dist = n2dist;
							N3 = N2;

							n2dist = n1dist;
							N2 = N1;

							n1dist= dist;
							N1 = FishList[i].gameObject;
						}
						else
						{
							n4dist = n3dist;
							N4 = N3;
							
							n3dist = n2dist;
							N3 = N2;
							
							n2dist = dist;
							N2 = FishList[i].gameObject;
						}
					}
					else
					{
						n4dist = n3dist;
						N4 = N3;
						
						n3dist = dist;
						N3 = FishList[i].gameObject;
					}
				}
				else
				{
					n4dist = dist;
					N4 = FishList[i].gameObject;

				}			 	
			}
			else
			{
				FISH_MIN_DISTANCE = Random.Range(10,30);
				//FISH_MAX_SPEED = Random.Range(15,20);
			}
		}

		neighbours.Clear();

		for (i = 0;i < Mathf.Min (MAX_NEIGHBOURS, 4);i++)
		{
			//neighbours.Add(tempList.Values[i]);
			if (!(N1 == null))
				neighbours.Add(N1);
			if (!(N2 == null))
				neighbours.Add(N2);
			if (!(N3 == null))
				neighbours.Add(N3);
			if (!(N4 == null))
				neighbours.Add(N4);
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
			if ((neighbours[i].gameObject == this.gameObject)|| (neighbours[i].gameObject == null))
				continue;

			temp =  neighbours[i].transform.position - transform.position;
			tempDist = temp.magnitude;
			if (tempDist <= FISH_MAX_DISTANCE)
			{
			 	final += ((FISH_MAX_DISTANCE-tempDist)/FISH_MAX_DISTANCE) * (neighbours[i].transform.position- transform.position)/tempDist;
			//((FISH_NEIGHBOR_MAX_DISTANCE - d)/FISH_NEIGHBOR_MAX_DISTANCE) * (school[neighbors[i]]->position - position)/d;
			}
		}

		if (neighbours.Count == 0) 
		{
			final = velocity;		
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
			if ((neighbours[i].gameObject == this.gameObject)|| (neighbours[i].gameObject == null))
				continue;
			
			temp =  neighbours[i].transform.position - transform.position;
			tempDist = temp.magnitude;
			if (tempDist < FISH_MIN_DISTANCE )
			{
				final += -((FISH_MIN_DISTANCE-tempDist)/FISH_MIN_DISTANCE) * (neighbours[i].transform.position- transform.position)/tempDist;
			//((FISH_NEIGHBOR_MAX_DISTANCE - d)/FISH_NEIGHBOR_MAX_DISTANCE) * (school[neighbors[i]]->position - position)/d;
			}
		}
		if (neighbours.Count == 0) 
		{
			final = velocity;		
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
			if ((neighbours[i].gameObject == this.gameObject)|| (neighbours[i].gameObject == null))
				continue;
			
			temp =  neighbours[i].transform.position - transform.position;
			tempDist = temp.magnitude;
			if (tempDist <= FISH_MAX_DISTANCE)
			{
				final += ((FISH_MAX_DISTANCE-tempDist)/FISH_MAX_DISTANCE) * (neighbours[i].transform.forward);//- velocity)/tempDist;
				//((FISH_NEIGHBOR_MAX_DISTANCE - d)/FISH_NEIGHBOR_MAX_DISTANCE) * (school[neighbors[i]]->position - position)/d;
			}
		}
		if (neighbours.Count == 0) 
		{
			final = velocity;		
		}
		return final;
	}
}
