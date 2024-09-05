import { ValidationErrors } from "./validation-errors";

export interface ErrorResponse {
    type: string;
    title: string;
    status: number;
    errors: ValidationErrors;
    traceId: string;
}
