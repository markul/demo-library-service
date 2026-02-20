# DeletePayment

## Purpose
Deletes payment by identifier.

## Endpoint
DELETE /api/payments/{id}

## Parameters
id (GUID): payment identifier.

## Examples
- Input: Examples/DeletePayment/Input.md
- Output: Examples/DeletePayment/Output.md

## Responses
- Success: 204 No Content
- Failure: 404 Not Found

## Algorithm
![Algorithm](./Diagrams/DeletePayment/Algorithm.svg)
