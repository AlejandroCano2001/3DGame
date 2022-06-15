//Programación de Videojuegos, Universidad de Málaga (Prof. M. Nuñez, mnunez@uma.es)
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

public class Aprendiz_2_incognitas : MonoBehaviour
{
    weka.classifiers.trees.M5P saberPredecirFuerzaX;
    weka.classifiers.trees.M5P saberPredecirFuerzaZ;
    weka.core.Instances casosEntrenamientoX;
    weka.core.Instances casosEntrenamientoZ;
    //Text texto;
    private string ESTADO = "Sin conocimiento";
    public GameObject bullet;
    GameObject Instanciabullet, PuntoObjetivo;
    float distanciaObjetivoX, distanciaObjetivoZ, mejorFuerzaX, mejorFuerzaZ;
    public float valorMaximoFx = 10, valorMaximoFz = 10, pasoFx, pasoFz;
    float Fy_calculada, valorMaximoFy=18;                                                //Es un ejemplo: Se asume que este valor es extremo para ese problema
    Rigidbody r;
    public Transform shootingSpot;
    public GameObject zombie;

    float valor_calculada_por_metodo_simple(float valorMaximoFy)                        //Calcula una “Fy válida” usando algún método simple.
    {
        float minimoValor = 1f;
       /*Ejemplo:*/ float valorFactible = (minimoValor + valorMaximoFy) / 2f;                 //Por ejemplo, la fuerza media entre el mínimo y el máximo.
        return valorFactible;
    }

    void Start()
    {
        Fy_calculada = valor_calculada_por_metodo_simple(valorMaximoFy);             //Se va a aprender Fx, hay que seleccionar Fy factible
        //texto = Canvas.FindObjectOfType<Text>();
        if (ESTADO == "Sin conocimiento") StartCoroutine("Entrenamiento");          //Lanza el proceso de entrenamiento                                          

    }

    IEnumerator Entrenamiento()
    {
        casosEntrenamientoX = new weka.core.Instances(new java.io.FileReader("Assets/ExperienciasX.arff"));  //Lee fichero con las variables y experiencias
        casosEntrenamientoZ = new weka.core.Instances(new java.io.FileReader("Assets/ExperienciasZ.arff"));  //Lee fichero con las variables y experiencias

        //texto.text = "ENTRENAMIENTO: crea una tabla con las Fx utilizadas y distancias alcanzadas (Fy calculada=" + Fy_calculada.ToString("0.00")+" N)";
        print("Datos de entrada= Fy=" + Fy_calculada + " Fx variables de 1 a " + valorMaximoFx+"  "+((valorMaximoFx==0 || Fy_calculada==0)?" ERROR: alguna fuerza es siempre 0":""));
        if (casosEntrenamientoX.numInstances() < 10)
            for (float Fx = 1; Fx <= valorMaximoFx; Fx = Fx + pasoFx)               //BUCLE de planificación de la fuerza FX durante el entrenamiento
            {
                for (float Fz = 1; Fz <= valorMaximoFz; Fz = Fz + pasoFz)
                {
                    Instanciabullet = Instantiate(bullet, shootingSpot.position, Quaternion.identity);
                    Rigidbody rb = Instanciabullet.GetComponent<Rigidbody>();               //Crea una bullet física
                    rb.AddForce(new Vector3(Fx, Fy_calculada, Fz), ForceMode.Impulse);  //y la lanza con esa fuerza Fx  (Fy se escoge en el Start())
                    yield return new WaitUntil(() => (rb.transform.position.y <= 0.325));        //... y espera a que la bullet llegue al suelo

                    Instance casoAaprenderX = new Instance(casosEntrenamientoX.numAttributes());
                    Instance casoAaprenderZ = new Instance(casosEntrenamientoZ.numAttributes());
                    print("con fuerzas:   Fy_fijo=" + Fy_calculada + "  y  Fx=" + Fx + "  se alcanzó una distancia de " + rb.transform.position.x);
                    casoAaprenderX.setDataset(casosEntrenamientoX);                           //crea un registro de experiencia
                    casoAaprenderZ.setDataset(casosEntrenamientoZ);                           //crea un registro de experiencia
                    casoAaprenderX.setValue(0, Fx);                                          //guarda el dato de la fuerza utilizada
                    casoAaprenderX.setValue(1, Fz);
                    casoAaprenderX.setValue(2, rb.transform.position.x);                     //anota la distancia alcanzada
                    casoAaprenderZ.setValue(0, Fz);                                          //guarda el dato de la fuerza utilizada
                    casoAaprenderZ.setValue(1, Fx);                                          //guarda el dato de la fuerza utilizada
                    casoAaprenderZ.setValue(2, rb.transform.position.z);                     //anota la distancia alcanzada
                    casosEntrenamientoX.add(casoAaprenderX);                                  //guarda el registro de experiencia en X
                    casosEntrenamientoZ.add(casoAaprenderZ);                                  //guarda el registro de experiencia en Z
                    rb.isKinematic = true; rb.GetComponent<Collider>().isTrigger = true;    //...opcional: paraliza la bullet
                    Destroy(Instanciabullet, 1f);                                           //...opcional: destruye la bullet en 1 seg para que ver donde cayó.
                }            
            }                                                                           //FIN bucle de lanzamientos con diferentes de fuerzas
        //APRENDIZADE CONOCIMIENTO EN X:  
        saberPredecirFuerzaX = new M5P();                                            //crea un algoritmo de aprendizaje M5P (árboles de regresión)
        casosEntrenamientoX.setClassIndex(0);                                        //la variable a aprender será la fuerza Fx (id=0) dada la distancia
        saberPredecirFuerzaX.buildClassifier(casosEntrenamientoX);                    //REALIZA EL APRENDIZAJE DE FX A PARTIR DE LAS EXPERIENCIAS

        File salida = new File("Assets/Finales_ExperienciasX.arff");
        if (!salida.exists())
            System.IO.File.Create(salida.getAbsoluteFile().toString()).Dispose();
        ArffSaver saver = new ArffSaver();
        saver.setInstances(casosEntrenamientoX);
        saver.setFile(salida);
        saver.writeBatch();

        //APRENDIZADE CONOCIMIENTO EN Z:  
        saberPredecirFuerzaZ = new M5P();                                            //crea un algoritmo de aprendizaje M5P (árboles de regresión)
        casosEntrenamientoZ.setClassIndex(0);                                        //la variable a aprender será la fuerza Fx (id=0) dada la distancia
        saberPredecirFuerzaZ.buildClassifier(casosEntrenamientoZ);                    //REALIZA EL APRENDIZAJE DE FX A PARTIR DE LAS EXPERIENCIAS

        File salida2 = new File("Assets/Finales_ExperienciasZ.arff");
        if (!salida2.exists())
            System.IO.File.Create(salida.getAbsoluteFile().toString()).Dispose();
        ArffSaver saver2 = new ArffSaver();
        saver2.setInstances(casosEntrenamientoX);
        saver2.setFile(salida2);
        saver2.writeBatch();

        //EVALUACION DEL CONOCIMIENTO APRENDIDO: 
        print("intancias=" + casosEntrenamientoX.numInstances());
        if (casosEntrenamientoX.numInstances() >= 10)
        {
            Evaluation evaluador = new Evaluation(casosEntrenamientoX);                   //...Opcional: si tien mas de 10 ejemplo, estima la posible precisión
            evaluador.crossValidateModel(saberPredecirFuerzaX, casosEntrenamientoX, 10, new java.util.Random(1));
            print("El Error Absoluto Promedio durante el entrenamiento fue de " + evaluador.meanAbsoluteError().ToString("0.000000") + " N");
        }

        distanciaObjetivoX = UnityEngine.Random.Range(1.0f, 15.0f);                          //Distancia de la Canasta (... Opcional: generada aleatoriamente)

        //SITUA UNA CANASTA
        //
        /*PuntoObjetivo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);           // ... opcional: muestra la canasta a la distancia propuesta
        PuntoObjetivo.transform.position = new Vector3(distanciaObjetivoX, -1, 0);
        PuntoObjetivo.transform.localScale = new Vector3(1.1f, 1, 1.1f);
        PuntoObjetivo.GetComponent<Collider>().isTrigger = true;                      //...  opcional: hace que la canasta no sea física 
        */

        PuntoObjetivo = zombie;

        ESTADO = "Con conocimiento";

    }

    void FixedUpdate()                                                                    //Aplica conocimiento aprendido para lanzar a la canasta propuesta
    {
        if (ESTADO == "Con conocimiento")
        {
            Instance casoPrueba = new Instance(casosEntrenamientoX.numAttributes());  //Crea un registro de experiencia durante el juego
            Instance casoPrueba2 = new Instance(casosEntrenamientoZ.numAttributes());
            casoPrueba.setDataset(casosEntrenamientoX);
            casoPrueba2.setDataset(casosEntrenamientoZ);
            casoPrueba.setValue(2, PuntoObjetivo.transform.position.x);                               //le pone el dato de la distancia a alcanzar
            casoPrueba2.setValue(2, PuntoObjetivo.transform.position.z);                               //le pone el dato de la distancia a alcanzar

            mejorFuerzaX = (float)saberPredecirFuerzaX.classifyInstance(casoPrueba);  //predice la fuerza dada la distancia utilizando el algoritmo M5P
            mejorFuerzaZ = (float)saberPredecirFuerzaZ.classifyInstance(casoPrueba2);  //predice la fuerza dada la distancia utilizando el algoritmo M5P
            print("Durante el juego, se observó Y=" + distanciaObjetivoX + ". El NPC calcula la fuerza X =" + mejorFuerzaX);

            Instanciabullet = Instantiate(bullet, shootingSpot.position, Quaternion.identity);                     //Utiliza la bullet física del juego (si no existe la crea)
            r = Instanciabullet.GetComponent<Rigidbody>();
            r.AddForce(new Vector3(mejorFuerzaX, Fy_calculada, mejorFuerzaZ), ForceMode.Impulse);          //y porfin la la lanza en el videojuego con la fuerza encontrara
            print("Se lanzó una bullet con fuerzas:   Fy_fijo = "+Fy_calculada+"  y  Fx =" + mejorFuerzaX );
            ESTADO = "Acción realizada";

        }
        if (ESTADO == "Acción realizada")
        {
            //texto.text = "Para una canasta a " + distanciaObjetivoX.ToString("0.000") + " m, la fuerza Fx a utilizar será de " + mejorFuerzaX.ToString("0.000") + "N  (Fy calculada=" + Fy_calculada.ToString("0.00") + " N)";
            if (r.transform.position.y < 0)                                            //cuando la bullet cae por debajo de 0 m
            {                                                                          //escribe la distancia en x alcanzada
                print("La canasta está a una distancia de " + distanciaObjetivoX + " m");
                print("La bullet lanzada llegó a " + r.transform.position.x + ". El error fue de " + (r.transform.position.x - distanciaObjetivoX).ToString("0.000000") + " m");
                r.isKinematic = true;
                ESTADO = "FIN";
            }
        }
    }
}
