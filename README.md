# Wordy Backend

A minimal API backend for a Wordle-like game built with C# and ASP.NET Minimal API.

## Features
- Create and manage game sessions
- Start a new game with custom word length and max attempts
- Submit guesses and receive feedback
- Persistent word list stored in `Data/words.txt`
- API documentation via Swagger

## Running with Docker
Ensure you have Docker installed. To run the backend, use:
```sh
docker build -t wordle-backend .
docker run -p 8080:8080 wordle-backend
```

## API Endpoints

### Create a Session
**Endpoint:** `POST /create-session`

**Response:**
- `200 OK`: Returns a `UserSessionDTO` containing the session ID.
- `500 Internal Server Error`: If an error occurs.

---
### Get a Session
**Endpoint:** `GET /get-session`

**Response:**
- `200 OK`: Returns a `UserSessionDTO`.
- `404 Not Found`: If the session does not exist.

---
### Create a Game
**Endpoint:** `POST /create-game`

**Request Body:** (form-data or x-www-form-urlencoded)
```json
{
  "word-length": 5,
  "max-attempts": 5
}
```

**Response:**
- `200 OK`: Returns a `GameInstanceDTO`.
- `404 Not Found`: If the session does not exist.

---
### Submit a Guess
**Endpoint:** `POST /guess`

**Request Body:** (form-data or x-www-form-urlencoded)
```json
{
  "guess": "apple"
}
```

**Response:**
- `200 OK`: Returns `GameInstanceDTO` with guess feedback.
- `400 Bad Request`: If the guess is invalid.
- `404 Not Found`: If the game does not exist.

## Data Storage
The game words are stored in `Data/words.txt`. You can modify this file to add more words.

## Swagger API Documentation
Once the server is running, you can access the API documentation at:
```
http://localhost:8080/swagger/index.html
```

