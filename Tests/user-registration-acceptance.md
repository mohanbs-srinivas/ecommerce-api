# User Registration Acceptance Tests

## **Feature:** Member Registration

### **Scenario:** Successfully register a new member

- **Given** a valid member registration payload
- **When** a POST request is sent to `/api/account/register/member`
- **Then** the response should return status `200 OK`
- **And** the response should include success = true
- **And** the response should include a userId
- **And** the response should indicate requiresEmailVerification = true

### **Scenario:** Fail to register with duplicate email

- **Given** an email that already exists in the system
- **When** a POST request is sent to `/api/account/register/member`
- **Then** the response should return status `400 Bad Request`
- **And** the response should include an error message about duplicate email

### **Scenario:** Fail to register with weak password

- **Given** a member registration payload with a weak password
- **When** a POST request is sent to `/api/account/register/member`
- **Then** the response should return status `400 Bad Request`
- **And** the response should include password complexity error messages

---

## **Feature:** Staff Registration

### **Scenario:** Successfully register a new staff member

- **Given** a valid staff registration payload with unique employee ID
- **When** a POST request is sent to `/api/account/register/staff`
- **Then** the response should return status `200 OK`
- **And** the response should include success = true
- **And** the response should indicate requiresApproval = true
- **And** the response should indicate requiresEmailVerification = true

### **Scenario:** Fail to register with duplicate employee ID

- **Given** an employee ID that already exists
- **When** a POST request is sent to `/api/account/register/staff`
- **Then** the response should return status `400 Bad Request`
- **And** the response should include an error about duplicate employee ID

---

## **Feature:** Email Verification

### **Scenario:** Successfully verify email with valid token

- **Given** a valid user ID and verification token
- **When** a POST request is sent to `/api/account/verify-email`
- **Then** the response should return status `200 OK`
- **And** the user's email should be marked as verified

### **Scenario:** Fail to verify with invalid token

- **Given** an invalid or expired verification token
- **When** a POST request is sent to `/api/account/verify-email`
- **Then** the response should return status `400 Bad Request`
- **And** the response should include an error message about invalid token

---

## **Feature:** Input Validation

### **Scenario:** Validate required fields

- **Given** a registration payload with missing required fields
- **When** a POST request is sent to registration endpoints
- **Then** the response should return status `400 Bad Request`
- **And** the response should include specific field validation errors

### **Scenario:** Validate email format

- **Given** a registration payload with invalid email format
- **When** a POST request is sent to registration endpoints
- **Then** the response should return status `400 Bad Request`
- **And** the response should include email format validation error