import { FormControl, FormGroup, Validators } from '@angular/forms';

export class OrderForm {
    public static createForm(): FormGroup {
        return new FormGroup({
            firstName: new FormControl(null, [Validators.required, Validators.maxLength(30)]),
            lastName: new FormControl(null, [Validators.required, Validators.maxLength(30)]),
            email: new FormControl(null, [Validators.required]),
            phoneNumber: new FormControl(null, [Validators.required, Validators.pattern('[0-9]{3}-[0-9]{3}-[0-9]{3}')]),
            street: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
            streetNumber: new FormControl(null, [Validators.required, Validators.maxLength(8)]),
            flatNumber: new FormControl(null, []),
            postalCode: new FormControl(null, [Validators.required, Validators.pattern('[0-9]{2}-[0-9]{3}')]),
            city: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            country: new FormControl({ value: 'Polska', disabled: true }, [Validators.required]),
            comment: new FormControl(null, [Validators.maxLength(100)]),
        });
    }
}
