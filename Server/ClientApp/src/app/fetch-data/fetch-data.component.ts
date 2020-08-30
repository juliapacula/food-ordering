import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';

@Component({
    selector: 'app-fetch-data',
    templateUrl: './fetch-data.component.html',
})
export class FetchDataComponent implements OnInit {
    public forecasts: WeatherForecast[];

    constructor(
        private _http: HttpClient,
        @Inject('BASE_URL') private _baseUrl: string,
    ) {}

    public ngOnInit(): void {
        this._getForecast();
    }

    private _getForecast(): void {
        this._http.get<WeatherForecast[]>(this._baseUrl + 'weatherforecast')
            .subscribe((result: WeatherForecast[]) => {
                this.forecasts = result;
            });
    }
}

interface WeatherForecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
