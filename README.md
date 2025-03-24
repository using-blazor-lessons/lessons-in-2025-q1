# "Using Blazor" Lessons in 2025 Q1

Published under MIT No AI Licence:

- [License](LICENSE.md)

## Lesson 02 - State Management with the Flux pattern

It is important to clearify the role of Redux, because there is often a lot of confusion about it.
First, Redux is an implementation of the Flux pattern. It has its roots in the apple/swift ecosystem, so I have been told (not verified).
Redux was very successful in the successful SPA frameworks. Secend, it came with the Redux Dev Tools, which is a independent powerful tool, not an inherent part.

Itself, the Flux pattern defines a technology independent concept to manage states in a standardized control loop. Later the "inner" control loop was extended by the "outer" control loop.

The inner control loop defines
- Components - produce Actions by user interactions or technical triggers.
- Dispatcher - takes occuring Actions and dispatches them to consumers.
- Reducers - the consumers of the inner control loop.
- Stores - Immutable in memory states, used as data sources for Components.

Remarks: Who feels reminded of WPF MVVM is not wrong. The intention of decoupling is the same.

The outer control loop defines
- Effects - consumers (and producers) to standardize remote requests and their reasults into the flux pattern.


![Screenshot 00](lesson_02_statemanagement_with_flux/00_Flux.drawio.png)


Its important to understand the timing behind. The inner control loop is local and always faster than the remote working outer control loop.
For the typical timeline of any use case imagine following scenario:

1. A user defines some filters and sends out a query to a bigger database in the backend. Therfore an action is dispached in the code, containing the filter's data.
2. ALL Reducers which match the action type receive the action and apply it to the related stores. When more than one store should be affected, you need one reducer per store.
3. Related stores are updated and bound components are marked for rerendering. In our case, the data is not fetched till now. So a boolean called "showSpinner" is set to true.
4. Components are rendered and will show a spinner instead of a datatable.
5. When (2) is performed ALL effects which match the action type receive the action as well.
6. Responses to these request will take some time and the results are dispatched again. Trick: Every response is an action, same like 1.
7. Same like 2.
8. Same like 3, but "showSpinner" is set to false.
9. Components are rendered and will show the fetched data.

### 00 - View - Program & Program

![Screenshot 00](lesson_01_auto_mode_and_di/00_view_program_and_program.png)

### 01 - Create - Common Services

![Screenshot 01](lesson_01_auto_mode_and_di/01_create_common_services.png)

### 02 - Code - Static helper stub for DI

![Screenshot 02](lesson_01_auto_mode_and_di/02_code_as_static_and_add_method_stub_for_di.png)

### 03 - Code - Call common services with builder services

![Screenshot 03](lesson_01_auto_mode_and_di/03_code_call_common_services_with_builder_services.png)