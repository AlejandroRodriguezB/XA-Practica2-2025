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

## 3-Tests utilizados


