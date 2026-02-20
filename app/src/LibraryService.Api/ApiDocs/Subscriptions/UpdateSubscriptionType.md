# UpdateSubscriptionType

## Purpose
Updates an existing subscription type.

## Endpoint
PUT /api/subscriptions/types/{id}

## Parameters
id (GUID): subscription type identifier. Body: name, period, price.

## Examples
- Input: Examples/UpdateSubscriptionType/Input.md
- Output: Examples/UpdateSubscriptionType/Output.md

## Responses
- Success: 204 No Content
- Failure: 404 Not Found

## Algorithm
![Algorithm](./Diagrams/UpdateSubscriptionType/Algorithm.svg)
