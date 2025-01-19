using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem   ;

public class sliceComponent : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public LayerMask sliceableLayer;
    public Material crossSectionMaterial;
    public VelocityEstimator velocityEstimator;
    public float cutForce = 200;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if(hasHit)
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }
    }
    public void Slice(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if(hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull);
            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull);
            upperHull.layer = target.layer;
            lowerHull.layer = target.layer;

            Destroy(target);
        }

    }
    public void SetupSlicedComponent(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }
}