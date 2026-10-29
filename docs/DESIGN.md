Design: Course Waitlist (one offering)
Scope

Enroll, Drop and FIFO waitlist promotion for one course. In-memory .NET 10 service class plus xUnit tests. No UI, CLI, database, catalog, payments, identity integration or policy engine.

Assumptions
Eligibility: a learner is eligible if they have completed ALL required courses. If none are required, everyone is eligible.
Lookup: a small read-only lookup (HasCompleted(learnerId, courseId)) supplies completions. An unknown learner simply has no completions. The lookup never throws (contract), and it always receives normalized ids.
Join time: an ineligible learner is rejected and never queued.
Promotion time: a waiter can only become ineligible if the lookup later revokes a completion. At promotion we re-check, and an ineligible waiter is SKIPPED but RETAINED in place.
Re-enrollment: a learner who re-enrolls after a drop goes to the back of the queue.
Normalization: learner ids and required course ids are trimmed and compared with ordinal case-insensitive comparison. A null or blank learner id is InvalidRequest.
Construction rules: capacity ≤ 0, a null lookup, or a null or blank required course id throws at construction (programmer error).
Service boundary

CourseEnrollmentService owns the enrolled and waitlist state. Its only dependency is the lookup. The API is synchronous, because awaiting inside a lock is a trap.

State and configuration
State: an enrolled list and a waitlist (front = earliest).
Configuration: capacity and the set of required course ids, fixed at construction.
API
Enroll(id): Enrolled | Waitlisted | AlreadyEnrolled | AlreadyWaitlisted | NotEligible | InvalidRequest
Drop(id): Dropped (+ promoted ids) | RemovedFromWaitlist | NotFound | InvalidRequest
GetSnapshot(): immutable copy of enrolled and waitlist, taken under the lock.

Why Dropped carries a list: the promotion pass loops while a seat is open. One drop can therefore promote more than one learner if seats were already open because earlier waiters were ineligible. The list is usually empty or has one id.

Why RemovedFromWaitlist is separate: it distinguishes “a waiter was removed” from “an enrolled learner dropped and nobody was eligible to promote”. An empty promoted list alone can’t tell these apart.

Enroll check order:

Invalid id → InvalidRequest
Already enrolled → AlreadyEnrolled
Already waitlisted → AlreadyWaitlisted
Not eligible → NotEligible
Run the promotion pass
Seat open → Enrolled, otherwise append to the waitlist → Waitlisted

NotEligible never removes an existing waiter, because the waitlist check comes first.

Invariants
Enrolled count ≤ capacity.
A learner appears at most once across enrolled and waitlist.
Waitlist order is FIFO.
Rejected or no-op requests change nothing.
No seat is open while an eligible waiter exists, as of the last promotion pass, per the lookup at that moment. The pass runs after every enrolled-learner drop and before every Enroll that reaches step 5.
Promotion rule

The promotion pass runs after an enrolled learner drops and before each Enroll decides on a seat. While a seat is open, it promotes the earliest eligible waiter, scanning in FIFO order. Skipped waiters keep their position. If nobody is eligible, the seat stays open. A new eligible learner can then take it via Enroll without jumping ahead of any eligible waiter. A waitlist drop frees no seat, so it never promotes.

Concurrency

One lock around every operation and the snapshot, so check-then-act is atomic. Concurrent collections were rejected because the invariants span two lists. The lookup is called inside the lock, which is acceptable for an in-memory fake.

Test map
Test	Covers
Capacity available → Enrolled	AC1
Full → Waitlisted	AC2
Duplicate enroll (enrolled and waitlisted)	AC2, invariant 2
Drop promotes earliest eligible (FIFO)	AC3, invariant 3
Prerequisite failure at join time	AC1, AC4
Revoked completion: waiter skipped but retained	AC4
Invalid id (null, blank)	AC5
Repeated or unknown drop → NotFound, state unchanged	AC5, invariant 4
Waitlist drop: no promotion	promotion rule
No queue-jumping on Enroll	invariant 5
Concurrency: N parallel enrolls at capacity K, start barrier, no sleeps; plus same learner enrolled in parallel	invariants 1 and 2
Limitations
The lookup is called inside the lock, so it would need rethinking if it became I/O.
Ineligible entries can linger in the queue indefinitely.
No persistence, and no waitlist size limit.
A seat can stay open after a lookup change until the next drop or Enroll triggers a promotion pass.
Provenance

First draft written by me and revised after an AI design review. Every accept, edit or reject decision is recorded in docs/AI_USAGE_LOG.md.