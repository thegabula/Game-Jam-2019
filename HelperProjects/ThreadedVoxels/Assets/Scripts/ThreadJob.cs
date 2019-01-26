using UnityEngine;
using System.Collections;

public class ThreadedJob
{
	private bool m_IsDone1 = false;
	private bool m_IsDone2 = false;
	private object m_Handle = new object();
    private System.Threading.Thread m_Thread1 = null;
	private System.Threading.Thread m_Thread2 = null;
	public bool IsDone1
	{
		get
		{
			bool tmp;
			lock (m_Handle)
			{
				tmp = m_IsDone1;
			}
			return tmp;
		}
		set
		{
			lock (m_Handle)
			{
				m_IsDone1 = value;
			}
		}
	}
	public bool IsDone2
	{
		get
		{
			bool tmp;
			lock (m_Handle)
			{
				tmp = m_IsDone2;
			}
			return tmp;
		}
		set
		{
			lock (m_Handle)
			{
				m_IsDone2 = value;
			}
		}
	}
	
	public virtual void Start()
	{
		m_Thread1 = new System.Threading.Thread(RunThreadOne);
		m_Thread1.Start();
		IsDone1 = false;
    }
	public virtual void Start2()
	{
		m_Thread2 = new System.Threading.Thread (RunThreadTwo);
		m_Thread2.Start ();
		IsDone2 = false;
	}

	public virtual void Abort1()
	{
		m_Thread1.Abort();
	}
	public virtual void Abort2()
	{
		m_Thread2.Abort();
	}

	protected virtual void ThreadFunction1() { }
	protected virtual void ThreadFunction2() { }

	protected virtual void OnFinished() { }
	protected virtual void OnFinished2() { }
	
	public virtual bool Update()
	{
		if (IsDone1)
		{
			OnFinished();
			return true;
		}
		return false;
	}
	public virtual bool Update2()
	{
		if (IsDone2)
		{
			OnFinished2();
			return true;
		}
		return false;
	}

	IEnumerator WaitFor()
    {
		while(!Update())
		{
			yield return null;
		}
	}
	private void RunThreadOne()
	{
		ThreadFunction1();
		IsDone1 = true;
	}
	private void RunThreadTwo()
	{
		ThreadFunction2();
		IsDone2 = true;
	}
}

