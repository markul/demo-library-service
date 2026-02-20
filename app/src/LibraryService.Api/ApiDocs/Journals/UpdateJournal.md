# UpdateJournal

## Purpose
Updates an existing journal.

## Endpoint
PUT /api/journals/{id}

## Parameters
id (GUID) + body: title, issueNumber, publicationYear, publisher.

## Examples
- Input: Examples/UpdateJournal/Input.md
- Output: Examples/UpdateJournal/Output.md

## Responses
- Success: 204 No Content
- Failure: 404 Not Found

## Algorithm
![Algorithm](./Diagrams/UpdateJournal/Algorithm.svg)







