import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { 
  LucideAngularModule, 
  LayoutDashboard, 
  Package, 
  FileText, 
  Settings,
  ChevronRight
} from 'lucide-angular';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet, 
    RouterLink, 
    RouterLinkActive,
    LucideAngularModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  readonly title = signal('Korp ERP');
  readonly DashboardIcon = LayoutDashboard;
  readonly ProdutoIcon = Package;
  readonly NotaIcon = FileText;
  readonly SettingsIcon = Settings;
  readonly ArrowIcon = ChevronRight;
}
