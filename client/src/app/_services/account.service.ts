import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})

export class AccountService {  //AccountService will be responsible for making http requests from our client to our server.
  baseUrl = 'https://localhost:5001/api/'; 
  private currentUserSource = new BehaviorSubject<User | null>(null); 
  currentUser$ = this.currentUserSource.asObservable();


  constructor(private http: HttpClient) { }

  login(model:any){
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response:User) => {
        const user = response;
        if(user){
          localStorage.setItem('user', JSON.stringify(user))
          this.currentUserSource.next(user); //here was going to say what its next value is (user)
        }
      }

      )
    );
  }

  register(model:any){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe( //pipe gives us access to the RXjs operators and here we can do something with the response before the component above subscribes to the response here.
      map(user => {
        if (user){
        localStorage.setItem('user',JSON.stringify(user));
        this.currentUserSource.next(user);
        }
      })
    )
  }

  setCurrentUser(user: User){
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null); //set it next value to null if the user logs out.
  }
}
