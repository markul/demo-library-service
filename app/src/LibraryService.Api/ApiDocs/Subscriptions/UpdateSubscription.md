# UpdateSubscription

## Purpose
Updates an existing subscription.

## Endpoint
PUT /api/subscriptions/{id}

## Parameters
id (GUID): subscription identifier. Body: name, subscriptionTypeId, isActive, startDateUtc, clientIds.

## Examples
- Input: Examples/UpdateSubscription/Input.md
- Output: Examples/UpdateSubscription/Output.md

## Responses
- Success: 204 No Content
- Failure: 404 Not Found

## Algorithm
![Algorithm](./Diagrams/UpdateSubscription/Algorithm.svg)
