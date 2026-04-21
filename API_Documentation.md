# CommerceApi: Comprehensive API Master Specification

This document provides the definitive reference for the CommerceApi, including Request Body structures, Success Responses, and Failure Messages for every endpoint.

---

## 🔐 Global API Configuration
- **Base URL**: `https://localhost:<port>/api`
- **Auth Mode**: `JWT Bearer`
- **Security Header**: `Authorization: Bearer <token>`

---

## 1. Authentication Module (`/auth`)

### [POST] Register User
- **Route**: `/api/auth/register`
- **Request Body**:
```json
{
  "username": "newuser",
  "password": "Password123",
  "email": "user@example.com",
  "phoneNumber": "1234567890",
  "address": "123 Street"
}
```
- **Success (200 OK)**: `"Registered successfully."`
- **Failure (400 Bad Request)**: `"Username already exists."`

### [POST] Login User
- **Route**: `/api/auth/login`
- **Request Body**:
```json
{ "username": "newuser", "password": "Password123" }
```
- **Success (200 OK)**: `{ "token": "eyJhbG..." }`
- **Failure (401 Unauthorized)**: `"Invalid credentials."`

---

## 2. Category Module (`/categories`)

### [GET] All Categories
- **Success (200 OK)**: `[ { "id": 1, "name": "Electronics", "products": [] } ]`
- **Failure (401)**: Missing token.

### [POST] Create Category
- **Request Body**: `{ "name": "New Category", "description": "Details" }`
- **Success (201 Created)**: `{ "id": 5, "name": "New Category", ... }`
- **Failure (400)**: Missing required fields.

### [GET/PUT/DELETE] Single Category
- **Route**: `/api/categories/{id}`
- **Success (200/204)**: Object data or No Content.
- **Failure (404 Not Found)**: ID does not exist.

---

## 3. Product Module (`/products`)

### [GET] All Products
- **Success (200 OK)**: `[ { "id": 1, "name": "Phone", "price": 500, "stock": 10 } ]`

### [POST] Add Product
- **Request Body**:
```json
{
  "name": "Headphones",
  "price": 100,
  "stock": 50,
  "categoryId": 1
}
```
- **Success (201 Created)**: Returns the created Product object.

### [GET/PUT/DELETE] Single Product
- **Route**: `/api/products/{id}`
- **Success**: Product object or No Content.
- **Failure (404 Not Found)**: `"Not Found"` (Status 404).

---

## 4. Orders Module (`/orders`)

### [POST] Place Order (Single or Multiple Items)
- **Route**: `/api/orders`
- **Description**: Handles checkout for any number of items.
- **Request Body**:
```json
{
  "items": [
    { "productId": 1, "quantity": 1 },
    { "productId": 2, "quantity": 2 }
  ]
}
```
- **Success (200 OK)**:
```json
{
  "id": 10,
  "totalAmount": 700.0,
  "items": [
    { "productId": 1, "quantity": 1, "unitPrice": 500.0, "product": { "name": "Phone" } }
  ]
}
```
- **Failure (400 Bad Request)**:
  - `"Not enough stock for 'Phone'. Available: 0"`
  - `"Product with ID 99 does not exist."`

### [GET] My Orders
- **Success (200 OK)**: Array of your previous order objects.

---

## 5. Payments Module (`/payments`)

### [POST] Pay for Order / Retry Payment
- **Route**: `/api/payments`
- **Request Body**: `{ "orderId": 10, "method": "PayPal" }`
- **Success (200 OK)**:
```json
{
  "id": 5,
  "orderId": 10,
  "status": "Completed",
  "paidAt": "2026-04-13T11:45:00"
}
```
- **Failure (400/404)**:
  - `"This order has already been paid."`
  - `"Order not found or doesn't belong to you."`

### [GET] Payment Status
- **Route**: `/api/payments/order/{orderId}`
- **Success (200 OK)**: Payment object details.
- **Failure (404)**: `"No payment found for this order."`

---

## 6. Testing Module

### [GET] General Diagnostics
- **Route**: `/weatherforecast`
- **Success (200 OK)**: Array of 5 forecast objects.
- **Public**: Yes (No token needed).

### Unauthorized Access Test
- **Action**: Any Bearer-protected route without Header.
- **Success (401 Unauthorized)**: Resulting in a locked response.
