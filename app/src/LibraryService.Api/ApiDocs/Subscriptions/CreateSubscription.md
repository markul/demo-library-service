# CreateSubscription

## Purpose
Creates a new subscription.

## Endpoint
POST /api/subscriptions

## Parameters
Body: name, subscriptionTypeId, isActive, startDateUtc, clientIds.

## Examples
- Input: Examples/CreateSubscription/Input.md
- Output: Examples/CreateSubscription/Output.md

## Responses
- Success: 201 Created
- Failure: 404 Not Found

## Algorithm
![Algorithm](./Diagrams/CreateSubscription/Algorithm.svg)
