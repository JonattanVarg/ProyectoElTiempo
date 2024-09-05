export interface GenericResponse<T> {
    isSuccess: boolean;
    message: string;
    data: T | null;
}

