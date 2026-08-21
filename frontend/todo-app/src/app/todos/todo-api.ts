import { Service } from '@angular/core';
import { Observable } from 'rxjs';

import { CreateTodoRequest, Todo, UpdateTodoRequest } from './todo';

@Service()
export class TodoApi {
  getTodos(): Observable<Todo[]> {
    throw new Error('Not implemented');
  }

  getTodo(id: string): Observable<Todo> {
    throw new Error('Not implemented');
  }

  createTodo(request: CreateTodoRequest): Observable<Todo> {
    throw new Error('Not implemented');
  }

  updateTodo(id: string, request: UpdateTodoRequest): Observable<Todo> {
    throw new Error('Not implemented');
  }

  deleteTodo(id: string): Observable<void> {
    throw new Error('Not implemented');
  }
}
