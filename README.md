# Payment Gateway Integration
This project provides an Angular front-end and an ASP.NET Core back-end for handling online payments with PayHere.

## Table of Contents
- ASP.NET Core Back-End
  - Overview
  - Endpoints
  - Code Explanation
- Angular Front-End
  - Overview
  - Component Explanation
  - HTML Template
 
## ASP.NET Core Back-End

### Overview
The back-end API provides endpoints for generating payment hashes, handling payment notifications, and recording payment details. It uses the MD5 hashing algorithm for security and interacts with a database to store payment information.

### Endpoints
- CalculateHash ([HttpGet]): Generates a unique order ID and computes the MD5 hash using merchant details and payment information. Returns the hash, order ID, and amount in the response.
- Notify ([HttpPost("notify")]): Handles payment notifications from PayHere. Validates the MD5 signature and payment status. Updates the payment status in the database if valid.
- PostPayment ([HttpPost("records")]): Records payment details in the database.
### Code Explanation
#### Imports
Necessary namespaces for the code.

#### Controller Declaration
PaymentgatewayController is decorated with [Route] and [ApiController] attributes.

#### Class Properties
- _context: Database context to interact with the database.
- merchantSecret: Secret key for hashing (should be replaced with the actual merchant secret).
#### Constructor
Initializes the database context.

#### CalculateHash Endpoint ([HttpGet])
- Generates a unique order ID.
- Computes the MD5 hash using merchant details and payment information.
- Returns the hash, order ID, and amount in the response.
#### Notify Endpoint ([HttpPost("notify")])
- Handles payment notifications from PayHere.
- Validates the MD5 signature and payment status.
- Updates the payment status in the database if valid.
#### PostPayment Endpoint ([HttpPost("records")])
Records payment details in the database.

#### GenerateOrderId Method
Generates a unique order ID based on the current date and time.

#### ComputeMD5 Method
Computes the MD5 hash of a given string.

#### GenerateMd5Sig Method
Generates the MD5 signature for validation.

#### GetMd5Hash Method
Helper method to compute MD5 hash.

#### PaymentNotification Class
Model class representing the structure of a payment notification.

## Angular Front-End
### Overview
The front-end component allows users to enter payment details and initiate the payment process using PayHere.

### Component Explanation
#### Imports
Necessary Angular modules and services are imported.

#### Component Declaration
The @Component decorator defines the metadata for the component.

#### Class Properties
Declares the properties used within the component.

#### Constructor
Initializes the properties and services used in the component.

#### initiatePayment() Method
- Sends an HTTP GET request to fetch payment details from the back-end.
- Handles the response and assigns values to class properties.
- Sets up callback functions for payment completion, dismissal, and error.
- Constructs a payment object with required details and starts the payment process using PayHere.
#### ngOnInit() Method
- A lifecycle hook that gets called after the component has been initialized. Currently, it does nothing but can be used for initialization tasks.

### HTML Template
#### Form Structure
- The form uses Angular's two-way data binding ([(ngModel)]) to bind input fields to component properties.
- The form submission is handled by the initiatePayment method using (ngSubmit)="initiatePayment()".
#### Form Fields
- The form includes fields for the payment amount, first name, last name, email, phone, address, city, and country.
- Each field is bound to a corresponding property in the component using [(ngModel)].
#### Button
- The "Pay Now" button triggers the form submission and the initiatePayment method.
