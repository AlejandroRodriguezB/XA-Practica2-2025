module "network" {
  source       = "../../modules/network"
  network_name = "net-dev"
}

module "postgres" {
  source     = "../../modules/postgres"
  name       = "dev"
  user       = var.postgres_user
  password   = var.postgres_password
  database   = var.postgres_db
  network_id = module.network.network_id
}

module "webApi" {
  source              = "../../modules/webApi"
  name                = "dev"
  postgres_connection = module.postgres.postgres_url
  redis_connection    = ""
  network_id          = module.network.network_id
  exposed_port        = var.exposed_port
  build_context       = "../../WebApi"
  dockerfile          = "../../WebApi/Dockerfile"
  environment         = "Development"
}
