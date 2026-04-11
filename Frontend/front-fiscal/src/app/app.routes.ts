import { Routes } from '@angular/router';
import { ListaProdutos } from './features/produtos/lista-produtos/lista-produtos';
import { ListaNotas } from './features/notas/lista-notas/lista-notas';
import { CriarNota } from './features/notas/criar-nota/criar-nota';

export const routes: Routes = [
    { path: '', redirectTo: '/notas', pathMatch: 'full' }, 
    { path: 'produtos', component: ListaProdutos },
    { path: 'notas', component: ListaNotas },
    { path: 'notas/nova', component: CriarNota },
    { path: '**', redirectTo: '/notas' } 
];
