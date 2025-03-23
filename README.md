# Product Ordering Platform

## Description
ProductOrderingPlatform is a personal project designed using a microservice architecture. Built with **.NET**, **MongoDB**, **MySQL**, **Stripe**, and **RabbitMQ**, this platform functions like an e-commerce application.
**It's composed of three microservices:**
- **Orders Service:**  Handles the creation, processing, and management of orders.
- **Payments Service:** Handles payment transactions and integrates with Stripe for payment mocking.
- **Products Service:** Manages product information and inventory.

Each service operates independently, ensuring scalability, easier maintenance, and clear separation of concerns.

## Technologies

- C#
- .NET Core
- Entity Framework
- MySQL
- MongoDB
- RabbitMQ
- Stripe

## Set up

### Docker build:
Before you begin, ensure that both **Docker** and **Docker Compose** are installed on your system.

- **Navigate** to the project root directory.
- **Build and start the containers** by running:
   ```sh
    docker-compose up --build
    ```
 **Once the containers are running, open your browser to access the services:**
- Orders:`localhost:5001/swagger/index.html` 
- Products: `localhost:5002/swagger/index.html`
- Payments: `localhost:5003/swagger/index.html`

### Native Build

1. ### **Set Up the Databases:**

   **MySQL (for Order and Payment Services):**
    - Create two MySQL databases:
        - `orders_db`
        - `payment_db`
- Update the connection strings in the `appsettings.json` files of the Orders and Products services. For example, for the Order Service:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=orders_db;User=root;Password=yourpassword;"
  }
  ```
  And similarly, update the Payment Service connection string.


   **MongoDB (for Product Service):** 
   - The Product Service is configured to automatically create a MongoDB collection named `Products` if it does not already exist. When the service starts, it checks for the collection and creates it along with the necessary schema validation (ensuring that each document contains the required fields).
   - Simply running the Product Service will set up the collection automatically.
  

- Update the Payments Service’s connection string in its `appsettings.json`:
  ```json
  "MongoDbConnection": {
    "CollectionName": "Products",
    "ConnectionString": "mongodb://mongo-db:27017",
    "DatabaseName": "db_name"
  }
  ```
2. **Apply Database Migrations (for Order and Payment Services):**

    ```sh
    dotnet ef database update 
    ```

3. ### **Ensure Required Services Are Running:**

- **RabbitMQ:**  
  RabbitMQ is required for inter-service communication. You can run RabbitMQ using Docker Desktop or install it directly on your machine.

  **To run RabbitMQ with Docker:**
    ```sh
    docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
    ```
  **Stripe Integration:**  
  No additional setup is needed for Stripe. The project already includes the necessary package for mocking payment transactions.

4. ### **Configuration Details:**

- **OrderService:**
    
   In the `appsettings.json` of the **Order Service**, the `ApiSettings` section defines the base URLs for the Payment and Product services. These URLs are set for **Docker builds**. If you are running the project natively, update the URLs to match your local environment.
    
   **For a Native Build**, adjust the base URLs:

  ```json
     "ApiSettings": {
       "PaymentBaseUrl": "http://localhost:5003",
       "ProductBaseUrl": "http://localhost:5002"
   }
  ```
- **RabbitMq:**

   In the `appsettings.json` of the **Payment Service** and **Order Service**, the `RabbitMqConfig` section is used for inter-service communication. If you're building the project natively, you may need to change the `Host` to `localhost`. The `UserName` and `Password` are set to default (`guest`), but if you change these credentials, you must update the configuration accordingly.

   **For a Native Build**, adjust this section of the `appsettings.json`:

   ```json
     "RabbitMqConfig": {
       "Host": "rabbitmq",
       "QueueName": "paymentQueueStatus",  
       "UserName": "guest",  
       "Password": "guest"  
   }
  ```
     
4. ### **Build the Solutions:**
    ```sh
    dotnet build
    ```
5. ### **Start the Microservices:**

   Start each service in a separate terminal window or tab:
      ```sh
      dotnet run 
      ```
