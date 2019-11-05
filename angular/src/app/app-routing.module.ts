import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './auth.guard';

import { MainPageComponent } from './mainpage/mainpage.component';
import { BattleComponent } from './battle/battle.component';
import { BaseComponent } from './base/base.component';
import { StatsComponent } from './stats/stats.component';
import { SettingsComponent } from './settings/settings.component';
import { AdminComponent } from './admin/admin.component';
import { LoginComponent } from './login/login.component';
import { AdminUsersComponent } from './admin/admin-users/admin-users.component';
import { AdminBasesComponent } from './admin/admin-bases/admin-bases.component';
import { AdminSettingsComponent } from './admin/admin-settings/admin-settings.component';

@NgModule({
  imports: [RouterModule.forRoot([
    { path: '', component: MainPageComponent, data: { title: 'csharpgame' }, pathMatch: 'full', canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'base', component: BaseComponent },
    { path: 'battles', component: BattleComponent },
    { path: 'stats', component: StatsComponent },
    { path: 'settings', component: SettingsComponent },
    { path: 'admin', component: AdminComponent,
      children: [
        {
          path: '',
          children: [
            { path: 'users', component: AdminUsersComponent, outlet: 'sub', },
            { path: 'bases', component: AdminBasesComponent, outlet: 'sub', },
            { path: 'settings', component: AdminSettingsComponent, outlet: 'sub', canActivate: [AuthGuard] }
          ],
        }
      ]
    },
    { path: '**', redirectTo: '/' }
  ])],
  exports: [RouterModule]
})
export class AppRoutingModule { }