terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

resource "docker_volume" "grafana_data" {
  name = "${var.name}-grafana-data"
}

resource "docker_container" "grafana" {
  name  = "${var.name}-grafana"
  image = "grafana/grafana:12.1"

  env = [
    "GF_SECURITY_ADMIN_PASSWORD=${var.admin_password}",
    "GF_SECURITY_ADMIN_USER=${var.admin_user}"
  ]

  mounts {
    source = docker_volume.grafana_data.name
    target = "/var/lib/grafana"
    type   = "volume"
  }

  mounts {
    source = abspath("${path.module}/datasource.${var.name}.yaml")
    target = "/etc/grafana/provisioning/datasources/datasource.yaml"
    type = "bind"
  }

  mounts {
    source = abspath("${path.module}/dashboards.yaml")
    target = "/etc/grafana/provisioning/dashboards/dashboards.yaml"
    type = "bind"
  }

  mounts {
    source = abspath("${path.module}/custom")
    target = "/etc/grafana/provisioning/dashboards/custom"
    type = "bind"
  }

  ports {
    internal = 3000
    external = var.grafana_port
  }

  networks_advanced {
    name = var.network_id
  }
}

output "grafana_url" {
  value = "http://localhost:${var.grafana_port}"
}
