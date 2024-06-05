import { HttpClient } from "@angular/common/http";
import { Component, Inject, Input } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ToastrService } from "ngx-toastr";

declare var payhere: any; // Declare the external payhere library

@Component({
  selector: "app-online-payment",
  templateUrl: "./online-payment.component.html",
  styleUrl: "./online-payment.component.css",
})
export class OnlinePaymentComponent {
  // Define the properties to be used within the component
  private baseUrl: string;
  private http: HttpClient;
  private toastr: ToastrService;
  private hash: string;
  private price: number;
  private orderId: string;

  constructor(
    http: HttpClient,
    @Inject("BASE_URL") baseUrl: string,
    private router: Router,
    private modalService: NgbModal,
    toastr: ToastrService,
    private route: ActivatedRoute,
    private authService: SocialAuthService
  ) {
    // Initialize the class properties with constructor parameters
    this.http = http;
    this.baseUrl = baseUrl;
    this.toastr = toastr;
  }

  initiatePayment() {
    // Function to initiate the payment process
    let self = this;

    // HTTP GET request to get payment details from the backend
    this.http
      .get<any>(this.baseUrl + "replace with your Api URL" + this.amount)
      .subscribe(
        function (data) {
          // Handle the response from the backend
          self.hash = data.hash;
          self.price = data.amount;
          self.orderId = data.orderId;

          // Define the PayHere callback functions
          payhere.onCompleted = (orderId: string) => {
            console.log("Payment completed. OrderID: " + orderId);
          };

          payhere.onDismissed = () => {
            console.log("Payment dismissed");
            self.toastr.error("Order failed. Please try again.");
          };

          payhere.onError = (error: any) => {
            console.log("Error: " + error);
            self.toastr.error("Order failed. Please try again.");
          };

          // Construct the payment object using the response data
          const payment = {
            sandbox: false,
            merchant_id: "123456", // Replace with your Merchant ID
            return_url: "", // Important
            cancel_url: "http://sample.com/notify", // Important
            notify_url: "http://sample.com/notify",
            order_id: self.orderId,
            items: "Item name", // Provide the item name
            amount: self.price,
            currency: "USD",
            hash: self.hash, // Use the generated hash retrieved from backend
            first_name: "", // Add customer's first name
            last_name: "", // Add customer's last name
            email: "", // Add customer's email
            phone: "", // Add customer's phone number
            address: "", // Add customer's address
            city: "", // Add customer's city
            country: "", // Add customer's country
            delivery_address: "", // Add delivery address if different
            delivery_city: "", // Add delivery city if different
            delivery_country: "", // Add delivery country if different
            custom_1: "", // Optional custom field
            custom_2: "", // Optional custom field
          };

          // Start the payment process using PayHere
          payhere.startPayment(payment);
        },
        function (error) {
          // Handle errors from the HTTP GET request
          self.toastr.error(
            "Error occurred in getting the record with id: ERROR: " + error
          );
        }
      );
  }

  ngOnInit() {
    // Angular lifecycle hook that is called after the component is initialized
  }
}
