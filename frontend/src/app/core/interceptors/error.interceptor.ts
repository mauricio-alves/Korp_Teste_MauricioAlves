import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocorreu um erro inesperado. Tente novamente.';

      if (error.status === 0) {
        errorMessage = 'Sem conexão com o servidor. O Gateway está ativo?';
      } else if (error.status === 503) {
        errorMessage =
          'Serviço temporariamente indisponível. Circuito aberto devido a falhas excessivas no backend.';
      } else if (error.status === 409) {
        // Erro detalhado da camada de negócio
        errorMessage =
          error.error?.detail ||
          'Conflito de negócio (Saldo insuficiente ou Produto Duplicado).';
      }

      // Painel flutuante mostrando o erro brutalista
      snackBar.open(errorMessage, 'FECHAR', {
        duration: 5000,
        panelClass: ['error-snackbar'],
        horizontalPosition: 'end',
        verticalPosition: 'bottom',
      });

      return throwError(() => error);
    }),
  );
};
