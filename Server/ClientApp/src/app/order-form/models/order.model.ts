import { Dish } from '../../dishes-list';

export interface Order {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    street: string;
    streetNumber: string;
    flatNumber: string | null;
    postalCode: string;
    country: string;
    comment: string | null;
    dishes: Dish[];
}
