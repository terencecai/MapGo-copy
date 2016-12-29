using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private LocationManager locManager;
        private GameObject tavern;

        [SerializeField] public GameObject mainTavern;

        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )

            locManager = GameObject.Find("World").GetComponent<LocationManager>();
            m_Character = GetComponent<ThirdPersonCharacter>();
        }

        float doubleClickStart = 0;
        void OnMouseUp()
        {
            if ((Time.time - doubleClickStart) <= 0.3f)
            {
                doubleClickStart = -1;
                this.OnDoubleClick(); 
            }
            else
            {
                doubleClickStart = Time.time;
            }
        }

        void OnDoubleClick()
        {
            Debug.Log("Double Clicked!");
            var inside = UnityEngine.Random.insideUnitCircle * 40;
            var newPos = new Vector3(transform.position.x + inside.x, 0, transform.position.z + inside.y);

            tavern = Instantiate(mainTavern, newPos, mainTavern.transform.rotation);
            tavern.transform.localScale = new Vector3(1f, 1f, 1f);
            tavern.SetActive(true);
        }


        private void Update()
        {

            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInput.CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInput.CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif
            // pass all parameters to the character control script
            //m_Character.MoveTowardsTarget(target);

            m_Character.MoveTowardsTarget(locManager.getDirection());




        }
    }
}
