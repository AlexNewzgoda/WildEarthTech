using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildControl : MonoBehaviour
{
    public GameObject BlockChost;

    public Transform raystart;
    public GameObject BuildBlock;
    public float RayDist = 1;
    public LayerMask layerMask;

    public bool EnableBuild;
    public Material Red;
    public Material Green;

    private GameObject SnapedObj;

    public GameObject UI;

    private MountPoint lastpoint;

    public Vector3 RotateOffset;
    public Vector3 RotateStep;

    public Transform target;
    void Start()
    {
        BlockChost.transform.parent = null;
    }

    //������� �������
    public void EnableBuildSet()
    {
        EnableBuild = true;
        BlockChost.GetComponent<BlockGhost>().SetMaterials(Green);
    }
    public void DisableBuildSet()
    {
        EnableBuild = false;
        BlockChost.GetComponent<BlockGhost>().SetMaterials(Red);
    }


    //������� ������ � ��� �� ������
    void Update()
    {
        //�������� � ��������� ����� ������� ����� ������ ����� �� �������
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(BuildBlock == null)
            {          
                UI.SetActive(true);
            }
            else
            {
                BuildBlock = null;
                BlockChost.SetActive(false);
                target.gameObject.SetActive(false);
                RotateOffset = Vector3.zero;
            } 

        }

        //���� ��� ��������� ���� �� ������ �����
        if(BuildBlock != null)
        {
            target.gameObject.SetActive(true);
            if (BlockChost.activeSelf == false)
            {
                BlockChost.SetActive(true);
                EnableBuildSet();
            }
            if (Physics.Raycast(raystart.position, raystart.forward, out RaycastHit hit, RayDist, layerMask))
            {
                if (hit.transform.gameObject.GetComponent<MountPoint>() != null && hit.transform.gameObject.GetComponent<MountPoint>().MountedBlock == null)
                {
                    lastpoint = hit.transform.gameObject.GetComponent<MountPoint>();
                    target.position = lastpoint.MountPos.position;
                    target.rotation = Quaternion.Euler(lastpoint.MountPos.rotation.eulerAngles.x + RotateOffset.x, lastpoint.MountPos.rotation.eulerAngles.y + RotateOffset.y,
                    lastpoint.MountPos.rotation.eulerAngles.z + RotateOffset.z);
                    SnapedObj = lastpoint.gameObject;
                }
                else
                {
                    lastpoint = null;
                    SnapedObj = null;
                    target.position = hit.point;
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    RotateOffset += RotateStep;
                }

            }
            else
            {
                lastpoint = null;
                target.position = raystart.position + raystart.forward * RayDist;
                SnapedObj = null;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (EnableBuild)
                {
                    Instantiate(BuildBlock, target.position, target.rotation);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if(SnapedObj != null)
                {
                    Destroy(SnapedObj.GetComponentInParent<BlockConfig>().gameObject);
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel")>0)
            {
                RayDist++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                RayDist--;
            }

            BlockChost.transform.position = Vector3.MoveTowards(BlockChost.transform.position, target.position, 20 * Time.deltaTime);
            BlockChost.transform.rotation = Quaternion.Lerp(BlockChost.transform.rotation, target.rotation, 20 * Time.deltaTime);
        }
    }


    //����� ����� ������� ������������� ���� ��� ��������� �������
    public void SetBuildBlock(GameObject block)
    {
        BuildBlock = block;
        GameObject g = Instantiate(block);
        BlockChost.GetComponent<BlockGhost>().meshFilter.mesh = g.GetComponent<BlockConfig>().MainGeometry.mesh;
        target.GetComponent<BoxCollider>().size = BlockChost.GetComponent<BlockGhost>().meshFilter.mesh.bounds.size * 0.9f;
        RotateStep = g.GetComponent<BlockConfig>().RotateStep;
        Destroy(g);
    }
}
