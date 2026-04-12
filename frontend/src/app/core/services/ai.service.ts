import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AiSuggestionRequest {
  context: string;
  availableProducts: string;
}

export interface AiSuggestionResponse {
  suggestion: string;
}

@Injectable({
  providedIn: 'root',
})
export class AiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/ai';

  suggestProducts(
    request: AiSuggestionRequest,
  ): Observable<AiSuggestionResponse> {
    return this.http.post<AiSuggestionResponse>(
      `${this.apiUrl}/suggest-products`,
      request,
    );
  }
}
