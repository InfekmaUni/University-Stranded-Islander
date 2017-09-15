
#region Namespaces
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#endregion


public class WaterFlowFREEDemo : MonoBehaviour
{
	Color m_WaterSimple_Color;
	public GameObject m_SimpleWater;

	#region MonoBehaviour

	void Start()
	{

        m_WaterSimple_Color = new Color(1, 1, 1, 1);

        m_SimpleWater.GetComponent<Renderer>().material.SetColor("_Color", m_WaterSimple_Color);
    }

	void Update()
	{
	}

	#endregion // MonoBehaviour
}
