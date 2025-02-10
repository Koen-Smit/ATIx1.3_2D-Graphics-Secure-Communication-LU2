# ATIx-ICT-B1.3-2D-Graphics-Secure-Communication-2024-25-P3-LU2

# Create webapi
dotnet new webapi --use-controllers -n LU2-WebApi
dotnet new solution

```markdown
# API Endpoints & Responses

## Environment Endpoints
### GET `/api/environments` → Get all environments
- **Statuscodes:**
  - `200 OK` → Succesvolle respons
  - `500 Internal Server Error` → Serverfout

#### **Response**
```json
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

### GET `/api/environments/{id}` → Get a specific environment by ID
- **Statuscodes:**
  - `200 OK` → Omgeving gevonden
  - `404 Not Found` → Omgeving niet gevonden

#### **Response**
```json
{
  "id": 1,
  "name": "Forest",
  "maxHeight": 500,
  "maxLength": 800
}
```

#### **Fout (404 Not Found)**
```json
{
  "error": "Not Found",
  "message": "Environment with ID 99 not found."
}
```

### POST `/api/environments` → Create a new environment
- **Statuscodes:**
  - `201 Created` → Succesvol aangemaakt
  - `400 Bad Request` → Ongeldige invoer

#### **Request Body**
```json
{
  "name": "Ocean",
  "maxHeight": 1000,
  "maxLength": 2000
}
```

#### **Response**
```json
{
  "id": 3,
  "name": "Ocean",
  "maxHeight": 1000,
  "maxLength": 2000
}
```

#### **Fout (400 Bad Request)**
```json
{
  "error": "Bad Request",
  "message": "Missing required field: name."
}
```

---

## Object Endpoints
### GET `/api/environments/{envId}/objects` → Get all objects in an environment
- **Statuscodes:**
  - `200 OK` → Objecten opgehaald
  - `404 Not Found` → Omgeving niet gevonden

#### **Response**
```json
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

---

## User Endpoints
### POST `/api/users/register` → Register a new user
- **Statuscodes:**
  - `201 Created` → Gebruiker succesvol geregistreerd
  - `400 Bad Request` → Ongeldige gegevens

#### **Request Body**
```json
{
  "username": "JohnDoe",
  "password": "securepassword123"
}
```

#### **Response**
```json
{
  "message": "User registered successfully",
  "userId": 10
}
```

#### **Fout (400 Bad Request)**
```json
{
  "error": "Bad Request",
  "message": "Password must be at least 8 characters."
}
```

### POST `/api/users/login` → Authenticate user
- **Statuscodes:**
  - `200 OK` → Inloggen geslaagd
  - `401 Unauthorized` → Foutieve login

#### **Request Body**
```json
{
  "username": "JohnDoe",
  "password": "securepassword123"
}
```

#### **Response**
```json
{
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### **Fout (401 Unauthorized)**
```json
{
  "error": "Unauthorized",
  "message": "Invalid username or password."
}
```
```

