# Tatum Backend API Demo Guide

Base URL:
`http://localhost:5103`

For protected endpoints, add the token returned from login:
`Authorization: Bearer <token>`

---

## 1) Register a customer
Method: `POST`
URL: `http://localhost:5103/api/Auth/register`

Body:
```json
{
  "email": "jane.doe@example.com",
  "password": "Jane@1234",
  "firstName": "Jane",
  "lastName": "Doe",
  "phone": "+2348012345678"
}
```

---

## 2) Verify OTP
Method: `POST`
URL: `http://localhost:5103/api/Auth/verify-registration-otp`

Body:
```json
{
  "email": "jane.doe@example.com",
  "otp": "123456"
}
```

> Replace the OTP with the one sent by the app during registration.

---

## 3) Login
Method: `POST`
URL: `http://localhost:5103/api/Auth/login`

Body:
```json
{
  "email": "ada.okafor@tatumdemo.com",
  "password": "Customer@123"
}
```

Alternative with a newly registered customer:
```json
{
  "email": "jane.doe@example.com",
  "password": "Jane@1234"
}
```

---

## 4) Fetch accounts
Method: `GET`
URL: `http://localhost:5103/api/Account`

Headers:
```http
Authorization: Bearer <token>
```

Optional query string:
```http
http://localhost:5103/api/Account?CustomerId=YOUR_CUSTOMER_ID&pageNumber=1&pageSize=10
```

---

## 5) View billers and products
### Get billers
Method: `GET`
URL: `http://localhost:5103/api/Products/billers?pageNumber=1&pageSize=20`

### Get product items for a specific product
Method: `GET`
URL: `http://localhost:5103/api/Products/YOUR_PRODUCT_ID/items?pageNumber=1&pageSize=20`

Headers:
```http
Authorization: Bearer <token>
```

---

## 6) Purchase a product
Method: `POST`
URL: `http://localhost:5103/api/Transactions/purchase`

Headers:
```http
Content-Type: application/json
Authorization: Bearer <token>
```

Body:
```json
{
  "accountId": "YOUR_ACCOUNT_ID",
  "productId": "YOUR_PRODUCT_ID",
  "productItemId": "YOUR_PRODUCT_ITEM_ID",
  "amount": 1500,
  "fields": {
    "phoneNumber": "08031234567"
  }
}
```

Example without `productItemId` for custom airtime:
```json
{
  "accountId": "YOUR_ACCOUNT_ID",
  "productId": "YOUR_PRODUCT_ID",
  "amount": 2000,
  "fields": {
    "phoneNumber": "08031234567"
  }
}
```

---

## 7) View transaction list
Method: `GET`
URL: `http://localhost:5103/api/Transactions?pageNumber=1&pageSize=10`

Headers:
```http
Authorization: Bearer <token>
```

Optional filters:
```http
http://localhost:5103/api/Transactions?pageNumber=1&pageSize=20&status=Successful&fromDate=2026-09-01&toDate=2026-09-30
```

---

## 8) Admin summary / report
Method: `GET`
URL: `http://localhost:5103/api/Reporting/admin/summary?period=Last7Days`

Headers:
```http
Authorization: Bearer <admin-token>
```

Other valid periods:
```http
http://localhost:5103/api/Reporting/admin/summary?period=Yesterday
http://localhost:5103/api/Reporting/admin/summary?period=Last30Days
http://localhost:5103/api/Reporting/admin/summary?period=Last90Days
```

---

## Seeded demo users
Customer:
```json
{
  "email": "ada.okafor@tatumdemo.com",
  "password": "Customer@123"
}
```

Admin:
```json
{
  "email": "superadmin@tatumconnect.com",
  "password": "Admin@123456"
}
```

---

## Demo flow order
1. Register a customer
2. Verify OTP
3. Login
4. Get accounts
5. Get billers
6. Get products/items
7. Purchase a product
8. Get transactions
9. Admin summary

---

## Project presentation angle
This project is a digital finance and utility payment backend built for a fintech-style MVP. It supports secure onboarding, wallet/account management, bill payment flows (airtime, data, cable TV, electricity), transaction history, and admin reporting. The purpose is to show a backend that covers the most important parts of a digital payments product in a single, testable system.
