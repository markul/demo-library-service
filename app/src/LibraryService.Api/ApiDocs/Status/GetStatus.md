# GetStatus

## Purpose
Returns the current status of the service based on active subscriptions.

## Endpoint
GET /api/status

## Parameters
No route or query parameters.

## Logic
The `isActive` field is `true` when there are active subscriptions in the system, and `false` when there are no active subscriptions.

## Examples
- Input: Examples/GetStatus/Input.md
- Output: Examples/GetStatus/Output.md

## Responses
- Success: 200 OK
- Failure: 500 Internal Server Error

## Algorithm
![Algorithm](./Diagrams/GetStatus/Algorithm.svg)
