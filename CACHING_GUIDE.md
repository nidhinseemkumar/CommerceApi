# Detailed Technical Guide: Server-Side Caching

This guide provides a granular breakdown of how server-side caching is implemented in **CommerceApi**, including specific file locations, line numbers, and testing procedures.

---

## 1. Framework Configuration

The caching infrastructure is initialized and configured in the application entry point.

### File: [Program.cs](file:///c:/Users/midhu/OneDrive/Desktop/Nidhin/Claysys/CommerceApi/Program.cs)

*   **Line 56**: `builder.Services.AddMemoryCache();`
    *   Registers the `IMemoryCache` service into the DI container. This enables the use of the in-process memory store.
*   **Lines 60-66**: **Decorator Registration**
    ```csharp
    // Product Service with Cache Decorator
    builder.Services.AddScoped<ProductService>(); // Base implementation
    builder.Services.AddScoped<IProductService, CachedProductService>(); // Decorator

    // Category Service with Cache Decorator
    builder.Services.AddScoped<CategoryService>(); // Base implementation
    builder.Services.AddScoped<ICategoryService, CachedCategoryService>(); // Decorator
    ```
    *   By registering the concrete `ProductService` first and then the `IProductService` interface via `CachedProductService`, the DI container allows the decorator to receive the real database service (`inner`) while controllers receive the cached version.

---

## 2. Product Cache Implementation

### File: [CachedProductService.cs](file:///c:/Users/midhu/OneDrive/Desktop/Nidhin/Claysys/CommerceApi/Services/CachedProductService.cs)

| Feature | Lines | Logic Description |
| :--- | :--- | :--- |
| **Cache Options** | 11-13 | Sets a **5-minute sliding expiration** (resets on access) and a **30-minute absolute expiration** (forced refresh). |
| **Read (All)** | 15-25 | `GetAllAsync()`: Checks for `"Products_All"` key. If null, calls `inner.GetAllAsync()` and populates cache. |
| **Read (Single)** | 27-38 | `GetById(id)`: Checks for `"Product_{id}"`. Uses **Cache-Aside** logic to minimize DB load. |
| **Write/Invalidation** | 40-58 | Intercepts `Create`, `Update`, and `Delete` calls. After a successful DB operation, it triggers cache eviction. |
| **Eviction Logic** | 60-64 | `InvalidateCache(id)`: Explicitly removes the modified product and the global product list from memory to ensure consistency. |

---

## 3. Category Cache Implementation

### File: [CachedCategoryService.cs](file:///c:/Users/midhu/OneDrive/Desktop/Nidhin/Claysys/CommerceApi/Services/CachedCategoryService.cs)

| Feature | Lines | Logic Description |
| :--- | :--- | :--- |
| **Cache Options** | 11-13 | Uses longer durations (**10 min sliding / 1 hour absolute**) because categories change less frequently than inventory. |
| **Complex Invalid.** | 60-66 | **Cross-Entity Eviction**: When a category is modified, it also removes `"Products_All"` (Line 65) because product lists often include category data. |

---

## 4. Postman Verification (Step-by-Step)

Follow these steps to prove the caching logic is working exactly as designed.

### Phase A: Verifying the Hit (Speed)
1.  **Request 1 (Miss)**: Send `GET http://localhost:5252/api/products`
    *   Check Postman **Time** tab: Expect **>100ms**.
    *   Server Log: You will see `Executed DbCommand SELECT ... FROM [Products]`.
2.  **Request 2 (Hit)**: Send the same `GET` again immediately.
    *   Check Postman **Time** tab: Expect **<10ms**.
    *   Server Log: **No new SQL logged.**

### Phase B: Verifying Invalidation (Consistency)
1.  **Preparation**: Note the current name of Product ID 1.
2.  **Trigger (Update)**: Send `PUT http://localhost:5252/api/products/1` with a new name.
    *   This executes `InvalidateCache(1)` on the server.
3.  **Observation**: Send `GET http://localhost:5252/api/products/1`
    *   The time will be slow again (**Miss**).
    *   The response will contain the **NEW** name.
    *   Server Log: Shows a new `SELECT` command to re-populate the cache.

---

## 5. Summary of Key Techniques
1.  **Decorator Pattern**: Allows adding caching without modifying the `ProductService.cs` or `CategoryService.cs` files directly.
2.  **In-Process Caching**: Fast data retrieval directly from the Web API's RAM.
3.  **Specific Key Naming**: Uses strings like `Product_1`, `Product_2` to allow fine-grained invalidation without clearing everything.
4.  **Reference Type Safety**: Uses `System.Text.Json` settings (configured in `Program.cs` lines 73-76) to safely cache entities with circular references.
