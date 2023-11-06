using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Jugador : MonoBehaviour {
    public AudioClip sonido;
    public float intensidadDeSonido = 15f;
    public float radioTriggerCaminando = 1f;
    public float radioTriggerCorriendo = 2f;
    public LayerMask capaZombi;
    private AudioSource audioSource;
    private FirstPersonController fpsc;
    private SphereCollider trigger;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fpsc = GetComponent<FirstPersonController>();
        trigger = GetComponent<SphereCollider>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Disparar();
        }
        if (fpsc.ObtenerPerfilDeSigilo() == 0)
        {
            trigger.radius = radioTriggerCaminando;
        } else
        {
            trigger.radius = radioTriggerCorriendo;
        }
    }

    public void Disparar()
    {
        audioSource.PlayOneShot(sonido);
        Collider[] zombis = Physics.OverlapSphere(transform.position, intensidadDeSonido, capaZombi);
        for (int i = 0; i < zombis.Length; i++)
        {
            zombis[i].GetComponent<IAZombi>().HacerConsciente();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zombi"))
        {
            other.GetComponent<IAZombi>().HacerConsciente();
        }
    }
}
