# Commerce API - Data Flow Diagram (DFD) Report

This report documents the data architectural flow of the Commerce API across three levels of detail.

---

## 🌐 Level 0: Context Diagram
The highest level view showing how external actors interact with the system boundary.

```plantuml
@startuml
title Commerce API - Level 0 Context Diagram

actor "Customer" as customer
actor "Admin" as admin
rectangle "Commerce API System" as system

customer --> system : Register / Login
customer --> system : View Products
customer --> system : Place Order / Pay
system --> customer : JWT Token / Order Summary

admin --> system : Manage Products / Categories
system --> admin : CRUD Confirmations
@enduml
```

---

## 📂 Level 1: Process Diagram
Shows the data flow between the major service modules and the central SQL database.

```plantuml
@startuml
title Commerce API - Level 1 DFD

actor "User (Customer/Admin)" as user

rectangle "Authentication Services" as auth {
    process "Login/Register" as p1
}

rectangle "Product Services" as products {
    process "Product/Category Mgmt" as p2
}

rectangle "Order/Payment Services" as business {
    process "Place Order" as p3
    process "Process Payment" as p4
}

database "SQL Database" as db {
    storage "Users Table" as d1
    storage "Products Table" as d2
    storage "Orders Table" as d3
    storage "Payments Table" as d4
}

' Authentication Flow
user -> p1 : Credentials/Details
p1 <-> d1 : Validate/Create User
p1 -> user : JWT Token

' Product Flow
user -> p2 : Browse/Manage
p2 <-> d2 : Query/Update Products

' Order/Payment Flow
user -> p3 : Order DTO
p3 -> d2 : Check Stock
p3 -> d3 : Create Order
p3 -> user : Order Result

user -> p4 : Payment Details
p4 -> d3 : Validate Order
p4 -> d4 : Log Payment
p4 -> user : Payment Receipt

@enduml
```

---

## 🔍 Level 2: Detailed Process Breakdowns
Deep-dive into the internal logic of specific service methods.

### 2A. Order Placement (OrderService.PlaceOrderAsync)
```plantuml
@startuml
title Level 2 DFD - Order Placement

actor "Customer" as user
process "2.1 Validate DTO" as p1
process "2.2 Verify Product" as p2
process "2.3 Update Stock" as p3
process "2.4 Save Order" as p4

database "Products Table" as d1
database "Orders Table" as d2

user -> p1 : Order Items
p1 -> p2 : Validated IDs
p2 <-> d1 : FindByAsync
p2 -> p3 : In-Stock Items
p3 -> d1 : Decrement Stock
p3 -> p4 : Final Object
p4 -> d2 : SaveChangesAsync
p4 -> user : Order Object
@enduml
```

### 2B. Authentication (AuthService)
```plantuml
@startuml
title Level 2 DFD - Authentication

actor "User" as user
process "1.1 Check Conflicts" as p1
process "1.2 BCrypt Hash" as p2
process "1.3 Verify Hash" as p3
process "1.4 JWT Generation" as p4

database "Users Table" as d1

user -> p1 : Register Req
p1 <-> d1 : Check Duplicates
p1 -> p2 : Unique Info
p2 -> d1 : Save User

user -> p3 : Login Req
p3 <-> d1 : Fetch User Record
p3 -> p4 : Valid Hash
p4 -> user : Bearer Token
@enduml
```

### 2C. Payment Processing (PaymentService)
```plantuml
@startuml
title Level 2 DFD - Payment

actor "Customer" as user
process "3.1 Verify Ownership" as p1
process "3.2 Duplicate Check" as p2
process "3.3 Create Payment" as p3

database "Orders Table" as d1
database "Payments Table" as d2

user -> p1 : Order ID
p1 <-> d1 : Validate Owner
p1 -> p2 : Valid Order
p2 <-> d2 : Check Existing
p2 -> p3 : Not Paid
p3 -> d2 : Add & Save
p3 -> user : Receipt
@enduml
```
