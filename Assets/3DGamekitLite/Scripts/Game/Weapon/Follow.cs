using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform m_followTarget;
    private Vector3 m_offset;
    // Start is called before the first frame update
    void Start()
    {
        m_offset = transform.position - m_followTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_followTarget.position + m_offset;
    }
}
