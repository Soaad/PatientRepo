import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/auth.service';
import { UserLogin } from 'src/app/models/patient.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

 loginData: UserLogin = { username: '', password: '' };
  error: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  onLogin() {
    this.authService.login(this.loginData).subscribe({
      next: () => {
        this.error = null;
        this.router.navigate(['/patients']);
      },
      error: (err) => {
        this.error = 'Invalid credentials or server error.';
        console.error(err);
      },
    });
  }
}
