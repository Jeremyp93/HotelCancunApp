# Hotel Cancun

Hotel Cancun is a minimal web API to reserve the only room available in the last hotel in Cancun.

# REST API

The REST API endpoints are described below.

## Get availability of room

### Request

`GET /availability`

    curl -X 'GET' 'https://localhost:7105/availability?start=13%2F12%2F2021&end=14%2F12%2F2021' -H 'accept: application/json'

### Query Parameters

    Name: start
    Description: Start date of the reservation.
    Type: String
    Format: dd/MM/yyyy

    Name: end
    Description: End date of the reservation.
    Type: String
    Format: dd/MM/yyyy

### Response

    HTTP/1.1 200 OK
    Date: Thu, 24 Feb 2011 12:36:30 GMT
    Status: 200 OK
    Connection: close
    Content-Type: application/json
    Content-Length: 2

    true or false

## Get reservation

### Request

`GET /reservation/reservationNumber`

    curl -X 'GET' 'https://localhost:7105/reservation/3' -H 'accept: application/json'

### Response

    HTTP/1.1 200 OK
    Date: Thu, 24 Feb 2011 12:36:30 GMT
    Status: 200 OK
    Connection: close
    Content-Type: application/json
    Content-Length: 2

    {"name": "foo", "startDate": "2021-12-12T00:00:00", "endDate": "2021-12-13T00:00:00", "id": "e49cc315-fe3a-43d1-bb76-540be903ba0d", "reservationNumber": 3}

## Create a new Reservation

### Request

`POST /reservation/`

    curl -X 'POST' 'https://localhost:7105/reservation' -H 'accept: application/json' -H 'Content-Type: application/json' -d '{"name": "foo""startDate":"2021-12-12T19:50:07.181Z","endDate": "2021-12-13T19:50:07.181Z"}'

### Response

    HTTP/1.1 201 Created
    Date: Thu, 24 Feb 2011 12:36:30 GMT
    Status: 201 Created
    Connection: close
    Content-Type: application/json
    Location: /reservation/3
    Content-Length: 36

    {"name": "foo", "startDate": "2021-12-12T00:00:00", "endDate": "2021-12-13T00:00:00", "id": "e49cc315-fe3a-43d1-bb76-540be903ba0d", "reservationNumber": 3}

## Cancel reservation

### Request

`DELETE /reservation/reservationNumber`

    curl -X 'DELETE' 'https://localhost:7105/reservation/3' -H 'accept: */*'

### Response

    HTTP/1.1 204 No Content
    Date: Thu, 24 Feb 2011 12:36:33 GMT
    Status: 204 No Content
    Connection: close

## Update reservation

### Request

`PUT /reservation/reservationNumber`

    curl -X 'PUT' 'https://localhost:7105/reservation/3' -H 'accept: */*' -H 'Content-Type: application/json' -d '{"name": "foo", "startDate": "2021-12-13T20:16:50.526Z", "endDate": "2021-12-14T20:16:50.526Z",}'

### Response

    HTTP/1.1 204 No Content
    Date: Thu, 24 Feb 2011 12:36:33 GMT
    Status: 204 No Content
    Connection: close
