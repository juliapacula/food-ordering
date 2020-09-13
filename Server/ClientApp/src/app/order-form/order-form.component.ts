import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Dish } from '../dishes-list/models';
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
export class OrderFormComponent implements OnInit, OnDestroy {
    public form: FormGroup;
    private _wasSaved: boolean = false;

    constructor(
        private _cartState: CartState,
        private _orderService: OrderService,
        private _orderFulfillmentService: OrderFulfillmentService,
        private _router: Router,
    ) { }

    public ngOnInit(): void {
        this._orderFulfillmentService.connect().then(() => this._orderFulfillmentService.initOrder());
        this._initializeForm();
    }

    public ngOnDestroy(): void {
        if (!this._wasSaved) {
            this._orderFulfillmentService.cancelOrder();
        }
    }

    public submitOrder(): void {
        this._orderService.add({
            ...this.form.getRawValue(),
            dishes: this._cartState.dishes.map((d: Dish) => d.id),
        })
            .subscribe(
                () => {
                    this._wasSaved = true;
                    this._cartState.clear();
                    this._router.navigateByUrl('/dishes');
                },
                () => {
                    this._cartState.clear();
                    this._router.navigateByUrl('/dishes');
                },
            );
    }

    private _initializeForm(): void {
        this.form = OrderForm.createForm();
    }
}
