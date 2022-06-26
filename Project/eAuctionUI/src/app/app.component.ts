import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { share } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'eAuctionUI';
  prodEndPoint='https://7w3cy40af8.execute-api.us-east-1.amazonaws.com/Dev/show-products';
  endPoint= 'https://7w3cy40af8.execute-api.us-east-1.amazonaws.com/Dev/show-bids/';

result:any=null;
products:any;
prodDetail:any;

  constructor(private httpClient: HttpClient) {
      this.products=this.httpClient.get(this.prodEndPoint).pipe(share());
      this.result=null;
  }

  onOptionsSelected(selectedProduct:any){
   
var bidData=this.httpClient
// .get(this.endPoint+selectedProduct+"?pageNumber=1&pageSize=3&sortBy=BuyerFirstName&FilterBy=100")
.get(this.endPoint+selectedProduct)
.pipe(share());
    this.result =bidData; 
}
}
