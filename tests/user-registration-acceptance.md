# Acceptance Tests for User Registration Feature

## **Feature:** User Registration

**Scenario:** Successfully register a new user

- **Given** the API is running
- **When** a POST request is sent to `/api/users/register` with valid user data
- **Then** the response should return status `201 Created`
- **And** the response should include the newly created user details
- **And** the user should be assigned the default 'Member' role
- **And** the user should have a membership date set to current date

**Scenario:** Fail to register with duplicate email

- **Given** a user already exists with email "john.doe@example.com"
- **When** a POST request is sent to `/api/users/register` with the same email
- **Then** the response should return status `409 Conflict`
- **And** the response should contain an error message about duplicate email

**Scenario:** Fail to register with invalid email format

- **Given** the API is running
- **When** a POST request is sent to `/api/users/register` with an invalid email format
- **Then** the response should return status `400 Bad Request`
- **And** the response should contain validation errors for email format

**Scenario:** Fail to register with weak password

- **Given** the API is running
- **When** a POST request is sent to `/api/users/register` with a password that doesn't meet strength requirements
- **Then** the response should return status `400 Bad Request`
- **And** the response should contain an error message about password strength

**Scenario:** Fail to register with missing required fields

- **Given** the API is running
- **When** a POST request is sent to `/api/users/register` with missing required fields (name, email, or password)
- **Then** the response should return status `400 Bad Request`
- **And** the response should contain validation errors for the missing fields

---

## **Feature:** Retrieve All Users

**Scenario:** Successfully fetch all users

- **Given** the API is running
- **When** a GET request is sent to `/api/users`
- **Then** the response should return status `200 OK`
- **And** the response should contain a list of users

**Scenario:** No users available

- **Given** the user database is empty
- **When** a GET request is sent to `/api/users`
- **Then** the response should return an empty list

---

## **Feature:** Retrieve a Single User

**Scenario:** Successfully fetch a user by ID

- **Given** a user exists with `id = 1`
- **When** a GET request is sent to `/api/users/1`
- **Then** the response should return status `200 OK`
- **And** the response should contain the correct user details

**Scenario:** User not found

- **Given** no user exists with `id = 999`
- **When** a GET request is sent to `/api/users/999`
- **Then** the response should return status `404 Not Found`

---

## **Feature:** Delete a User

**Scenario:** Successfully delete a user

- **Given** a user exists with `id = 1`
- **When** a DELETE request is sent to `/api/users/1`
- **Then** the response should return status `204 No Content`
- **And** the user should be removed from the database

**Scenario:** Fail to delete a non-existing user

- **Given** no user exists with `id = 999`
- **When** a DELETE request is sent to `/api/users/999`
- **Then** the response should return status `404 Not Found`