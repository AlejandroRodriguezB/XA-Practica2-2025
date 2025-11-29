terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

resource "docker_image" "webApi" {
  name = "webapi-${var.name}:latest"
  build {
    context    = var.build_context
    dockerfile = var.dockerfile
  }
}

resource "docker_container" "web" {
  name    = "${var.name}-web"
  image   = docker_image.webApi.image_id
  restart = var.restart

  env = concat([
    "ASPNETCORE_ENVIRONMENT=${var.environment}",
    "ConnectionStrings__PostgresConnection=${var.postgres_connection}",
    "MINIO__ENDPOINT=${var.name}-minio:9000",
    "MINIO__ACCESSKEY=${var.minio_user}",
    "MINIO__SECRETKEY=${var.minio_password}"
    ],
    var.redis_connection != "" ? ["Redis__Connection=${var.redis_connection}"] : []
  )

  healthcheck {
    test     = ["CMD-SHELL", "wget -qO- http://localhost:8080/status || exit 1"]
    interval = "10s"
    timeout  = "3s"
    retries  = 3
  }

  networks_advanced {
    name = var.network_id
  }
}

output "container_name" {
  value = docker_container.web.name
}
