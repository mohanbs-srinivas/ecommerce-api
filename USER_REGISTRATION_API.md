# User Registration API Documentation

This document describes the user registration functionality added to the e-commerce API.

## Overview

The system supports two types of user registration:
- **Member Registration**: For regular customers who can place orders
- **Staff Registration**: For employees with elevated permissions (requires admin approval)

## Authentication & Identity

The system uses ASP.NET Core Identity with the following features:
- Password complexity requirements
- Email verification
- User roles and permissions
- Admin approval workflow for staff

## API Endpoints

### Member Registration

**POST** `/api/account/register/member`

Register a new member (customer) account.

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "1234567890",
  "address": "123 Main St",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful. Please check your email for verification instructions.",
  "errors": [],
  "userId": "guid-here",
  "requiresEmailVerification": true,
  "requiresApproval": false
}
```

### Staff Registration

**POST** `/api/account/register/staff`

Register a new staff member account (requires admin approval).

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "phone": "0987654321",
  "address": "456 Oak St",
  "employeeId": "EMP001",
  "department": "IT",
  "password": "StaffPass123!",
  "confirmPassword": "StaffPass123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful. Your account requires admin approval and email verification.",
  "errors": [],
  "userId": "guid-here",
  "requiresEmailVerification": true,
  "requiresApproval": true
}
```

### Email Verification

**POST** `/api/account/verify-email?userId={userId}&token={token}`

Verify a user's email address using the verification token sent via email.

**Response:**
```json
{
  "success": true,
  "message": "Email verified successfully"
}
```

## Admin Endpoints

### Get Pending Staff Registrations

**GET** `/api/admin/pending-staff-registrations`

Retrieve all staff members awaiting approval.

**Response:**
```json
[
  {
    "id": "guid-here",
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@example.com",
    "employeeId": "EMP001",
    "department": "IT",
    "createdAt": "2023-01-01T00:00:00Z",
    "isEmailVerified": false
  }
]
```

### Approve Staff Registration

**POST** `/api/admin/approve-staff/{userId}?approvedBy={approverName}`

Approve a pending staff registration.

**Response:**
```json
{
  "success": true,
  "message": "Staff registration approved successfully",
  "userId": "guid-here",
  "approvedAt": "2023-01-01T00:00:00Z",
  "approvedBy": "admin"
}
```

### Reject Staff Registration

**POST** `/api/admin/reject-staff/{userId}`

Reject and delete a pending staff registration.

**Request Body:**
```json
{
  "reason": "Incomplete documentation",
  "rejectedBy": "admin"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Staff registration rejected and account deleted",
  "userId": "guid-here",
  "reason": "Incomplete documentation"
}
```

### Get Statistics

**GET** `/api/admin/staff-statistics`

Get user registration statistics.

**Response:**
```json
{
  "totalStaff": 5,
  "approvedStaff": 4,
  "pendingStaff": 1,
  "emailVerifiedStaff": 3,
  "totalMembers": 120,
  "activeMembers": 115
}
```

## Password Requirements

Passwords must meet the following criteria:
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character (@$!%*?&)

## Validation Rules

### Member Registration
- First Name: Required, max 50 characters
- Last Name: Required, max 50 characters
- Email: Required, valid email format, unique
- Phone: Optional, valid phone format
- Address: Optional, max 200 characters
- Password: Required, must meet complexity requirements

### Staff Registration
- All member fields plus:
- Employee ID: Required, max 20 characters, unique
- Department: Required, max 50 characters

## Error Responses

All endpoints return consistent error responses:

```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Detailed error message 1", "Detailed error message 2"],
  "userId": null,
  "requiresEmailVerification": false,
  "requiresApproval": false
}
```

## Security Features

- **Password Hashing**: Passwords are securely hashed using ASP.NET Core Identity
- **Email Verification**: Users must verify their email before full account activation
- **Admin Approval**: Staff accounts require manual approval
- **Input Validation**: Comprehensive server-side validation
- **Rate Limiting**: Built-in protection against spam registrations
- **CSRF Protection**: Cross-site request forgery protection

## Database Schema

The system extends the default ASP.NET Core Identity schema with:

- **ApplicationUser**: Base user class with common fields
- **Member**: Extends ApplicationUser for customer accounts
- **Staff**: Extends ApplicationUser for employee accounts
- **EmailVerificationToken**: Manages email verification tokens

## Integration Notes

- Uses Entity Framework Core with in-memory database for development
- Supports both SQL Server and in-memory databases
- Logging integrated with ASP.NET Core logging framework
- Compatible with existing ecommerce API structure