### Key Features

- **Asynchronous Database Connection**: Initializes a connection to a ClickHouse database using hardcoded connection string parameters, including host, port, database name, user credentials, and connection timeout settings.

- **Retry Policy with Polly**: Utilizes Polly, a .NET resilience and transient-fault-handling library, to define retry policies for database operations. This ensures that transient errors are gracefully handled by retrying failed operations with exponential backoff.

- **ExecuteNonQueryWithParametersAsync**: Executes a non-query SQL command asynchronously with parameters, such as inserting or updating data, using prepared statements to prevent SQL injection.

- **ExecuteQueryAsync**: Asynchronously executes a query and uses a delegate (`Action<IDataReader>`) to process each record in the result set, allowing for flexible data mapping.

- **ExecuteNonQueryAsync**: Executes a non-query SQL command asynchronously without parameters, suitable for SQL commands that don't require input parameters.

- **ExecuteQueryWithParametersAsync**: Similar to `ExecuteNonQueryWithParametersAsync`, but designed for querying data with parameters and processing the results using a delegate.

### Implementation Details

- The class defines a private `_connection` field of type `ClickHouseConnection`, responsible for managing the database connection. The connection is opened in the constructor.

- Each method implements a specific database operation, encapsulating the logic for command creation, parameter binding, and execution within `Task.Run` to ensure asynchronous execution.

- Retry policies are defined using Polly's `WaitAndRetryAsync` method, specifying the number of retry attempts and the backoff strategy (exponential in this case).

- Data mapping for queries is handled through delegates, allowing calling code to define how each row of the result set should be processed.

### Usage

This repository class is intended to be used within a larger .NET application that interacts with ClickHouse databases. It abstracts the complexity of database operations, error handling, and connection management, allowing developers to focus on business logic.

Before using this class, ensure that the `ClickHouse.Ado` and `Polly` libraries are included in your project. Modify the connection string in the constructor as needed to match your database's credentials and settings.

This documentation provides a comprehensive overview of the `ClickHouseAsyncRepository` class and its functionalities, suitable for inclusion in your project's README.md on GitHub.
