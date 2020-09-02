import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CartState } from '../shared/services/cart.state';
import { Dish } from './models';
import { DishesService } from './services';

@Component({
    selector: 'app-dishes-list',
    templateUrl: './dishes-list.component.html',
    styleUrls: ['./dishes-list.component.css'],
    providers: [DishesService],
})
export class DishesListComponent implements OnInit {
    public dishes$: Observable<Dish[]>;

    constructor(
        private _dishesService: DishesService,
        private _cartState: CartState,
    ) {}

    public ngOnInit(): void {
        this._initializeDishes();
    }

    public addDishToCart(dish: Dish): void {
        this._cartState.add(dish);
    }

    public removeDishFromCart(dish: Dish): void {
        this._cartState.remove(dish);
    }

    public isAddedToCart(dish: Dish): boolean {
        return this._cartState.includes(dish);
    }

    private _initializeDishes(): void {
        this.dishes$ = this._dishesService.getAll();
    }
}
