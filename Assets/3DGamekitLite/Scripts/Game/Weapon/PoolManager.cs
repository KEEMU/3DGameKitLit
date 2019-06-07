using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    public Dictionary<SphereAttack.WeaponType, List<GameObject>> projectilePool;
    public Dictionary<SphereAttack.WeaponType, List<GameObject>> muzzlePool;
    public Dictionary<SphereAttack.WeaponType, List<GameObject>> impactPool;
    

    private void Awake()
    {
        instance = this;
        projectilePool = new Dictionary<SphereAttack.WeaponType, List<GameObject>>();
        muzzlePool = new Dictionary<SphereAttack.WeaponType, List<GameObject>>();
        impactPool = new Dictionary<SphereAttack.WeaponType, List<GameObject>>();
    }

    public GameObject RequestProjectile(SphereAttack.WeaponType weaponType,Transform obj,Vector3 pos,Quaternion rot)
    {
        List<GameObject> li;
        projectilePool.TryGetValue(weaponType, out li);
        if (li == null)
        {
            li = new List<GameObject>();
            projectilePool.Add(weaponType, li);
        }
        for (int i = 0; i < li.Count; i++)
        {
            if (!li[i].activeSelf)
            {
                li[i].transform.position = pos;
                li[i].transform.rotation = rot;
                li[i].gameObject.SetActive(true);
                li[i].GetComponent<SeekerProjectile>().OnSpawned();
                return li[i].gameObject;
            }
        }
        GameObject newObj = Instantiate(obj.gameObject, pos, rot);
        li.Add(newObj);
        newObj.SetActive(true);
        newObj.GetComponent<SeekerProjectile>().OnSpawned();
        return newObj;
    }

    public GameObject RequestMuzzle(SphereAttack.WeaponType weaponType, Transform obj, Vector3 pos, Quaternion rot)
    {
        List<GameObject> li;
        muzzlePool.TryGetValue(weaponType, out li);
        if (li == null)
        {
            li = new List<GameObject>();
            muzzlePool.Add(weaponType, li);
        }
        for (int i = 0; i < li.Count; i++)
        {
            if (!li[i].activeSelf)
            {
                li[i].transform.position = pos;
                li[i].transform.rotation = rot;
                li[i].gameObject.SetActive(true);
                li[i].gameObject.GetComponent<Despawn>().OnSpawned();
                return li[i].gameObject;
            }
        }
        GameObject newObj = Instantiate(obj.gameObject, pos, rot);
        newObj.SetActive(true);
        li.Add(newObj);
        newObj.GetComponent<Despawn>().OnSpawned();
        return newObj;
    }

    public GameObject RequestImpact(SphereAttack.WeaponType weaponType, Transform obj, Vector3 pos, Quaternion rot)
    {
        List<GameObject> li;
        impactPool.TryGetValue(weaponType, out li);
        if (li == null)
        {
            li = new List<GameObject>();
            impactPool.Add(weaponType, li);
        }
        for (int i = 0; i < li.Count; i++)
        {
            if (!li[i].activeSelf)
            {
                li[i].transform.position = pos;
                li[i].transform.rotation = rot;
                li[i].gameObject.SetActive(true);
                return li[i].gameObject;
            }
        }
        GameObject newObj = Instantiate(obj.gameObject, pos, rot);
        li.Add(newObj);
        newObj.SetActive(true);
        newObj.GetComponent<Despawn>().OnSpawned();
        return newObj;
    }
}
