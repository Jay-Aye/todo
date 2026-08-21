import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';

import { TodoApi } from '../todo-api';
import { Todo } from '../todo';
import { TodoList } from './todo-list';

describe('TodoList', () => {
  let fixture: ComponentFixture<TodoList>;
  let todoApi: {
    getTodos: ReturnType<typeof vi.fn>;
    createTodo: ReturnType<typeof vi.fn>;
    updateTodo: ReturnType<typeof vi.fn>;
    deleteTodo: ReturnType<typeof vi.fn>;
  };

  const todos: Todo[] = [
    {
      id: '11111111-1111-1111-1111-111111111111',
      title: 'Buy milk',
      createdAt: '2026-08-21T10:00:00Z',
    },
    {
      id: '22222222-2222-2222-2222-222222222222',
      title: 'Walk the dog',
      createdAt: '2026-08-21T11:00:00Z',
    },
  ];

  beforeEach(async () => {
    todoApi = {
      getTodos: vi.fn(() => of(todos)),
      createTodo: vi.fn((request: { title: string }) =>
        of({
          id: '33333333-3333-3333-3333-333333333333',
          title: request.title,
          createdAt: '2026-08-21T12:00:00Z',
        }),
      ),
      updateTodo: vi.fn((id: string, request: { title: string }) =>
        of({
          id,
          title: request.title,
          createdAt: '2026-08-21T10:00:00Z',
        }),
      ),
      deleteTodo: vi.fn(() => of(undefined)),
    };

    await TestBed.configureTestingModule({
      imports: [TodoList],
      providers: [{ provide: TodoApi, useValue: todoApi }],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoList);
  });

  it('loads and renders todos on init', async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(todoApi.getTodos).toHaveBeenCalledOnce();
    const items = fixture.nativeElement.querySelectorAll('[data-testid="todo-item"]');
    expect(items.length).toBe(2);
    expect(items[0].textContent).toContain('Buy milk');
    expect(items[1].textContent).toContain('Walk the dog');
  });

  it('adds a todo when the form is submitted', async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const input = fixture.nativeElement.querySelector(
      '[data-testid="todo-title-input"]',
    ) as HTMLInputElement;
    const form = fixture.nativeElement.querySelector(
      '[data-testid="todo-form"]',
    ) as HTMLFormElement;

    input.value = 'Read a book';
    input.dispatchEvent(new Event('input'));
    form.dispatchEvent(new Event('submit'));
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(todoApi.createTodo).toHaveBeenCalledWith({ title: 'Read a book' });
    const items = fixture.nativeElement.querySelectorAll('[data-testid="todo-item"]');
    expect(items.length).toBe(3);
    expect(fixture.nativeElement.textContent).toContain('Read a book');
  });

  it('does not add a todo when the title is blank', async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const form = fixture.nativeElement.querySelector(
      '[data-testid="todo-form"]',
    ) as HTMLFormElement;
    form.dispatchEvent(new Event('submit'));
    fixture.detectChanges();
    await fixture.whenStable();

    expect(todoApi.createTodo).not.toHaveBeenCalled();
  });

  it('deletes a todo when delete is clicked', async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const deleteButton = fixture.nativeElement.querySelector(
      '[data-testid="delete-todo"]',
    ) as HTMLButtonElement;
    deleteButton.click();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(todoApi.deleteTodo).toHaveBeenCalledWith(todos[0].id);
    const items = fixture.nativeElement.querySelectorAll('[data-testid="todo-item"]');
    expect(items.length).toBe(1);
    expect(fixture.nativeElement.textContent).not.toContain('Buy milk');
  });

  it('enters edit mode when edit is clicked', async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const editButton = fixture.nativeElement.querySelector(
      '[data-testid="edit-todo"]',
    ) as HTMLButtonElement;
    editButton.click();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const editInput = fixture.nativeElement.querySelector(
      '[data-testid="edit-todo-input"]',
    ) as HTMLInputElement;
    expect(editInput).toBeTruthy();
    expect(editInput.value).toBe('Buy milk');
  });

  it('updates a todo when edit form is saved', async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    fixture.nativeElement.querySelector('[data-testid="edit-todo"]').click();
    fixture.detectChanges();

    const editInput = fixture.nativeElement.querySelector(
      '[data-testid="edit-todo-input"]',
    ) as HTMLInputElement;
    editInput.value = 'Buy oat milk';
    editInput.dispatchEvent(new Event('input'));

    fixture.nativeElement.querySelector('[data-testid="save-todo"]').click();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(todoApi.updateTodo).toHaveBeenCalledWith(todos[0].id, { title: 'Buy oat milk' });
    expect(fixture.nativeElement.textContent).toContain('Buy oat milk');
    expect(fixture.nativeElement.querySelector('[data-testid="edit-todo-input"]')).toBeNull();
  });

  it('does not update when edit title is blank', async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    fixture.nativeElement.querySelector('[data-testid="edit-todo"]').click();
    fixture.detectChanges();

    const editInput = fixture.nativeElement.querySelector(
      '[data-testid="edit-todo-input"]',
    ) as HTMLInputElement;
    editInput.value = '   ';
    editInput.dispatchEvent(new Event('input'));

    fixture.nativeElement.querySelector('[data-testid="save-todo"]').click();
    fixture.detectChanges();
    await fixture.whenStable();

    expect(todoApi.updateTodo).not.toHaveBeenCalled();
  });

  it('cancels edit without calling the API', async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    fixture.nativeElement.querySelector('[data-testid="edit-todo"]').click();
    fixture.detectChanges();

    fixture.nativeElement.querySelector('[data-testid="cancel-edit"]').click();
    fixture.detectChanges();

    expect(todoApi.updateTodo).not.toHaveBeenCalled();
    expect(fixture.nativeElement.querySelector('[data-testid="edit-todo-input"]')).toBeNull();
    expect(fixture.nativeElement.textContent).toContain('Buy milk');
  });

  it('shows an error when loading fails', async () => {
    todoApi.getTodos.mockReturnValue(throwError(() => new Error('network')));

    fixture = TestBed.createComponent(TodoList);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('[data-testid="todo-error"]')?.textContent).toContain(
      'Failed to load todos',
    );
  });
});
