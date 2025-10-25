import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { AsyncPipe, NgIf } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    AsyncPipe,
    NgIf,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatListModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private breakpointObserver = inject(BreakpointObserver);
  isHandset$ = this.breakpointObserver.observe(Breakpoints.Handset);
}
