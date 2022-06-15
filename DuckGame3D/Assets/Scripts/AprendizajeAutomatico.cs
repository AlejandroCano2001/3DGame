using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using weka.classifiers.trees;
using weka.classifiers.evaluation;
using weka.core;
using java.io;
using java.lang;
using java.util;
using weka.classifiers.functions;
using weka.classifiers;
using weka.core.converters;

public class AprendizajeAutomatico : MonoBehaviour
{
    weka.classifiers.trees.M5P saberPredecirFuerzaX;
    weka.core.Instances casosEntrenamiento;
    //Text texto;
    private string ESTADO = "Sin conocimiento";
    public GameObject bullet;
    GameObject bulletInstance, PuntoObjetivo;
    float distanciaObjetivo, mejorFuerzaZ, mejorFuerzaX;
    public float valorMaximoFz = 10, pasoFz;
    float Fy_calculada, valorMaximoFy = 5;                                                //Es un ejemplo: Se asume que este valor es extremo para ese problema
    Rigidbody r;
    private Rigidbody rbCar;
    public Transform pista;
    public GameObject car;
    public bool alcanzable = false;

    float valor_calculada_por_metodo_simple(float valorMaximoFy)                        //Calcula una “Fy válida” usando algún método simple.
    {
        float minimoValor = 1f;
        /*Ejemplo:*/
        float valorFactible = (minimoValor + valorMaximoFy) / 2f;                 //Por ejemplo, la fuerza media entre el mínimo y el máximo.
        return valorFactible;
    }
    void Start()
    {
        Fy_calculada = valor_calculada_por_metodo_simple(valorMaximoFy);             //Se va a aprender Fz, hay que seleccionar Fy factible
        //texto = Canvas.FindObjectOfType<Text>();

        rbCar = car.GetComponent<Rigidbody>();

        if (ESTADO == "Sin conocimiento") StartCoroutine("Entrenamiento");          //Lanza el proceso de entrenamiento

    }

    IEnumerator Entrenamiento()
    {
        //casosEntrenamiento = new weka.core.Instances(new java.io.FileReader("Assets/Experiencias.arff"));  //Lee fichero con las variables y experiencias
        casosEntrenamiento = new weka.core.Instances(new java.io.FileReader("Assets/Finales_Experiencias.arff"));  //Lee fichero con las variables y experiencias

        if (casosEntrenamiento.numInstances() < 10)
            for (float Fz = 1; Fz <= valorMaximoFz; Fz = Fz + pasoFz)                       //BUCLE de planificación de la fuerza Fz durante el entrenamiento
            {
                bulletInstance = Instantiate(bullet) as GameObject;
                Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();                    //Crea una bala
                rb.AddRelativeForce(new Vector3(0, Fy_calculada, -Fz), ForceMode.Impulse);  //Se lanza con esa fuerza Fz
                yield return new WaitUntil(() => (rb.transform.position.y <= 7.55f));       //Espera a que la pelota llegue al suelo

                Instance casoAaprender = new Instance(casosEntrenamiento.numAttributes());
                
                casoAaprender.setDataset(casosEntrenamiento);                           //crea un registro de experiencia
                casoAaprender.setValue(0, Fz);                                          //guarda el dato de la fuerza utilizada
                casoAaprender.setValue(1, rb.transform.position.z);                     //anota la distancia alcanzada
                casosEntrenamiento.add(casoAaprender);                                  //guarda el registro de experiencia 
                rb.isKinematic = true; rb.GetComponent<Collider>().isTrigger = true;    //...opcional: paraliza la pelota
                Destroy(bulletInstance, 1f);                                           //...opcional: destruye la pelota en 1 seg para que ver donde cayó.            
            }                                                                           //FIN bucle de lanzamientos con diferentes de fuerzas

        //APRENDIZADE CONOCIMIENTO:  
        saberPredecirFuerzaX = new M5P();                                            //crea un algoritmo de aprendizaje M5P (árboles de regresión)
        casosEntrenamiento.setClassIndex(0);                                        //la variable a aprender será la fuerza Fz (id=0) dada la distancia
        saberPredecirFuerzaX.buildClassifier(casosEntrenamiento);                    //REALIZA EL APRENDIZAJE DE Fz A PARTIR DE LAS EXPERIENCIAS
/*
        File salida = new File("Assets/Finales_Experiencias.arff");
        if (!salida.exists())
            System.IO.File.Create(salida.getAbsoluteFile().toString()).Dispose();
        ArffSaver saver = new ArffSaver();
        saver.setInstances(casosEntrenamiento);
        saver.setFile(salida);
        saver.writeBatch();

        //EVALUACION DEL CONOCIMIENTO APRENDIDO: 
        
        if (casosEntrenamiento.numInstances() >= 10)
        {
            Evaluation evaluador = new Evaluation(casosEntrenamiento);                   //...Opcional: si tien mas de 10 ejemplo, estima la posible precisión
            evaluador.crossValidateModel(saberPredecirFuerzaX, casosEntrenamiento, 10, new java.util.Random(1));
            print("El Error Absoluto Promedio durante el entrenamiento fue de " + evaluador.meanAbsoluteError().ToString("0.000000") + " N");
        }
*/
        ESTADO = "Con conocimiento";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((ESTADO == "Con conocimiento") && alcanzable)
        {
            calcularObjetivo();

            Instance casoPrueba = new Instance(casosEntrenamiento.numAttributes());  //Crea un registro de experiencia durante el juego
            casoPrueba.setDataset(casosEntrenamiento);
            casoPrueba.setValue(1, distanciaObjetivo);                               //le pone el dato de la distancia a alcanzar

            mejorFuerzaZ = (float)saberPredecirFuerzaX.classifyInstance(casoPrueba);  //predice la fuerza dada la distancia utilizando el algoritmo M5P
            mejorFuerzaX = calcularFuerzaX();

            disparar();
           
            ESTADO = "Acción realizada";

        }
        if (ESTADO == "Acción realizada")
        {
            if (r.transform.position.y <= 7.55f)                                            //cuando la bala cae por debajo de 7.55 m
            {                                                                               //destruye la bala
                r.isKinematic = true;
                Destroy(bulletInstance, 0f);
                ESTADO = "Con conocimiento";
            }
        }
    }

    
    private void calcularObjetivo()
    {
        float offset = calcularOffset();

        if (rbCar.velocity.magnitude >= 5 && rbCar.transform.position.x >= 12.5f)                  //si tiene velocidad
            distanciaObjetivo = car.transform.position.z + offset; 
        else                                                                                        //si esta casi parado
            distanciaObjetivo = car.transform.position.z;
    }

    private float calcularOffset()
    {
        if(car.transform.position.x >=12.5f)    //esta en la recta principal
        {
            if (Mathf.Abs(transform.position.z - car.transform.position.z) >= 30f)
                return 30f;
            else
                return Mathf.Abs(transform.position.z - car.transform.position.z);
        }
        else                                    //esta en la parte del puente
        {
                return Mathf.Abs(transform.position.x - car.transform.position.x);
        }
        
    }
    
    private float calcularFuerzaX()
    {
        float fuerza;

        if (car.transform.position.x >= 12.5f)
            fuerza = (car.transform.position.x - 18) / (-2);
        else 
            fuerza = (car.transform.position.x - 22) / (-2);

        if (rbCar.transform.position.x <= -2.25)
            fuerza += 3f;
        return fuerza;
    }

    private void disparar()
    {
        gameObject.transform.LookAt(car.transform.position);

        bulletInstance = Instantiate(bullet) as GameObject;                      //Crea Bala
        bulletInstance.transform.LookAt(car.transform.position);
        r = bulletInstance.GetComponent<Rigidbody>();
        r.AddRelativeForce(new Vector3(-mejorFuerzaX, Fy_calculada, -mejorFuerzaZ), ForceMode.Impulse);   //Fuerza disparo
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            alcanzable = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            alcanzable = false;
    }
}
