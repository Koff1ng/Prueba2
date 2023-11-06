using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAZombi : MonoBehaviour {
    public Transform jugador;
    public enum TipoMerodeo { Aleatorio, PuntoDeReferencia};
    public TipoMerodeo tipoMerodeo = TipoMerodeo.Aleatorio;
    public float velocidadMerodeo = 2.8f;
    public float velocidadPersecucion = 4f;
    public float campoDeVista = 120f;
    public float distanciaDeVista = 10f;
    public float rangoDeMerodeo = 7f;
    public Transform[] puntosDeReferencia;

    private int indicePuntoDeReferencia = 0;
    private bool consciente = false;
    private NavMeshAgent agente;
    private Renderer renderizador;
    private Vector3 puntoDeMerodeo;
    private Animator animator;

    public void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        renderizador = GetComponent<Renderer>();
        puntoDeMerodeo = CalcularPuntoAleatorio();
        animator = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        if (consciente)
        {
            //Perseguir/atacar al jugador
            agente.SetDestination(jugador.position);
            agente.speed = velocidadPersecucion;
            animator.SetBool("Alerta", true);
        } else
        {
            animator.SetBool("Alerta", false);
            agente.speed = velocidadMerodeo;
            BuscarJugador();
            Merodear();
        }
    }

    public void BuscarJugador()
    {
        if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(jugador.position)) < campoDeVista / 2f)
        {
            if (Vector3.Distance(transform.position, jugador.position) < distanciaDeVista)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, jugador.position, out hit))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        HacerConsciente();
                    }
                }
                
            }
        }
    }

    public void Merodear()
    {
        if (tipoMerodeo == TipoMerodeo.Aleatorio)
        {
            if (Vector3.Distance(puntoDeMerodeo, transform.position) < 2f)
            {
                puntoDeMerodeo = CalcularPuntoAleatorio();
            }
            else
            {
                agente.SetDestination(puntoDeMerodeo);
            }
        } else
        {
            if (puntosDeReferencia.Length >= 2)
            {
                if (Vector3.Distance(transform.position, puntosDeReferencia[indicePuntoDeReferencia].position) < 2f)
                {
                    if (indicePuntoDeReferencia == puntosDeReferencia.Length - 1)
                    {
                        indicePuntoDeReferencia = 0;
                    }
                    else
                    {
                        indicePuntoDeReferencia++;
                    }
                } else
                {
                    agente.SetDestination(puntosDeReferencia[indicePuntoDeReferencia].position);
                }
            } else
            {
                Debug.LogWarning("No hay suficientes puntos de referencia, por favor asignar por lo menos 2");
            }
        }
    }

    public void HacerConsciente()
    {
        consciente = true;
    }

    public Vector3 CalcularPuntoAleatorio()
    {
        Vector3 puntoAleatorio = (Random.insideUnitSphere * rangoDeMerodeo) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(puntoAleatorio, out navHit, rangoDeMerodeo, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }
}
