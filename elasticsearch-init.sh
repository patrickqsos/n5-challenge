#!/bin/bash

echo "Waiting for Elasticsearch to start..."
until curl -s http://elasticsearch:9200/_cluster/health > /dev/null 2>&1; do
    echo "Elasticsearch not ready yet, waiting..."
    sleep 10
done

echo "Elasticsearch is ready!"

# Wait a bit more to ensure all services are fully initialized
sleep 30

echo "Creating Kibana service account token..."

# Create the service account token for Kibana
RESPONSE=$(curl -s -X POST \
    -u elastic:elastic \
    -H 'Content-Type: application/json' \
    http://elasticsearch:9200/_security/service/elastic/kibana/credential/token)

# Extract the token from the response
TOKEN=$(echo $RESPONSE | grep -o '"value":"[^"]*"' | cut -d'"' -f4)

if [ -n "$TOKEN" ]; then
    echo "Service account token created successfully"
    echo "Token: $TOKEN"
    
    # Replace 'changeit' in kibana.yml with the token
    sed -i "s/elasticsearch.serviceAccountToken: changeit/elasticsearch.serviceAccountToken: $TOKEN/" kibana.yml
    echo "Token replaced in kibana.yml"
else
    echo "Failed to create service account token"
    echo "Response: $RESPONSE"
    exit 1
fi

echo "Elasticsearch initialization completed successfully" 