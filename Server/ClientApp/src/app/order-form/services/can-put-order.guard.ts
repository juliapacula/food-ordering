import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { CartState } from '../../shared';

@Injectable({
    providedIn: 'root',
})
export class CanPutOrderGuard implements CanActivate {
    constructor(
        private _router: Router,
        private _cartState: CartState,
    ) {}

    public canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot,
    ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
        return this._cartState.size > 0 ? true : this._router.parseUrl('/dishes');
    }

}
