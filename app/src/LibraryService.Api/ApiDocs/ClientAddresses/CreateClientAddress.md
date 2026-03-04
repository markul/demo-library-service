# CreateClientAddress

## Purpose
Creates a new address for a specific client.

## Endpoint
POST /api/clients/{clientId}/addresses

## Parameters
Path: clientId. Body: city, country, address, postalCode.

## Examples
- Input: Examples/CreateClientAddress/Input.md
- Output: Examples/CreateClientAddress/Output.md

## Responses
- Success: 201 Created
- Failure: 400 Bad Request

## Algorithm
![Algorithm](./Diagrams/CreateClientAddress/Algorithm.svg)
