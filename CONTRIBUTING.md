# Contributing to CommerceApi

Thank you for your interest in contributing! Here are some basic guidelines for the project.

## Code of Conduct

Maintain a professional and inclusive environment.

## How to Contribute

1. **Fork** the repository and create your branch from `main`.
2. **Setup** the project locally following the `README.md`.
3. **Write Code**: Ensure your code follows the project's folder structure and naming conventions.
4. **Style**: Use the provided `.editorconfig` for consistent formatting.
5. **PRs**: Submit a pull request with a clear description of the changes markers.

## Architecture Guidelines

- Keep Controllers thin; move business logic to Services.
- Use DTOs for request/response bodies.
- Follow the Service/Interface pattern for dependency injection.
- Use partial classes for large services (e.g., `AuthService`).
