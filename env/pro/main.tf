module "network" {
  source = "../../modules/network"
  network_name = "net-pro"
}

module "postgres" {
  source = "../../modules/postgres"
  name = "pro"
  user = var.postgres_user
  password = var.postgres_password
  database = var.postgres_db
  network_id = module.network.network_id
}

module "redis" {
  source = "../../modules/redis"
  name = "pro"
  network_id = module.network.network_id
}

module "webApi" {
  source = "../../modules/webApi"
  name = "pro"
  postgres_connection = module.postgres.postgres_url
  redis_connection = module.redis.redis_url
  network_id = module.network.network_id
  exposed_port = var.exposed_port
  build_context = "../../WebApi"
  dockerfile = "../../WebApi/Dockerfile"
  environment = "Production"
}
