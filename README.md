This repo will eventually house a web app. The app will be a port of my existing [React Specials app](https://github.com/cagross/react-specials). The functionality of the app should remain the same, but the tech stack will differ in the following ways:

- Node.js/Express backend to a C#/.NET backend.
- MongoDB datbase to a PostgreSQL database.

The front-end will continue to be a React-based UI.

Stay tuned for more developments.

Update: I have now established both a functioning homepage (with AI component) and a functioning web API route. I also have a simple end-to-end test working.

- Homepage: A functioning homepage can be found at https://localhost:7165/.

- End-To-End Test: One single end-to-end test is working, and passing. The test is built upon xUnit and Selenium.

- AI component: The homepage displays AI-generated content: the most popular dish in a random US state. The final app will have a similar component.

- API: A single API route returning placeholder text has been setup. The route is:

`https://specialscapi20230823213803.azurewebsites.net/api/Products`

A GET request to this should return the text: `Hello World (from Carl)`

- A Swagger page describing my API route should be accessible at: https://specialscapi20230823213803.azurewebsites.net/swagger/index.html
