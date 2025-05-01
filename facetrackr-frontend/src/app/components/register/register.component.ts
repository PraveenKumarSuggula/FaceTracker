import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  name: string = '';
  email: string  = '';
  passwordHash: string  = '';
  role = 'Student';

  constructor(private authService: AuthService, private router: Router) {}

  register() {
    this.authService.register({name: this.name, email: this.email, passwordHash: this.passwordHash, role:this.role}).subscribe({
      next: (res) => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        alert('Registration failed!');
      }
    });
  }
}
