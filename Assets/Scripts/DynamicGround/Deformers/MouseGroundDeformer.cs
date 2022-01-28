using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseGroundDeformer : MonoBehaviour
{
    private float _power = 0;
    public float HeightLastResult = 0;

    public Texture2D BrushTexture;

    public float BrushAngle = 0;
    public float Scale = 1;

    private void Update()
    {
        bool lshift = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _power = Mathf.MoveTowards(_power, 1, Time.deltaTime * 2f);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000, 1<<6, QueryTriggerInteraction.Ignore))
            {
                DeformableGround ground = hit.collider.GetComponent<DeformableGround>();

                if(ground != null)
                {
                    ground.ApplyBrushDeform(hit.textureCoord, BrushAngle, Scale, BrushTexture);
                }
            }
        }
        else
        {
            _power = 0;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, 1<<6, QueryTriggerInteraction.Ignore))
            {
                DeformableGround ground = hit.collider.GetComponent<DeformableGround>();

                if (ground != null)
                {
                    HeightLastResult = ground.GetHeight(hit.textureCoord, 0.1f);
                    Debug.Log("fok: " + HeightLastResult);
                }
            }
        }

        float wheelDelta = Input.GetAxis("Mouse ScrollWheel");

        if (wheelDelta > 0)
        {
            BrushAngle += 1;
        }

        if (wheelDelta < 0)
        {
            BrushAngle -= 1;
        }

        //BrushAngle += Time.deltaTime * 180f;

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, 1 << 6))
            {
                DeformableGround ground = hit.collider.GetComponent<DeformableGround>();

                if (ground != null)
                {
                    ground.ApplyBrushDeform(hit.textureCoord, BrushAngle, Scale, BrushTexture);
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Height: " + HeightLastResult);
    }
}
