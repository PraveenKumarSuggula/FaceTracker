// src/app/components/login/login.component.ts
import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email = '';
  passwordHash = '';

  constructor(private auth: AuthService, private router: Router) {}

  login() {
    this.auth.login({ email: this.email, passwordHash: this.passwordHash }).subscribe((res: any) => {
      this.auth.saveToken(res.token);
      const role = this.auth.getRole();
      console.log("role:"+ role)
      if (role === 'Student') this.router.navigate(['/dashboard/student']);
      else if (role === 'Teacher') this.router.navigate(['/dashboard/teacher']);
      else if (role === 'Admin') this.router.navigate(['/dashboard/admin']);
    }, err => {
      alert('Login failed');
    });
  }
}
