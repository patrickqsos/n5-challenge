# N5 Challenge

## Start docker containers

1. Execute:

```bash
docker compose up -d db kafka kafka-ui elasticsearch kibana-init
```
The following docker containers will be created:

- db
    - SQL Server database version 2022 
    - Db Port: 1433
    - Db name: n5 (created with ef migration during n5-api start)
    - User: `sa`
    - Password: `n5ch4ll3ng3!`
- kafka
    - Kafka version 3.9.1
    - Listener for docker traffic: kafka:9092
    - Listener for host traffic: kafka:9091
    - Topic used: `operations`
- kafka-ui
    - UI to explore Kafka topics
    - Port: [8090](http://localhost:8090)

- elasticsearch
    - Elasticsearch version 8.17.0
    - Port: [9200](http://localhost:9200)
    - User: `elastic`
    - Pass: `elastic`
    - Datastream used: `logs-registry-n5`

- kibana-init:
    - Utility to obtain a service token for Kibana. It uses the following script: [elasticsearch-init.sh](elasticsearch-init.sh)

    > We must wait for this container to stop, that means service token was obtained and saved in: [kibana.yml](kibana.yml)

2. Execute:

```bash
docker compose up -d kibana
```
The following docker container will be created:
- kibana: 
    - UI for Elasticsearch
    - Port: [5601](http://localhost:5601)
    - User: `elastic`
    - Password: `elastic`

## Start API Rest

Endpoints available:

- GET /permissions/get
- POST /permissions/request
- PUT /permissions/modify/:id

There are two ways to start:

1. Using a docker container:

```bash
docker compose up -d n5-api
```
The following docker container will be created:

- n5-api

> This way uses environment `Staging` declared at: `N5Challenge/appsettings.Staging.json`

2. Locally:

```bash
 dotnet run --project ./N5Challenge  --launch-profile "development"
 ```

> This way uses environment `Development` declared at: `N5Challenge/appsettings.Development.json`

Both ways use port 8080: http://localhost:8080/swagger/index.html

## Tests

1. Execute:
```bash
dotnet test
```

> More info [here](N5Challenge.Tests/README.md)