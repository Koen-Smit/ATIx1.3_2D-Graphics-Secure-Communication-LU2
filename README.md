# ATIx-ICT-B1.3-2D-Graphics-Secure-Communication-2024-25-P3-LU2


## Environment Endpoints
GET /api/environments → Get all environments
GET /api/environments/{id} → Get a specific environment by ID
POST /api/environments → Create a new environment
PUT /api/environments/{id} → Update an environment
DELETE /api/environments/{id} → Delete an environment

## Object Endpoints
GET /api/environments/{envId}/objects → Get all objects in a specific environment
GET /api/environments/{envId}/objects/{id} → Get a specific object in an environment
POST /api/environments/{envId}/objects → Add a new object to an environment
PUT /api/environments/{envId}/objects/{id} → Update an object
DELETE /api/environments/{envId}/objects/{id} → Delete an object


## User Endpoints
POST /api/users/register → Register a new user
POST /api/users/login → Authenticate user




# JSON responses: 
## Environment Endpoints
GET /api/environments ->

```JSON
{
  "environments": [
    {
      "id": 1,
      "name": "Forest",
      "maxHeight": 500,
      "maxLength": 800
    },
    {
      "id": 2,
      "name": "Desert",
      "maxHeight": 300,
      "maxLength": 600
    }
  ]
}

```

GET /api/environments/1 →

```JSON
{
  "id": 1,
  "name": "Forest",
  "maxHeight": 500,
  "maxLength": 800
}
```

## Object Endpoints
GET /api/environments/1/objects →
```JSON
{
  "objects": [
    {
      "id": 101,
      "prefabId": "tree-01",
      "positionX": 150,
      "positionY": 200,
      "scaleX": 1.5,
      "scaleY": 1.5,
      "rotationZ": 45
    },
    {
      "id": 102,
      "prefabId": "rock-02",
      "positionX": 300,
      "positionY": 400,
      "scaleX": 1.0,
      "scaleY": 1.0,
      "rotationZ": 0
    }
  ]
}
```

GET /api/environments/1/objects/101 →
```JSON
{
  "id": 101,
  "prefabId": "tree-01",
  "positionX": 150,
  "positionY": 200,
  "scaleX": 1.5,
  "scaleY": 1.5,
  "rotationZ": 45
}
```


## User Endpoints
POST /api/users/register →
```json
{
  "username": "JohnDoe",
  "password": "securepassword123"
}
```

POST /api/users/login →
```json
{
  "username": "JohnDoe",
  "password": "securepassword123"
}

{
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```



# Create webapi
dotnet new webapi --use-controllers -n LU2-WebApi
dotnet new solution