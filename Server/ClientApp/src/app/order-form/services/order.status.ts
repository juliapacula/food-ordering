import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class OrderStatus {
    private _orderId: string | null;

    public get orderId(): string {
        return this._orderId;
    }

    public set orderId(value: string) {
        this._orderId = value;
    }

    public resetOrder(): void {
        this.orderId = null;
    }
}
