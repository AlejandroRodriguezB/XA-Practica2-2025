# XA-Practica2-2025
Practica 2 de Redes Avanzadas 2025 - Terraform

## 1- Guia de setup
### Prerrequisitos
Instalar:
- Docker Desktop
- Terraform ≥ 1.5
- .NET SDK 8.0
- Make (recomendado)
### 1.1- Preparar variables de entorno
Generar los siguentes archivos:
```
env/dev/terraform.tfvars
env/pro/terraform.tfvars
```
De la misma carpeta deberías de tener un archivo variables.tf que son las variables a rellenar es las tfvars con los valores que consideres
### 1.2- Desplegar entornos
Para desplegar pro:
```
make init-pro
make plan-pro
make apply-pro 
```
Para desplegar dev:
```
make init-dev
make plan-dev
make apply-dev
```
Y para borrar los entornos
```
make destroy-pro
o
make destroy-dev
```
## 2- Partes del proyecto y diagrama de arquitectura
| Servicio | Descripción |
|---|---|
|WebApi|Servicio web|
|NGINX (balanceador)|Balancea la carga entre las réplicas del servicio anterior (WebApi) siguiendo el algoritmo round robin|
|PostgreSQL|Base de datos|
|Redis|Cache de la base de datos (solo disponible en Pro)|
|MinIO|Gestor de imágenes|
|Prometheus|Métricas|
|Grafana|Dashboards y alertas|

Diagrama:

![Diagrama](Media/ArquitecturaP2.png)

## 3-Tests utilizados y outputs

### Aplicación Web, Base de Datos y Caché:
Al igual que en la práctica anterior, se puede insertar y borrar productos, y en producción se puede tirar la base de datos y sigue habiendo datos (aunque no se pueden modificar)
Con BD y cache:
![Img web cache + BD ](Media/BdCache.png)

Solo cache:
![Img web solo cache](Media/Cache.png)

Adicionalmente, se puede desplegar dev y pro al mismo tiempo:
![Img Docker todo desplegado](Media/All.png)


### Balanceador de Carga:
Cada vez que se carga la web se puede observar que la instancia cambia y concuerda con las instancias en docker:
![Img web instancia 1](Media/Ins1.png)
![Img web instancia 2](Media/Ins2.png)

### Monitorización y Logs:
Se puede acceder a prometheus donde se puede observar que se están consumiendo correctamente los datos:
![Img prometheus status](Media/Prometheus.png)

Y en grafana se obtiene auto el datasource y se generan gráficas automáticamente:
![Img grafana dashboard](Media/Grafana.png)

### Almacenamiento de Archivos:
Desde la web se ha creado una página para insertar imágenes:
![Img web upload img](Media/Minio.png)

Se actualiza en la cabecera:
![Img web header updated](Media/HeaderUpdated.png)

Y se puede ver desde la ui de minio:
![Img minio ui](Media/MinioUi.png)
