terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

resource "docker_volume" "prom_data" {
  name = "${var.name}-prometheus-data"
}

resource "docker_container" "prometheus" {
  name  = "${var.name}-prometheus"
  image = "prom/prometheus:v3.8.0-rc.0"

  mounts {
    source = abspath("${path.module}/prometheus.${var.name}.yml")
    target = "/etc/prometheus/prometheus.yml"
    type   = "bind"
  }

  mounts {
    source = docker_volume.prom_data.name
    target = "/prometheus"
    type   = "volume"
  }

  mounts {
    type   = "bind"
    source = abspath("${path.module}/alerts.yml")
    target = "/etc/prometheus/rules/alerts.yml"
  }

  ports {
    internal = 9090
    external = var.prometheus_port
  }

  networks_advanced {
    name = var.network_id
  }
}
