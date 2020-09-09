import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { CartState } from '../shared/services';
import { OrderService } from './services';
import { OrderForm } from './utils/order.form';

@Component({
    selector: 'app-order-form',
    templateUrl: './order-form.component.html',
    styleUrls: ['./order-form.component.css'],
    providers: [
        OrderService,
    ],
})
export class OrderFormComponent implements OnInit {
    public form: FormGroup;

    constructor(
        private _cartState: CartState,
        private _orderService: OrderService,
    ) { }

    public ngOnInit(): void {
        this._initializeForm();
    }

    public submitOrder(): void {
        this._orderService.add({
            ...this.form.getRawValue(),
            dishes: this._cartState.dishes,
        })
            .subscribe();
    }

    private _initializeForm(): void {
        this.form = OrderForm.createForm();
        this.form.valueChanges.subscribe(() => console.log('this.form: ', this.form));
    }
}
