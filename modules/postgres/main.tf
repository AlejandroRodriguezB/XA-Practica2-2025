terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

resource "docker_volume" "dbdata" {
  name = "${var.name}-volume"
}

resource "docker_container" "postgres" {
  name  = "${var.name}-postgres"
  image = "postgres:18"

  env = [
    "POSTGRES_USER=${var.user}",
    "POSTGRES_PASSWORD=${var.password}",
    "POSTGRES_DB=${var.database}"
  ]

  mounts {
    target = "/var/lib/postgresql/data"
    source = docker_volume.dbdata.name
    type   = "volume"
  }

  mounts {
    target = "/docker-entrypoint-initdb.d/init.sql"
    source = abspath("${path.module}/init.sql")
    type   = "bind"
  }

  networks_advanced {
    name = var.network_id
  }
}

output "postgres_url" {
  value = "Host=${docker_container.postgres.name};Port=5432;Database=${var.database};Username=${var.user};Password=${var.password}"
}
