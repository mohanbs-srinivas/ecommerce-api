# Acceptance Tests for ProductsController

## **Feature:** Retrieve All Products

**Scenario:** Successfully fetch all products

- **Given** the API is running
- **When** a GET request is sent to `/api/products`
- **Then** the response should return status `200 OK`
- **And** the response should contain a list of products

**Scenario:** No products available

- **Given** the product database is empty
- **When** a GET request is sent to `/api/products`
- **Then** the response should return an empty list

---

## **Feature:** Retrieve a Single Product

**Scenario:** Successfully fetch a product by ID

- **Given** a product exists with `id = 1`
- **When** a GET request is sent to `/api/products/1`
- **Then** the response should return status `200 OK`
- **And** the response should contain the correct product details

**Scenario:** Product not found

- **Given** no product exists with `id = 999`
- **When** a GET request is sent to `/api/products/999`
- **Then** the response should return status `404 Not Found`

---

## **Feature:** Create a New Product

**Scenario:** Successfully create a product

- **Given** a valid product payload
- **When** a POST request is sent to `/api/products`
- **Then** the response should return status `201 Created`
- **And** the response should include the newly created product details

**Scenario:** Fail to create a product with missing fields

- **Given** an invalid product payload (e.g., missing name or price)
- **When** a POST request is sent to `/api/products`
- **Then** the response should return status `400 Bad Request`

---

## **Feature:** Update an Existing Product

**Scenario:** Successfully update a product

- **Given** a product exists with `id = 1`
- **When** a PUT request is sent to `/api/products/1` with updated details
- **Then** the response should return status `204 No Content`
- **And** the product should be updated in the database

**Scenario:** Fail to update a non-existing product

- **Given** no product exists with `id = 999`
- **When** a PUT request is sent to `/api/products/999`
- **Then** the response should return status `404 Not Found`

---

## **Feature:** Delete a Product

**Scenario:** Successfully delete a product

- **Given** a product exists with `id = 1`
- **When** a DELETE request is sent to `/api/products/1`
- **Then** the response should return status `204 No Content`
- **And** the product should be removed from the database

**Scenario:** Fail to delete a non-existing product

- **Given** no product exists with `id = 999`
- **When** a DELETE request is sent to `/api/products/999`
- **Then** the response should return status `404 Not Found`
