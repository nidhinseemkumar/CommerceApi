# CommerceApi: Detailed API Reference

This document provides exact sample inputs, success responses, and failure messages for every API module.

---

## 🔐 Global Authentication
- **Base URL**: `https://localhost:<port>/api`
- **Auth Header**: `Authorization: Bearer <JWT_TOKEN>` (Required for all except Auth)

**Common Authentication Failure:**
- **Status**: `401 Unauthorized`
- **Cause**: Missing or expired token.
- **Response**: `(Empty body or standard IIS error)`

---

## 1. Authentication Controller (`/auth`)

### Register User (`POST /register`)
- **Sample Input**:
```json
{
  "username": "tester123",
  "password": "StrongPassword123",
  "email": "tester@example.com",
  "phoneNumber": "123-456-7890",
  "address": "123 Tech Lane"
}
```
- **Success (200 OK)**: `"Registered successfully."`
- **Failure (400 Bad Request)**: `"Username already exists."`

### Login User (`POST /login`)
- **Sample Input**: Same as Register (minimum username/password).
- **Success (200 OK)**: `{ "token": "eyJhbG..." }`
- **Failure (401 Unauthorized)**: `"Invalid credentials."`

---

## 2. Products Controller (`/products`)

### Create/Update Product
- **Success (200/201)**: Returns the project object.
- **Failure (404 Not Found)**: Happens on PUT/DELETE if ID doesn't exist.

---

## 3. Orders Controller (`/orders`)

### Place Order (`POST /`)
- **Sample Input**:
```json
{
  "items": [{ "productId": 1, "quantity": 5 }]
}
```
- **Success (200 OK)**: Returns the completed Order object with Items and calculated Total.
- **Failure (400 Bad Request)**:
  - `"Order items cannot be empty."` (Sent empty list)
  - `"Product with ID 99 does not exist."` (Invalid ID)
  - `"Not enough stock for 'Keyboard'. Available: 2, Requested: 5."` (Inventory issue)

---

## 4. Payments Controller (`/payments`)

### Process Payment (`POST /`)
- **Sample Input**: `{ "orderId": 12, "method": "Stripe" }`
- **Success (200 OK)**: Returns the Payment object with `status: "Completed"`.
- **Failure (404 Not Found)**: `"Order not found or doesn't belong to you."`
- **Failure (400 Bad Request)**: `"This order has already been paid."`

### Get Payment Status (`GET /order/{orderId}`)
- **Failure (404 Not Found)**:
  - `"Order not found."`
  - `"No payment found for this order."`

---

## 🛠️ Debugging Reference (Breakpoints)

| Controller | Method | Line | Key Variable to Watch |
| :--- | :--- | :--- | :--- |
| Auth | Register | 22 | `dto.Username` (Duplicate check) |
| Auth | Login | 39 | `user` (checks if credentials match) |
| Orders | PlaceOrder | 41 | `product.Stock` (Stock verification) |
| Payments | Pay | 24 | `existing` (Prevents double charging) |

---

## 💡 Troubleshooting Tips
1. **Got a 400?** Check the "Response Body" in Postman. It will tell you exactly which business rule (Stock, Duplicates, existence) you broke.
2. **Got a 401?** Check your `Authorization` header. It must start with `Bearer ` (with a space).
3. **Got a 404?** Ensure the `id` you are passing in the URL matches an existing record in your database.
