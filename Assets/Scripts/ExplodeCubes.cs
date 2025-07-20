
using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{
    public GameObject explosionPar;
    private bool _collisionSet; 
    
    private void OnCollisionEnter(Collision collision)
    { 
        
        if(collision.gameObject.tag=="Cube"&&!_collisionSet)
        {
            for(int i=collision.transform.childCount-1; i>=0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(170f, Vector3.up, 5f);
                child.SetParent(null);
            }
            GameObject newVfx=Instantiate(explosionPar, new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.contacts[0].point.z), Quaternion.identity) as GameObject;
            Destroy(newVfx, explosionPar.GetComponent<ParticleSystem>().main.startLifetime.constantMax);      
            Camera.main.gameObject.AddComponent<CameraShake>();
            Destroy(collision.gameObject);
            _collisionSet = true;  
        }
    }
}
