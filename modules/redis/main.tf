terraform {
  required_providers {
    docker = {
      source = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

resource "docker_container" "redis" {
  name  = "${var.name}-redis"
  image = "redis:8"

  mounts {
    target = "/data"
    source = "${var.name}-redis-volume"
    type   = "volume"
  }

  networks_advanced {
    name = var.network_id
  }
}

output "redis_url" {
  value = "${docker_container.redis.name}:6379"
}