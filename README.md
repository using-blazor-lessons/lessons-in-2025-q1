# "Using Blazor" Lessons in 2025 Q1

Published under MIT No AI Licence:

- [License](LICENSE.md)

## Lesson 01 - Auto Mode and DI

Additionally to existing Blazor Render Modes
- Static server-side
- Interactive server-side
- Interactive WebAssembly client-side

Microsoft introduced with .NET 8.0
- Interactive Auto

In the Auto Render Mode pages are rendered first on server side, later on client side.
This results in various questions to the code structure:
Where to place Components?
How to handle runtime behavior in the application lifecycle, expecially the dependency injection?

Good things first: at compile-time most things are settled.
Open point: dependency injection
Question: in which di-container-instance is an instance?

### 00 - View - Program & Program

![Screenshot 00](lesson_01_auto_mode_and_di/00_view_program_and_program.png)

### 01 - Create - Common Services

![Screenshot 01](lesson_01_auto_mode_and_di/01_create_common_services.png)

### 02 - Code - Static helper stub for DI

![Screenshot 02](lesson_01_auto_mode_and_di/02_code_as_static_and_add_method_stub_for_di.png)

### 03 - Code - Call common services with builder services

![Screenshot 03](lesson_01_auto_mode_and_di/03_code_call_common_services_with_builder_services.png)