import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { AppComponent } from './app.component';
import { DishesListComponent } from './dishes-list';
import { HomeComponent } from './home';
import { NavMenuComponent } from './nav-menu';
import { CanPutOrderGuard, OrderFormComponent } from './order-form';
import { ErrorInterceptor } from './shared';

const routes = [
    { path: '', redirectTo: '/dishes', pathMatch: 'full' },
    { path: 'dishes', component: DishesListComponent },
    { path: 'order', component: OrderFormComponent, canActivate: [CanPutOrderGuard] },
];

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        DishesListComponent,
        OrderFormComponent,
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        BrowserAnimationsModule,
        ReactiveFormsModule,
        RouterModule.forRoot(routes),
        ToastrModule.forRoot(),
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
