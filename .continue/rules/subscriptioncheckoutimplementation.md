---
globs: '["app/src/LibraryService.Application/**", "app/src/LibraryService.Api/**"]'
description: Apply this rule when implementing subscription checkout feature
alwaysApply: true
---

When implementing subscription checkout functionality, ensure all domain logic is in Application layer, keep controllers thin, use MediatR commands/queries, implement proper idempotency with idempotencyKey, handle external service integration (PaymentService), and include comprehensive documentation and tests.