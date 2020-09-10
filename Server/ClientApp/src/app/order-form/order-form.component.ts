import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { CartState } from '../shared/services';
import { OrderFulfillmentService, OrderService } from './services';
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
        private _orderFulfillmentService: OrderFulfillmentService,
    ) { }

    public ngOnInit(): void {
        this._orderFulfillmentService.connect();
        this._initializeForm();
    }

    public submitOrder(): void {
        this._orderService.add({
            ...this.form.getRawValue(),
            id: '673db885-736c-4531-a2b6-a58d088b46d7',
            dishes: this._cartState.dishes,
        })
            .subscribe();
    }

    private _initializeForm(): void {
        this.form = OrderForm.createForm();
    }
}
