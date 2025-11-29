terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

resource "docker_network" "internal" {
  name = var.network_name
}

output "network_id" {
  value = docker_network.internal.id
}
