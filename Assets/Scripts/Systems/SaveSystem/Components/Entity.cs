using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

public class Entity : MonoBehaviour
{
    public static Entity GetEntityByID(string id)
    {
        return GameSerializer.GetEntityByID(id);
    }
   
    public string ID = null;
    public AssetReference PrefabReference;
    public Component[] SerializableComponents;

    [System.Serializable]
    public struct PropData
    {
        public string PrefabGUID;
        public string ID;
        public bool Active;
        public Vector3 Pos;
        public Quaternion Rot;
        public string[] CmpsData;
    }

    public static string GetNewGUID()
    {
        return System.Guid.NewGuid().ToString();
    }

    public void SetNewGUID()
    {
        ID = GetNewGUID();
    }

    public virtual string GetData ()
    {
        PropData data = new PropData()
        {
            PrefabGUID =  this.PrefabReference.AssetGUID,
            ID = this.ID,
            Active = this.gameObject.activeSelf,
            Pos = this.transform.position,
            Rot = this.transform.rotation,
            CmpsData = null
        };

        if (SerializableComponents != null)
        {
            data.CmpsData = new string[SerializableComponents.Length];

            for (int c = 0; c < SerializableComponents.Length; c++)
            {
                ISaveSerialized cmp;
                if(SerializableComponents[c].TryGetComponent(out cmp))
                {
                    data.CmpsData[c] = cmp.GetSavedData();
                }
            }
        }
        
        return JsonUtility.ToJson(data);
    }

    public virtual void SetData (PropData propData)
    {
        PrefabReference = new AssetReference(propData.PrefabGUID);
        ID = propData.ID;
        this.transform.position = propData.Pos;
        this.transform.rotation = propData.Rot;
        this.gameObject.SetActive(propData.Active);

        if (propData.CmpsData != null)
        {
            for (int c = 0; c < propData.CmpsData.Length; c++)
            {
                SerializableComponents[c].GetComponent<ISaveSerialized>().SetSavedData(propData.CmpsData[c]);
            }
        }
    }

    private void Awake()
    {
        GameSerializer.OnAddEntity(this);
    }

    private void OnDestroy()
    {
        GameSerializer.OnRemoveEntity(this);
    }
}
