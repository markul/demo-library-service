# UpdateClient

## Purpose
Updates an existing client.

## Endpoint
PUT /api/clients/{id}

## Parameters
id (GUID) + body: firstName, lastName, email.

## Examples
- Input: Examples/UpdateClient/Input.md
- Output: Examples/UpdateClient/Output.md

## Responses
- Success: 204 No Content
- Failure: 404 Not Found

## Algorithm
![Algorithm](./Diagrams/UpdateClient/Algorithm.svg)






