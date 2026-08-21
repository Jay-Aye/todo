import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { TodoApi } from '../todo-api';
import { Todo } from '../todo';

@Component({
  imports: [FormsModule],
  selector: 'app-todo-list',
  styleUrl: './todo-list.css',
  templateUrl: './todo-list.html',
})
export class TodoList implements OnInit {
  private readonly todoApi = inject(TodoApi);

  protected readonly todos = signal<Todo[]>([]);
  protected readonly newTitle = signal('');
  protected readonly editingId = signal<string | null>(null);
  protected readonly editTitle = signal('');
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadTodos();
  }

  protected addTodo(): void {
    const title = this.newTitle().trim();
    if (!title) {
      return;
    }

    this.error.set(null);
    this.todoApi.createTodo({ title }).subscribe({
      next: (todo) => {
        this.todos.update((current) => [...current, todo]);
        this.newTitle.set('');
      },
      error: () => this.error.set('Failed to add todo'),
    });
  }

  protected startEdit(todo: Todo): void {
    this.editingId.set(todo.id);
    this.editTitle.set(todo.title);
    this.error.set(null);
  }

  protected cancelEdit(): void {
    this.editingId.set(null);
    this.editTitle.set('');
  }

  protected saveEdit(id: string): void {
    const title = this.editTitle().trim();
    if (!title) {
      return;
    }

    this.error.set(null);
    this.todoApi.updateTodo(id, { title }).subscribe({
      next: (updated) => {
        this.todos.update((current) =>
          current.map((todo) => (todo.id === id ? updated : todo)),
        );
        this.cancelEdit();
      },
      error: () => this.error.set('Failed to update todo'),
    });
  }

  protected deleteTodo(id: string): void {
    this.error.set(null);
    this.todoApi.deleteTodo(id).subscribe({
      next: () => {
        this.todos.update((current) => current.filter((todo) => todo.id !== id));
        if (this.editingId() === id) {
          this.cancelEdit();
        }
      },
      error: () => this.error.set('Failed to delete todo'),
    });
  }

  private loadTodos(): void {
    this.loading.set(true);
    this.error.set(null);

    this.todoApi.getTodos().subscribe({
      next: (todos) => {
        this.todos.set(todos);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load todos');
        this.loading.set(false);
      },
    });
  }
}
