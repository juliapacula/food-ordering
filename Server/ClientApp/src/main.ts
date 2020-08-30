import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

export const getBaseUrl: Function = (): string => {
    return document.getElementsByTagName('base')[0].href;
};

const providers = [
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
];

if (environment.production) {
    enableProdMode();
}

platformBrowserDynamic(providers).bootstrapModule(AppModule)
    // tslint:disable-next-line:no-any no-console
    .catch((err: any) => console.log(err));
