terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "3.6.2"
    }
  }
}

locals {
  rendered_conf = templatefile(
    abspath("${path.module}/nginx.conf.tmpl"),
    {
      webapi_names = var.webapi_names
      web_port     = 8080
    }
  )
}

resource "local_file" "nginx_conf" {
  filename = abspath("${path.module}/generated-nginx.conf")
  content  = local.rendered_conf
}

resource "docker_container" "nginx" {
  name  = "${var.name}-nginx"
  image = "nginx:latest"

  ports {
    internal = 80
    external = var.public_port
  }

  mounts {
    target = "/etc/nginx/nginx.conf"
    type   = "bind"
    source = local_file.nginx_conf.filename
  }

  networks_advanced {
    name = var.network_id
  }
}
