terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

resource "docker_volume" "minio_data" {
  name = "${var.name}-minio-data"
}

resource "docker_container" "minio" {
  name  = "${var.name}-minio"
  image = "minio/minio:latest"

  networks_advanced {
    name = var.network_id
  }

  env = [
    "MINIO_ROOT_USER=${var.admin_user}",
    "MINIO_ROOT_PASSWORD=${var.admin_password}"
  ]

  command = ["server", "/data", "--console-address", ":9001"]

  ports {
    internal = 9001
    external = var.minio_port
  }

  mounts {
    source = docker_volume.minio_data.name
    target = "/data"
    type   = "volume"
  }
}
