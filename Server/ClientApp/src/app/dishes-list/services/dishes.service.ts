import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Dish } from '../models/dish.model';

@Injectable()
export class DishesService {
    constructor(
        private _http: HttpClient,
    ) { }

    public getAll(): Observable<Dish[]> {
        return this._http.get<Dish[]>(`/api/dishes`);
    }
}
