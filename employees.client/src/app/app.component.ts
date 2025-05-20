import { Component, OnDestroy } from '@angular/core';
import { finalize, Subject, takeUntil } from 'rxjs';
import { EmployeePair } from './core/models/employeePair.model';
import { EmployeesService } from './core/services/employees.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnDestroy {

  private destroy$ = new Subject<void>();
  public employees: EmployeePair[] = [];
  public selectedFile: File | null = null;
  public error: string = '';
  public message: string = '';

  constructor(private productService: EmployeesService) { }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0 && input.files[0].name.endsWith('.csv')) {
      this.selectedFile = input.files[0];
    }
  }

  onUploadFile(fileInput: HTMLInputElement): void {

    if (!this.selectedFile) {
      return;
    }

    this.error = '';
    this.message = '';
    const formData = new FormData();
    formData.append('file', this.selectedFile!);

    this.productService.uploadFile(formData)
      .pipe(takeUntil(this.destroy$))
      .pipe(finalize(() => {
        fileInput.value = '';
        this.selectedFile = null;
      }))
      .subscribe({
        next: (data) => {
          this.message = `Max pair: ${data[0].employeeId1} & ${data[0].employeeId2} worked together for ${data[0].daysWorked} days on project ${data[0].projectId}`;
          this.employees = data;
        },
        error: (err) => {
          this.error = err.message;
          this.employees = [];
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
