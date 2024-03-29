using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private Transform spine;
    [SerializeField] private Transform gunPosition;
    [SerializeField] private int bulletCost;

    public AudioSource ShotSongs;


    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private BaseManager manager;

    [SerializeField]
    private float shootTime;

    private void Awake() {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        manager = BaseManager.instance;
    }

    private void Update() {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

        if (starterAssetsInputs.aim) {
           
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 13f));

            Vector3 worldAimTarget = mouseWorldPosition;
            Vector3 spineWorldAimTarget = mouseWorldPosition;

            worldAimTarget.y = transform.position.y;
           // spineWorldAimTarget.y = spine.transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
           // Vector3 spineAimDirection = (spineWorldAimTarget - spine.transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            //  spine.transform.forward = Vector3.Lerp(spine.transform.forward, spineAimDirection, Time.deltaTime * 20f);

           
        } 
        
        else 
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 13f));
        }


        if (starterAssetsInputs.shoot&&starterAssetsInputs.aim) 
        
        {
            if (manager.currentBattery>=bulletCost)
            {
                Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
                Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
                //*/
                starterAssetsInputs.shoot = false;
                
                ShotSongs.pitch = (Random.Range(0.7f, 0.9f));
                ShotSongs.Play();
                manager.currentBattery -= bulletCost;
                shootTime = Time.time + 3;
            }

       

        }
        else
        {
            starterAssetsInputs.shoot = false;

        }

        if (shootTime < Time.time)
        {
            starterAssetsInputs.aim = false;
        }
      
        

    }

    


    private void LateUpdate()
    {
        if (starterAssetsInputs.aim)
        {
            var rotationVector = spine.transform.rotation.eulerAngles;
            rotationVector.y += 30;
            spine.transform.rotation = Quaternion.Euler(rotationVector);
        }
       
    }

}