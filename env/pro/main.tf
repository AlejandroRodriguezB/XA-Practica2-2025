module "network" {
  source       = "../../modules/network"
  network_name = "net-pro"
}

module "postgres" {
  source     = "../../modules/postgres"
  name       = "pro"
  user       = var.postgres_user
  password   = var.postgres_password
  database   = var.postgres_db
  network_id = module.network.network_id
}

module "redis" {
  source     = "../../modules/redis"
  name       = "pro"
  network_id = module.network.network_id
}

module "prometheus" {
  source          = "../../modules/prometheus"
  name            = "pro"
  network_id      = module.network.network_id
  prometheus_port = var.exposed_prometheus_port
}

module "grafana" {
  source         = "../../modules/grafana"
  name           = "pro"
  network_id     = module.network.network_id
  grafana_port   = var.grafana_port
  admin_user     = var.grafana_user
  admin_password = var.grafana_password
}

module "minio" {
  source         = "../../modules/minio"
  name           = "pro"
  network_id     = module.network.network_id
  minio_port     = var.minio_port
  admin_user     = var.minio_user
  admin_password = var.minio_password
}

module "webApi" {
  source              = "../../modules/webApi"
  for_each            = toset([for i in range(var.replicas) : "pro${i}"])
  name                = each.key
  postgres_connection = module.postgres.postgres_url
  redis_connection    = module.redis.redis_url
  network_id          = module.network.network_id
  build_context       = "../../WebApi"
  dockerfile          = "../../WebApi/Dockerfile"
  environment         = "Production"
  minio_user          = var.minio_user
  minio_password      = var.minio_password
  restart             = "always"
}

locals {
  webapi_names = [
    for w in module.webApi : w.container_name
  ]
}

module "nginx" {
  source       = "../../modules/nginx"
  name         = "pro"
  network_id   = module.network.network_id
  public_port  = var.exposed_web_port
  webapi_names = [for w in module.webApi : w.container_name]
}
