$ErrorActionPreference = "Stop"

$baseUrl = "http://localhost:5251/api"

# 1. Register a test user
$registerBody = @{
    Username = "testuser_multi"
    Password = "Password123"
    Email = "test@example.com"
    PhoneNumber = "1234567890"
    Address = "Testing Location 123"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$baseUrl/auth/register" -Method POST -Body $registerBody -ContentType "application/json"
} catch {}

# 2. Login to get token
$tokenResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method POST -Body $registerBody -ContentType "application/json"
$token = $tokenResponse.token

# 3. Get Products
$products = Invoke-RestMethod -Uri "$baseUrl/products" -Method GET
Write-Host "Available Products:"
$products | Select-Object Id, Name | Format-Table

# 4. Place Order with multiple VALID items
$orderBody = @{
    Items = @(
        @{ ProductId = 1; Quantity = 1 },
        @{ ProductId = 2; Quantity = 1 }
    )
} | ConvertTo-Json -Depth 5

$headers = @{
    Authorization = "Bearer $token"
}

Write-Host "Placing Order with VALID items..."
try {
    $result = Invoke-RestMethod -Uri "$baseUrl/orders" -Method POST -Headers $headers -Body $orderBody -ContentType "application/json"
    Write-Host "Success! Order placed."
    $result | ConvertTo-Json -Depth 5
} catch {
    Write-Host "Error placing valid order: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "Response Body: $($reader.ReadToEnd())"
    }
}

# 5. Place Order with an INVALID item
$invalidOrderBody = @{
    Items = @(
        @{ ProductId = 1; Quantity = 1 },
        @{ ProductId = 99; Quantity = 1 }
    )
} | ConvertTo-Json -Depth 5

Write-Host "`nPlacing Order with INVALID item (ID 99)..."
try {
    $result = Invoke-RestMethod -Uri "$baseUrl/orders" -Method POST -Headers $headers -Body $invalidOrderBody -ContentType "application/json"
    Write-Host "Success! Order placed."
} catch {
    Write-Host "Error placing invalid order: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "Response Status: $($_.Exception.Response.StatusCode)"
        Write-Host "Response Body: $($reader.ReadToEnd())"
    }
}
