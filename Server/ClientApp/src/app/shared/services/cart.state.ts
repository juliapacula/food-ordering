import { Injectable } from '@angular/core';
import { Dish } from '../../dishes-list';

@Injectable({
    providedIn: 'root',
})
export class CartState {
    private _dishes: Dish[] = [];

    public get size(): number {
        return this._dishes.length;
    }

    public get dishes(): Dish[] {
        return this._dishes;
    }

    public add(dish: Dish): void {
        this._dishes.push({ ...dish });
    }

    public remove(dish: Dish): void {
        this._dishes = this._dishes.filter((d: Dish) => d.id !== dish.id);
    }

    public includes(dish: Dish): boolean {
        return this._dishes.some((d: Dish) => d.id === dish.id);
    }

    public clear(): void {
        this._dishes = [];
    }
}
