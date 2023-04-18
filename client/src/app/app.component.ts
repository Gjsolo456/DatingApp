import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  title = 'Dating app';


  constructor(private accountService: AccountService){} //inject account service
  

  ngOnInit(): void {
    this.setCurrentUser(); //which will set our User object if we have something in localStorage
  }


  setCurrentUser(){
    const userString = localStorage.getItem('user');
    if(!userString) return; //if null then this will return from the method and stop the execution of this method.
    const user: User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
 
}
