# Contributing to ZeepUtils

Thank you for your interest in contributing! To maintain code quality and a consistent development pace, we follow a set of best practices for Git workflows and branching.

## Branching Strategy

We use **Git Flow** (simplified):

- **`master`**: The stable branch. Code here is always production-ready and matches the latest release.
- **`develop`**: The integration branch. All new features and bug fixes are merged here first.
- **Feature Branches (`feature/your-feature`)**: Create these from `develop` for individual tasks.
- **Hotfix Branches (`hotfix/description`)**: Create these from `master` for urgent production fixes.

### Workflow Example

1.  **Fork** the repository (if you are an external contributor).
2.  Create a feature branch from `develop`:
    ```bash
    git checkout develop
    git pull origin develop
    git checkout -b feature/cool-new-utility
    ```
3.  Implement your changes and commit.
4.  Push your branch and open a **Pull Request** to the `develop` branch.
5.  Once approved and passing CI, it will be merged into `develop`.
6.  Periodic releases will merge `develop` into `master`.

## CI/CD Workflows

We use GitHub Actions to automate our development process:

- **CI (Build & Test)**: Triggered on every push and pull request to `master` and `develop`. It ensures the project compiles and follows our coding standards.
- **Publishing**: Triggered when a new tag is pushed (e.g., `v1.2.0`). It automatically packages the library and publishes it to NuGet.

## Coding Standards

- Follow the rules defined in `.editorconfig`.
- Use PascalCase for public members.
- Ensure all public APIs have KDoc (XML documentation) comments.
- Keep methods focused and small.

## Releases

To trigger a release:
1. Update the version in `ZeepUtils.csproj`.
2. Merge `develop` into `master`.
3. Create a tag: `git tag -a v1.2.x -m "Release v1.2.x"`.
4. Push the tag: `git push origin v1.2.x`.
