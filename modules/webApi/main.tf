terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

resource "docker_image" "webApi" {
  name         = "webapi-${var.name}:latest"
  build {
    context    = var.build_context
    dockerfile = var.dockerfile
  }
}

resource "docker_container" "web" {
  name  = "${var.name}-web"
  image = docker_image.webapi.image_id

  env = concat([
    "ASPNETCORE_ENVIRONMENT=${var.environment}",
    "POSTGRES_CONNECTION=${var.postgres_connection}"
  ], 
  var.redis_connection != "" ? ["REDIS_CONNECTION=${var.redis_connection}"] : []
  )

  networks_advanced {
    name = var.network_id
  }

  ports {
    internal = 8080
    external = var.exposed_port
  }
}