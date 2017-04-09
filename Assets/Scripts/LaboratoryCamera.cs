using UnityEngine;
using System.Collections;

public class LaboratoryCamera : MonoBehaviour 
{
	private Vector3 cameraPivot;
	private Quaternion cameraRotation, saveRotation;

	float slerpParam, resetTimer;
	const float RESET_LENGTH = 2f;
	const float SLERP_SPEED = 0.05f;

	const float MAX_FOV = 60, MIN_FOV = 10;
	const float FOV_SCROLL = -15f;
	const float FOV_LERP_SPEED = -1f/FOV_SCROLL;
	float fovLerp = 0;
	
	private GameObject focusTarget;

	void Awake()
	{
		cameraPivot = this.transform.position;
		cameraRotation = this.transform.rotation;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*if(Input.GetMouseButton(1))
		{
			fovLerp += FOV_LERP_SPEED * Time.deltaTime;
			resetTimer = RESET_LENGTH;
		}
		else if(resetTimer < 0)
			fovLerp -= 2 * FOV_LERP_SPEED * Time.deltaTime;

		fovLerp = Mathf.Clamp (fovLerp, 0f, 1f);
		GetComponent<Camera> ().fieldOfView = Mathf.Lerp (MAX_FOV, MIN_FOV, fovLerp);*/

		GetComponent<Camera> ().fieldOfView += Input.GetAxis ("Mouse ScrollWheel") * FOV_SCROLL;
		GetComponent<Camera> ().fieldOfView = Mathf.Clamp (GetComponent<Camera> ().fieldOfView, MIN_FOV, MAX_FOV);

		/*if(Input.GetKey(KeyCode.LeftShift))
		{
			if(focusTarget != null)
			{
				this.transform.rotation = Quaternion.Slerp
					(this.transform.rotation, Quaternion.LookRotation
					 (focusTarget.transform.position - this.transform.position), slerpParam);
				slerpParam += Time.deltaTime * SLERP_SPEED;
				//transform.LookAt(focusTarget.transform.position);
			}
		}
		else
		{
			float x = Input.GetAxisRaw ("Mouse X");
			float y = Input.GetAxisRaw ("Mouse Y");
			if(x != 0 || y != 0)
			{
				Quaternion yaw = Quaternion.AngleAxis (x, this.transform.up);
				Quaternion pitch = Quaternion.AngleAxis(-y, this.transform.right);
				this.transform.rotation = yaw * pitch * transform.rotation;
				saveRotation = this.transform.rotation;
				slerpParam = 0;
				resetTimer = RESET_LENGTH;
			}

			if(resetTimer < 0)
			{
				slerpParam += Time.deltaTime * SLERP_SPEED;
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, cameraRotation, slerpParam);
				GetComponent<Camera> ().fieldOfView = 
					Mathf.Lerp (GetComponent<Camera> ().fieldOfView, MAX_FOV, slerpParam*FOV_LERP_SPEED);
			}
			else
				resetTimer -= Time.deltaTime;
		}*/
	}

	public void SetFocus(GameObject go)
	{
		slerpParam = 0;
		focusTarget = go;
	}
}
