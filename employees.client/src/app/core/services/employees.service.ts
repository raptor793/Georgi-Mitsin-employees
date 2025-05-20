import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EmployeePair } from '../models/employeePair.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeesService {

  private apiUrl = environment.apiUrl + '/employee';

  constructor(private http: HttpClient) { }

  uploadFile(data: FormData): Observable<EmployeePair[]> {
    return this.http.post<EmployeePair[]>(this.apiUrl, data)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 400 && error.error?.errors) {
      const validationErrors = error.error.errors;
      const messages = Object.values(validationErrors).flat();

      return throwError(() => new Error(messages.join('\n')));
    } else if (error.status === 400 || error.status === 413 && error.error) {
      return throwError(() => new Error(error.error));
    } 

    return throwError(() => new Error('An unexpected error occurred.'));
  }
}
