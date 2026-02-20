# CreatePayment

## Purpose
Creates a new payment.

## Endpoint
POST /api/payments

## Parameters
Body: uniqueId, amount, subscriptionId, clientId, externalId, status.

## Examples
- Input: Examples/CreatePayment/Input.md
- Output: Examples/CreatePayment/Output.md

## Responses
- Success: 201 Created
- Failure: 400 Bad Request

## Algorithm
![Algorithm](./Diagrams/CreatePayment/Algorithm.svg)
