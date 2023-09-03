This repo will eventually house a web app. The app will be a port of my existing [React Specials app](https://github.com/cagross/react-specials). The functionality of the app should remain the same, but the tech stack will differ in the following ways:

- Node.JS/React backend to a C#/.NET backend.
- MongoDB datbase to a PostgreSQL database.

The front-end will continue to be a React-based UI.

Stay tuned for more developments.

Update: A working web API has been established. A functioning homepage with placeholder text can be found at https://localhost:7165/. Separately, a single API route returning placeholder text has been setup. The route is:

`https://specialscapi20230823213803.azurewebsites.net/api/Products`

A GET request to this should return the text: `Hello World (from Carl)`
