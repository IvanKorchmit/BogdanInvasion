using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
public class FogOfWar : MonoBehaviour
{
	public GameObject m_fogOfWarPlane;
	public Transform m_player;
	public LayerMask m_fogLayer;
	public LayerMask obstacle_Layer;
	public NativeArray<float3> vert;
	public float wallDistance;
	public float m_radius = 5f;
	private float m_radiusSqr { get { return m_radius * m_radius; } }

	private Mesh m_mesh;
	private Vector3[] m_vertices;
	private Color[] m_colors;

	// Use this for initialization
	void Start()
	{
		Initialize();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		float inc = 1f/4;
		for (float x = -1; x < 1f; x += inc)
		{
			for (float y = -1; y < 1; y += inc)
			{
				CastFog(x, y);
			}
		}
    }

    private void CastFog(float x, float y)
    {
		Vector3 dir = new Vector3(x,0, y);
		dir.Normalize();
		Physics.Raycast(m_player.transform.position, dir, out RaycastHit ray, m_radius, obstacle_Layer);
		Vector3 point = ray.collider != null ? ray.point + ray.normal / wallDistance : m_player.position + (dir * m_radius);
		if (Physics.Raycast(point + Vector3.up * 1000, Vector3.down, out RaycastHit hit, uint.MaxValue, m_fogLayer))
        {
			StartAJob(hit.point);		
			UpdateColor();
        }
    }
	private void StartAJob(Vector3 hit)
    {
		NativeArray<Color> cols = new NativeArray<Color>(m_colors, Allocator.TempJob);
		Matrix4x4 trs = Matrix4x4.identity;
		trs.SetTRS(m_fogOfWarPlane.transform.position, m_fogOfWarPlane.transform.rotation, m_fogOfWarPlane.transform.localScale);
		if (vert.Length < m_vertices.Length)
		{
			vert = new NativeArray<float3>(m_vertices.Length, Allocator.Persistent);
			for (int i = 0; i < vert.Length; i++)
			{
				vert[i] = m_vertices[i];
			}
		}
		FogOfWarJob fow = new FogOfWarJob() { hitPoint = hit, m_colors = cols, m_radiusSqr = m_radiusSqr,
			transform = trs,
			m_vertices = vert, deltaTime = Time.deltaTime};
		JobHandle jh = fow.Schedule(m_colors.Length, 400);	
		jh.Complete();
		m_colors = cols.ToArray();	
		cols.Dispose();
		// huy
    }
    void Initialize()
	{
		m_mesh = m_fogOfWarPlane.GetComponent<MeshFilter>().mesh;
		m_vertices = m_mesh.vertices;
		m_colors = new Color[m_vertices.Length];
		for (int i = 0; i < m_colors.Length; i++)
		{
			m_colors[i] = Color.black;
		}	
		UpdateColor();
	}

	void UpdateColor()
	{
		m_mesh.colors = m_colors;
	}
}


public struct FogOfWarJob : IJobParallelFor
{
	[ReadOnly]
	public float deltaTime;
	[ReadOnly]
	public float m_radiusSqr;
	public NativeArray<Color> m_colors;
	[ReadOnly]
    public float3 hitPoint;
    [ReadOnly]
    public NativeArray<float3> m_vertices;
    public float4x4 transform;
    public void Execute(int index)
    {
        int i = index;
        float3 v = transform.LocalToWorld(m_vertices[i]);
        float dist = Vector3.SqrMagnitude(v - hitPoint);
        if (dist < m_radiusSqr)
        {
            float alpha = Mathf.Min(m_colors[i].a, dist / m_radiusSqr);
            Color c = m_colors[i];
            c.a = alpha;
            m_colors[i] = c;
        }
        else
		{
			if (m_colors[i].a < 0.75f)
			{ 				
				float a = Mathf.Lerp(m_colors[i].a, 1f, deltaTime);
				Color c = m_colors[i];
				c.a = a;
				m_colors[i] = c;
            }
        }

    }

}


public static class Ext
{
	public static float3 WorldToLocal(this float4x4 transform, float3 point)
	{
		return math.transform(math.inverse(transform), point);
	}

	public static float3 LocalToWorld(this float4x4 transform, float3 point)
	{
		return math.transform(transform, point);
	}
}