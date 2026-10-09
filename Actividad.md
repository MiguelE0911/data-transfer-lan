# Organización y Arquitectura de Computadoras

## Medición de transferencia de archivos en una red LAN

En esta práctica copiarás cinco archivos de distintos tipos y tamaños desde tu computadora hacia la BOSGAME. Registrarás los tiempos y velocidades, comprobarás las características de tu tarjeta de red y compararás los resultados con los cálculos teóricos. Todas las acciones se realizarán mediante ventanas y menús de Windows y una aplicación de transferencia de archivos.

---

## Objetivos de aprendizaje

- Identificar el modelo de la tarjeta de red y la velocidad negociada de la conexión Ethernet.
- Distinguir bits y bytes, velocidad del enlace y rendimiento efectivo de una copia.
- Calcular el tiempo ideal y la velocidad efectiva de transferencia.
- Explicar las diferencias observadas al compartir la red y los recursos de la BOSGAME.

---

## Escenario y acceso

| Elemento | Dato de la práctica |
|---|---|
| Destino | BOSGAME conectada a la LAN del laboratorio |
| Carpeta compartida única | Velocidad |
| Usuario general | FAST |
| Contraseña | 12345 |
| Subcarpeta del estudiante | FAST-Miguel |
| Ejemplo | FAST-Miguel |
| Ruta de acceso | `\\BOSGAME\Velocidad`<br>`\\192.168.100.10\Velocidad` |

---

## Ficha técnica de la conexión del estudiante

Completa la ficha antes de transferir. La capacidad máxima del modelo y la velocidad negociada son datos distintos. Para los cálculos utiliza la velocidad negociada y el límite conocido del recorrido.

| Característica | Dato registrado |
|---|---|
| Fabricante y modelo exacto de la tarjeta | |
| Tipo de interfaz con el equipo — integrada PCIe o USB | |
| Velocidad máxima declarada | |
| Velocidad negociada de la conexión Ethernet | |
| Versión del controlador de Windows | |
| Categoría y longitud aproximada del cable | |
| Velocidad del enlace de la BOSGAME | 2.5Gbps |
| Switch | 1Gbps |
| Límite nominal del recorrido para el cálculo | |

Obtén el límite nominal como la menor velocidad conocida entre tu enlace, los puertos del switch utilizados y el enlace de la BOSGAME. Ese valor es un techo teórico; no garantiza el rendimiento de copia.

---

## Procedimiento en Windows

### 1 Identificar la tarjeta y el enlace

Haz clic derecho en **Inicio → Administrador de dispositivos → Adaptadores de red**. Identifica el adaptador Ethernet que usarás, abre **Propiedades** y registra su modelo. En la pestaña **Controlador** anota la versión. No selecciones adaptadores virtuales ni la tarjeta Wi-Fi si la copia será por cable.

Para consultar la velocidad negociada, abre **Panel de control → Red e Internet → Centro de redes y recursos compartidos → Cambiar configuración del adaptador**. Haz doble clic en la conexión Ethernet activa y registra el campo **Velocidad**. Toma una captura. Los nombres de los menús pueden variar según la versión de Windows.

Consulta la ficha del fabricante para la capacidad máxima del modelo; no deduzcas esa capacidad solo por la velocidad actual. Si el adaptador es USB, registra también el puerto utilizado.

### 2 Revisar los cinco archivos

En el Explorador, haz clic derecho en cada archivo → **Propiedades**. Registra el nombre, la extensión y el valor exacto de **Tamaño** en bytes. No uses *Tamaño en disco*. Los tamaños siguientes son orientativos; los cálculos usarán los valores reales.

| Tipo de archivo | Tamaño orientativo | Tamaño real en bytes |
|---|---|---|
| Cualquier Archivo (Formato) | 15 MB | |
| Cualquier Archivo (Formato) | 40 MB | |
| Cualquier Archivo (Formato) | 150 MB | |
| Cualquier Archivo (Formato) | 500 MB | |
| Cualquier Archivo (Formato) | 1GB | |

### 3 Acceder a la carpeta compartida

Escribe `\\192.168.100.10\Velocidad` en la barra de direcciones del Explorador. Cuando Windows solicite las credenciales, selecciona **Usar otra cuenta** si es necesario. Introduce el usuario `FAST` o, con el path si es necesario, `bosgame-srv\FAST` y la contraseña `12345`.

Dentro de la carpeta Velocidad, haz clic derecho → **Nuevo → Carpeta** y crea la carpeta **FAST-Nombre del Estudiante**, sustituyendo la última parte por tu nombre. Ejemplo: `FAST-Ana Pérez`. Crea dentro `RONDA_1`, `RONDA_2` y `SIMULTANEA`. Son subcarpetas de una única carpeta compartida.

Utiliza exclusivamente tu subcarpeta. La cuenta FAST es común para el grupo y no separa los permisos de cada estudiante. Si Windows ya usa otra cuenta para acceder a la BOSGAME, avisa al docente para cerrar esa conexión desde la interfaz gráfica.

---

## Creación de Aplicación

Utiliza las opciones de **programación** que pueda tener alguna herramienta de **IA**, para crear una aplicación que sirva para medir la transferencia de archivos de una computadora hacia la bosgame y te pueda dar información adicional sobre la transferecia.

---

## Ejecuta las siguientes acciones

- Abre la Aplicación que creaste, agrega solamente el primer archivo y selecciona como destino `Velocidad\FAST-Nombre del Estudiante\RONDA_1`.
- Selecciona **Copiar** para conservar el original. Registra el tiempo de copia y la velocidad promedio de la operación, con la unidad exacta que muestra el programa. Conserva una captura de *Status* o *Log* que permita identificar el archivo y el resultado.
- Repite con los cuatro archivos restantes, uno por operación. No ejecutes una copia de los cinco juntos para registrar velocidades individuales.
- Repite las cinco operaciones hacia `RONDA_2`. Las carpetas nuevas evitan omitir archivos existentes. Registra cada ronda por separado.
- Cuando el docente lo indique, copia solamente el X a `SIMULTANEA` al mismo tiempo que los compañeros de tu tanda. Registra el número de participantes, el tiempo y la velocidad promedio.
- Si el tiempo aparece como cero o es demasiado corto para medirlo, anota **No medible con esta resolución**. No dividas entre cero: solicita un archivo mayor. La caché, el almacenamiento y las tareas de fondo pueden cambiar los resultados entre rondas.

---

## Registro de las transferencias individuales

| Archivo | Tiempo R1 s | Tiempo R2 s | Simultaneo |
|---|---|---|---|
| Cualquier Archivo | | | |
| Cualquier Archivo | | | |
| Cualquier Archivo | | | |
| Cualquier Archivo | | | |
| Cualquier Archivo | | | |

---

## *Cálculos y comprobación con la tarjeta de red*

Usa unidades decimales: 1 MB = 1.000.000 bytes; 1 GB = 1.000 MB; 1 Mb/s = 1.000.000 bits/s; 1 Gb/s = 1.000 Mb/s. Un byte contiene ocho bits.

Para cada archivo calcula el tiempo ideal y la velocidad efectiva en ambas rondas. Muestra los datos, las conversiones y la sustitución en las fórmulas. Redondea al final a dos decimales.

- **Tiempo ideal en s** = (tamaño en bytes × 8) ÷ límite nominal del recorrido en bits/s
- **Velocidad efectiva en MB/s** = tamaño en bytes ÷ (tiempo de copia en s × 1.000.000)
- **Velocidad efectiva en Mb/s** = velocidad efectiva en MB/s × 8
- **Porcentaje respecto al límite nominal** = (velocidad efectiva en Mb/s ÷ límite nominal en Mb/s) × 100

**Ejemplo:** 150.000.000 bytes, límite de 1 Gb/s y copia de 3 s. Tiempo ideal: 1,20 s; velocidad efectiva: 50 MB/s = 400 Mb/s; porcentaje: 40 %. Es una comparación de rendimiento de copia con el techo nominal, no una medición aislada de la eficiencia de Ethernet.

| Archivo | Tiempo ideal s | MB/s R1 | Mb/s R1 | MB/s R2 | Mb/s R2 |
|---|---|---|---|---|---|
| Archivo Formato | | | | | |
| Archivo Formato | | | | | |
| Archivo Formato | | | | | |
| Archivo Formato | | | | | |
| Archivo Formato | | | | | |

---

## Preguntas de análisis

1. ¿Cuál es la velocidad máxima de tu tarjeta y cuál fue la negociada? ¿Coinciden?
2. ¿Qué elemento establece el límite nominal del recorrido? Justifica con la ficha técnica.
3. ¿Qué archivo tardó más y cuál alcanzó mayor velocidad efectiva?
4. ¿Por qué un archivo pequeño puede mostrar menor velocidad o un tiempo poco preciso?
5. ¿Cómo se comparan tus cálculos con la velocidad promedio de TeraCopy? Revisa sus unidades.
6. ¿Por qué la copia real puede tardar más que el tiempo ideal?
7. ¿Qué cambió al copiar el video simultáneamente? Compara los tiempos y los participantes.
8. ¿Puedes atribuir las diferencias solo al tipo de archivo si también cambia su tamaño? Explica.

---

## Adjunte

- Captura de pantalla de la aplicación que construyó, en donde se aprecie la ejecución del programa (3 o 4 capturas diferentes).