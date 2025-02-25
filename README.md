# ATIx-ICT-B1.3-2D-Graphics-Secure-Communication-2024-25-P3-LU2

## TO DO:

### Handling Concurrency Issues in API
**Problem:** Concurrent Updates Overwriting Data

### Bulk Operations and Database Transactions
**Question:** What if saving one of the 100 objects fails?  
**Considerations:**
- What should the final state of the system be?
- Which HTTP status code is appropriate?
- Je kunt je voorstellen dat wanneer je 100 objecten wilt opslaan, het inefficiënt zou zijn om 100 afzonderlijke verzoeken naar de API te sturen. In plaats daarvan kun je de API zo ontwerpen dat deze een lijst met objecten accepteert om in één keer op te slaan. Dit vermindert het netwerkverkeer aanzienlijk.

**Possible Outcomes:**
- 99 objects saved
- 0 objects saved

### Adding a User
**Details:**
- Username
- Hashed password
- Foreign key or table to add multiple environments to the user

## Create Database

- Connect to localdb:
  - **Server name:** (localdb)\MSSQLLocalDB
  - **Authentication:** Windows Authentication
- Right-click "databases" and choose: 'New database'
- Use `db.sql` script in this repo to create the database

## Database Usage
- Object-Relational-Mapping framework (ORM)
- Dapper, why? - Lightweight & fast, still uses queries which is useful for skill development

### User Secrets:
- `dotnet user-secrets init`
- `dotnet user-secrets set "SqlConnectionString" "(your connectionstringhere)"`
  - `(Server=(localdb)\\MSSQLLocalDB;Database=;Integrated Security=True;)`

## Create Web API
- `dotnet new webapi --use-controllers -n LU2-WebApi`
- `dotnet new solution`

Access the API:
- [Swagger](https://localhost:7067/swagger)
- [WeatherForecast](https://localhost:7067/WeatherForecast)

---



# API Documentation
#### Authentication & Authorization:
This API uses JWT-based authentication, Some endpoints require specific roles and claims.

## Account Management
### Add a Claim to a User
- **Endpoint:** `POST /account/AccountManagement/{id}/{claim}/{value}`
- **Permissions:**Requires ManageAccount permission
- **Allowed Role:** Admin

## Environment2D

### Get All Environment2D Objects
- **Endpoint:** `GET /Environment2D`
- **Permissions:** Requires Read permission

### Create a New Environment2D Object
- **Endpoint:** `POST /Environment2D`
- **Permissions:** Requires Write permission

### Get Environment2D Object by ID
- **Endpoint:** `GET /Environment2D/{id}`
- **Permissions:** Requires Read permission

### Update an Environment2D Object
- **Endpoint:** `PUT /Environment2D/{id}`
- **Permissions:** Requires Write permission

### Delete an Environment2D Object
- **Endpoint:** `DELETE /Environment2D/{id}`
- **Permissions:** Requires Delete permission
- **Allowed Role:** Admin

## LU2-WebApi (Authentication & User Management)

### Register a New User
- **Endpoint:** `POST /account/register`
- **Permissions:** Public (No authentication required)

### Login
- **Endpoint:** `POST /account/login`
- **Permissions:** Public (Requires valid user credentials)

### Refresh Token
- **Endpoint:** `POST /account/refresh`
- **Permissions:** Requires a valid refresh token

### Confirm Email
- **Endpoint:** `GET /account/confirmEmail`
- **Permissions:** Public (Requires valid email confirmation token)

### Resend Confirmation Email
- **Endpoint:** `POST /account/resendConfirmationEmail`
- **Permissions:** Public (Requires valid user credentials)

### Forgot Password
- **Endpoint:** `POST /account/forgotPassword`
- **Permissions:** Public

### Reset Password
- **Endpoint:** `POST /account/resetPassword`
- **Permissions:** Public (Requires valid reset token)

### Manage Two-Factor Authentication (2FA)
- **Endpoint:** `POST /account/manage/2fa`
- **Permissions:** Requires User role

### Get User Account Info
- **Endpoint:** `GET /account/manage/info`
- **Permissions:** Requires User role

### Update User Account Info
- **Endpoint:** `POST /account/manage/info`
- **Permissions:** Requires User role

### Logout
- **Endpoint:** `POST /account/logout`
- **Permissions:** Requires authentication

## Debugging

### Get User Claims (Debugging)
- **Endpoint:** `GET /debug/claims`
- **Permissions:** Requires Admin role

## API Health Check

### Health Check
- **Endpoint:** `GET /`
- **Permissions:** Public (No authentication required)

## Object2D

### Get All Object2D Items
- **Endpoint:** `GET /Object2D`
- **Permissions:** Requires Read permission

### Create a New Object2D Item
- **Endpoint:** `POST /Object2D`
- **Permissions:** Requires Write permission

### Get Object2D Item by ID
- **Endpoint:** `GET /Object2D/{id}`
- **Permissions:** Requires Read permission

### Update an Object2D Item
- **Endpoint:** `PUT /Object2D/{id}`
- **Permissions:** Requires Write permission

### Delete an Object2D Item
- **Endpoint:** `DELETE /Object2D/{id}`
- **Permissions:** Requires Delete permission
- **Allowed Role:** Admin



# API Endpoints & Responses

## Environment Endpoints

### GET `/api/environments` → Get all environments
- **Status Codes:**
  - `200 OK` → Successful response
  - `500 Internal Server Error` → Server error

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
- **Status Codes:**
  - `200 OK` → Environment found
  - `404 Not Found` → Environment not found

#### **Response**
```json
{
  "id": 1,
  "name": "Forest",
  "maxHeight": 500,
  "maxLength": 800
}
```

#### **Error (404 Not Found)**
```json
{
  "error": "Not Found",
  "message": "Environment with ID 99 not found."
}
```

### POST `/api/environments` → Create a new environment
- **Status Codes:**
  - `201 Created` → Successfully created
  - `400 Bad Request` → Invalid input

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

#### **Error (400 Bad Request)**
```json
{
  "error": "Bad Request",
  "message": "Missing required field: name."
}
```

### PUT `/api/environments/{id}` → Update an environment
- **Status Codes:**
  - `200 OK` → Successfully updated
  - `400 Bad Request` → Invalid input
  - `404 Not Found` → Environment not found

#### **Request Body**
```json
{
  "name": "Updated Forest",
  "maxHeight": 600,
  "maxLength": 900
}
```

#### **Response**
```json
{
  "id": 1,
  "name": "Updated Forest",
  "maxHeight": 600,
  "maxLength": 900
}
```

### DELETE `/api/environments/{id}` → Delete an environment
- **Status Codes:**
  - `204 No Content` → Successfully deleted
  - `404 Not Found` → Environment not found

#### **Response**
```json
{}
```

---

## Object Endpoints

### GET `/api/environments/{envId}/objects` → Get all objects in an environment
- **Status Codes:**
  - `200 OK` → Objects retrieved
  - `404 Not Found` → Environment not found

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

### PUT `/api/environments/{envId}/objects/{id}` → Update an object
### DELETE `/api/environments/{envId}/objects/{id}` → Delete an object

---

## User Endpoints

### POST `/api/users/register` → Register a new user
- **Status Codes:**
  - `201 Created` → User successfully registered
  - `400 Bad Request` → Invalid data

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

#### **Error (400 Bad Request)**
```json
{
  "error": "Bad Request",
  "message": "Password must be at least 8 characters."
}
```

### POST `/api/users/login` → Authenticate user
- **Status Codes:**
  - `200 OK` → Login successful
  - `401 Unauthorized` → Invalid login

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

#### **Error (401 Unauthorized)**
```json
{
  "error": "Unauthorized",
  "message": "Invalid username or password."
}
```












### GET `` → 
- **Rights** ->
- **Conditions** ->
- **Status Codes:**
  - `400` → 

#### **Request Body**
```json

```


# account-info
### GET `https://localhost:7067/account-info/claims` → 
- **Rights** -> Ingelogd op eigen account + CanReadEntity
- **Conditions** -> Je krijgt alleen data die gekoppeld staat aan jou account.
- **Status Codes:**
  - `200` → Success
  - `400` → Bad-request
  - `403` → Forbidden

#### **Request Body**
```json
[
  {
    "id": 0,
    "userId": "string",
    "claimType": "string",
    "claimValue": "string"
  }
]
```
### GET `https://localhost:7067/account-info/environments` → 
- **Rights** -> Ingelogd op eigen account + CanReadEntity
- **Conditions** -> Je krijgt alleen data die gekoppeld staat aan jou account.
- **Status Codes:**
  - `200` → Success
  - `400` → Bad-request
  - `403` → Forbidden

#### **Request Body**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "string",
    "maxLength": 0,
    "maxHeight": 0,
    "createdAt": "2025-02-24T23:17:12.141Z",
    "updatedAt": "2025-02-24T23:17:12.141Z",
    "environmentType": 0
  }
]
```


# environment2d
### GET `https://localhost:7067/environment2d` → 
- **Rights** -> Ingelogd op eigen account + CanReadEntity
- **Conditions** -> Je krijgt alleen data die gekoppeld staat aan jou account.
- **Status Codes:**
  - `200` → Success
  - `400` → Bad-request
  - `403` → Forbidden

#### **Request Body**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "string",
    "maxLength": 0,
    "maxHeight": 0,
    "createdAt": "2025-02-24T23:17:12.141Z",
    "updatedAt": "2025-02-24T23:17:12.141Z",
    "environmentType": 0
  }
]
```

### POST `https://localhost:7067/environment2d` → 
- **Rights** -> Ingelogd op eigen account + CanReadEntity
- **Conditions** -> Naam moet uniek zijn per user, 1/25 karakters, max. 5 per user
- **Status Codes:**
  - `200` → Success
  - `400` → Bad-request
  - `403` → Forbidden

#### **Request Body**
```json
{
  "name": "string",
  "environmentType": 0
}
```

### DELETE `https://localhost:7067/environment2d` → 
- **Rights** -> Ingelogd op eigen account + CanReadEntity
- **Conditions** -> environmentId, gebruiker moet ingelogd zijn om eigen wereld te verwijderen. anders niet.
- **Status Codes:**
  - `200` → Environment deleted successfully.
  - `400` → Bad-request
  - `403` → Forbidden




### POST `https://localhost:7067/environment2d` → 
- **Rights** -> Ingelogd op eigen account + CanReadEntity
- **Conditions** -> Naam moet uniek zijn per user, 1/25 karakters, max. 5 per user
- **Status Codes:**
  - `200` → Success
  - `400` → Bad-request
  - `403` → Forbidden

#### **Request Body**
```json
{
  "name": "string",
  "environmentType": 0
}
```