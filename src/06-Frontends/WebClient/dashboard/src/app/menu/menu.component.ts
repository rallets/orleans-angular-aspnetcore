import { Component, OnInit } from '@angular/core';
import { faDiceD20 } from '@fortawesome/free-solid-svg-icons';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {
  faDiceD20 = faDiceD20;

  isCollapsed = true;

  externalDashboardUrl = environment.externalDashboard;

  constructor() { }

  ngOnInit() {
  }

  toggleMenu() {
    this.isCollapsed = !this.isCollapsed;
  }

}
