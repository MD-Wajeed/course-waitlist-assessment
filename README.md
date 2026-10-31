# Course Waitlist Assessment

Enroll, drop and FIFO waitlist promotion for one course. In-memory .NET 10 service, no UI or database.

## Run
    dotnet build
    dotnet test

Last result: 11 passed, 0 failed, 0 warnings (xUnit v3 adapter, .NET 10.0.14).

Requires the .NET 10 SDK.
Tests cover: capacity available, full, duplicates, FIFO promotion, prerequisite failure, revoked completion (skip and retain), invalid ids, repeated or unknown drop, waitlist drop, no queue-jumping, and one parallel-enroll test.
   

## Layout
- src/CourseWaitlist.Core: Contracts.cs, CourseEnrollmentService.cs
- tests/CourseWaitlist.Tests: FakeLookup.cs, CourseEnrollmentServiceTests.cs
- docs/DESIGN.md: assumptions, invariants, promotion rule
- docs/AI_USAGE_LOG.md: AI prompts and decisions

## Key assumptions
Eligible = completed ALL required courses. Ineligible learners are rejected at join, and skipped but retained at promotion. Re-enroll goes to the back. Full details in docs/DESIGN.md.

## Limitations
- Lookup is called inside the lock (fine in memory, rethink for I/O).
- Ineligible waiters can linger in the queue.
- A seat can stay open after a lookup change until the next drop or Enroll.
- No persistence, no waitlist limit.
- The concurrency test shows no duplicates or overflow, but it cannot prove the absence of races.
- Ids keep the casing of the first request.
- An enrolled learner whose prerequisite is later revoked stays enrolled. Only waiters are re-checked (deliberate scope choice).
- Duplicate required course ids are collapsed ("CS101" and " cs101 " count as one).