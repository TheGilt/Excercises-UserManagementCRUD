Main entry point is Program.cs which inlcudes:
- Boilerplate for swagger to make testing accessible and easy
- Middleware for some mock Auth enforcement
- Middleware for logging request info
- Middleware for holistic exception handling
- User class model with annotation to enforce valid structure

Controllers/UserController.cs includes:
- CRUD api for the users data
- Validation which, coupled with the User class' model annotations, enforces both structure and values.

UserData.cs includes:
- Singular concurrent collection acting as a standin for a proper database

Copilot was used to write boilerplate setup and swagger configuration, refine the exception handling middleware, create the auth middleware, and minor tweaks to controller logic and middleware ordering to conform with best practices.
