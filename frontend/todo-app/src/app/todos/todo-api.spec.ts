import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { TodoApi } from './todo-api';
import { Todo } from './todo';

describe('TodoApi', () => {
  let api: TodoApi;
  let httpTesting: HttpTestingController;

  const sampleTodo: Todo = {
    id: '11111111-1111-1111-1111-111111111111',
    title: 'Buy milk',
    createdAt: '2026-08-21T10:00:00Z',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    api = TestBed.inject(TodoApi);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('getTodos issues GET /api/todos and returns the response', () => {
    let result: Todo[] | undefined;

    api.getTodos().subscribe((todos) => {
      result = todos;
    });

    const req = httpTesting.expectOne('/api/todos');
    expect(req.request.method).toBe('GET');
    req.flush([sampleTodo]);

    expect(result).toEqual([sampleTodo]);
  });

  it('getTodo issues GET /api/todos/:id and returns the response', () => {
    let result: Todo | undefined;

    api.getTodo(sampleTodo.id).subscribe((todo) => {
      result = todo;
    });

    const req = httpTesting.expectOne(`/api/todos/${sampleTodo.id}`);
    expect(req.request.method).toBe('GET');
    req.flush(sampleTodo);

    expect(result).toEqual(sampleTodo);
  });

  it('createTodo issues POST /api/todos and returns the created todo', () => {
    let result: Todo | undefined;

    api.createTodo({ title: 'Buy milk' }).subscribe((todo) => {
      result = todo;
    });

    const req = httpTesting.expectOne('/api/todos');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ title: 'Buy milk' });
    req.flush(sampleTodo);

    expect(result).toEqual(sampleTodo);
  });

  it('updateTodo issues PUT /api/todos/:id and returns the updated todo', () => {
    const updated = { ...sampleTodo, title: 'Buy oat milk' };
    let result: Todo | undefined;

    api.updateTodo(sampleTodo.id, { title: 'Buy oat milk' }).subscribe((todo) => {
      result = todo;
    });

    const req = httpTesting.expectOne(`/api/todos/${sampleTodo.id}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ title: 'Buy oat milk' });
    req.flush(updated);

    expect(result).toEqual(updated);
  });

  it('deleteTodo issues DELETE /api/todos/:id', () => {
    let completed = false;

    api.deleteTodo(sampleTodo.id).subscribe(() => {
      completed = true;
    });

    const req = httpTesting.expectOne(`/api/todos/${sampleTodo.id}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);

    expect(completed).toBe(true);
  });
});
