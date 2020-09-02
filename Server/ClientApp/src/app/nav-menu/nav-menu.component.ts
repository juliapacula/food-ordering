import { Component } from '@angular/core';
import { CartState } from '../shared/services/cart.state';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css'],
})
export class NavMenuComponent {
    constructor(
        private _cartState: CartState,
    ) {}

    public get cartSize(): number {
        return this._cartState.size;
    }

    public get isCartEmpty(): boolean {
        return this._cartState.size === 0;
    }
}
