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
  products= [];
  endPoint= "https://dummy.restapiexample.com/api/v1/employee/12";

  public result$: Observable<any>;

  constructor(private httpClient: HttpClient) {
        this.result$ = this.httpClient
          //  .get('https://api.github.com/users/hbrotan')
          .get(this.endPoint)
          .pipe(share());
  }
}
