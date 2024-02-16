docker compose --env-file ./config/.env.local -f docker-compose.local.yml -f docker-compose.monitoring.yml up -d

switch (Read-Host 'Clean Up? (Y/n)'){
    Y { 

#stop all containers

docker kill $(docker ps -q)

docker rm $(docker ps -a -q)

docker rmi -f $(docker images -q)

docker rm $(docker ps -a -f status=exited -q)

#cleanup volumes
docker volume prune --force       
}
    N {   
    }
    default {  }
}