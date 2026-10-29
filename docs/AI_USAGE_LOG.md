# AI Prompt and Use Record

Tool : Sonnet 5.5 (web chat)


## AI-1: Requirements review


**My understanding, written before asking AI:**

**PLEASE EXCUSE MY GRAMMER AND TYPOS**
so here is my understdning that is we have to just desing an simple corsle enrolment feature slice nothing that should support the candidate enrolments dropping them and waiting list that will fifo based but only eligble learner in the waiting list and also we dont knwo who are eligbile 
so i think i have to or we have to decide the policy or rules for eligble learners and for dropping as well i guess as just dropping mention  then we need poliy for this as well  and also  and acpetence cretiera states that duplcaites remove and when capacity remains enorl or lelgieb learning from witing list to promtion 
so that mans first erolment with  capactiy exists nad when full remaing will go to waiting list then if spmeone drops may be selft or form teaaching site or for any reason again tha can be filled using waitg lsit  and obvaus enaligbel elanrer not promoted so they are saying doc policy whihc policy ithknk we not decide for learning elgibltiy and dropping and ovsouly covering test case for all edge caes too and scope budnreis its just a future slise we dont need anything extra 
so no fonrt end end simple and main polices for elgible learner and waiting list assumtopns so for drpping as well we need polices and final solution will be only service of methods that will be PASSED wiht paramters and they will return whter enrolled or dropped or waiting list and also we do have waiting list queue fifo one so just will cover duplcaited and also the inavlid scneros along wiht test cases covering and will not have any cli.

**Prompt sent:**

Hi, I am an software engg giving an enginnering assesmnet we have problm stametn we need to completetargetting .net 10  so first i will be gicing u the my understanding what exaclty need to be done befoer i design anything so ur role is act as a sneior architect  read problem statement caerfully  anlasye it proelrey and then tell me what gaps is there in my understanding and sugges me what need to be corrected in my underdating if any then i will review it and i will move towards desing sam aagin i will dsign and give me perspective then take ur arhiteclt lelvel suggstion designs and improvments  so here is the problem stament let me know if u have any questions  here are the all details  

 request you to complete the assessment within the stipulated timeline.
 
As part of our evaluation process, we would like you to complete the attached AI Engineering Assessment. This assessment is designed to evaluate your technical skills, AI assisted engineering, Prompt Engineering , problem-solving approach, and understanding of modern software engineering practices.
 
Submission Instructions
•   Complete the assigned assessment as per the instructions

 so here is my understdning that is we have to just desing an simple corsle enrolment feature slice nothing that should support the candidate enrolments dropping them and waiting list that will fifo based but only eligble learner in the waiting list and also we dont knwo who are eligbile so i think i have to or we have to decide the policy or rules for eligble learners and for dropping as well i guess as just dropping mention  then we need poliy for this as well  and also  and acpetence cretiera states that duplcaites remove and when capacity remains enorl or lelgieb learning from witing list to promtion so that mans first erolment with  capactiy exists nad when full remaing will go to waiting list then if spmeone drops may be selft or form teaaching site or for any reason again tha can be filled using waitg lsit  and obvaus enaligbel elanrer not promoted so they are saying doc policy whihc policy ithknk we not decide for learning elgibltiy and dropping and ovsouly covering test case for all edge caes too and scope budnreis its just a future slise we dont need anything extra so no fonrt end end simple and main pl=olices for elgible learner and waiting list assumtopns so for drpping as well we need polices and final solution will be only service of methods that will be alled wiht paramters and they will erturn whter enrolled or dropped or waiting list and also we do have waiting list queue fifo one so just will cover duplcaited and also the inavlid scneros along wiht test cases covering and will not have any cli as we are very late and left wiht less then half time  so revew my nunderstading wiht acutl one does it mathc and what impromenetns need and then next promp will decide about the design i iwll do same i will give my desing then lets review same so

and alos imprtant mention dont start directly writing code untill i say lets go sllwoly step by step and u will lnot have any code or files access and ya just dont go with me corect me and disucss and finalize thigns whats sound croret

and also sotre my prompts as i need to update prmtops too so give me my exact promot and summarise verison what i ask for and then will follow up

**Response**


Architect review of your understanding
Verdict

Your understanding is about 80% right. The core is correct: one offering, enroll, drop, FIFO waitlist, duplicate protection, defined eligibility, edge-case tests, no frontend and no extras. The gaps are in what you left out, and a few points need tightening.

What matches the document
Scope: a single-offering slice with no catalog, payments, UI or identity.
Policy decisions are ours: the document does not define eligibility, so we must state the prerequisite and waitlist assumptions.
Order of events: enroll if there is capacity, waitlist if full, and promote the earliest eligible learner when a seat opens.
Tests: they must cover edge cases, and the document lists the exact categories.
No configurable policy engine: keep the rules simple and documented.
Gaps and corrections

1. Check for a starter project first (biggest gap).
The document says to work in the supplied starter and follow its data model, persistence, test conventions and any locking pattern. Only if none exists do we use the minimal fallback. Do you have a starter? If yes, the design must follow it. If not, a service with an in-memory store is fine. The fallback allows “service or CLI”, so skipping the CLI is acceptable.

2. Eligibility is a prerequisite rule, not a general concept.
The document says “prerequisite” specifically. We must define it, for example: the learner must have completed the offering’s required course(s). That means the service needs a way to know a learner’s completed courses. We need a minimal lookup for this, not a full learner system.

3. The ineligible-learner policy is the central decision.
You noted that we need a policy, but the document is explicit: “what happens when an ineligible learner reaches the front of the queue.” There are two options:

Skip and retain: the learner keeps their position and is passed over for now.
Skip and remove: the learner is dropped from the queue.

Related point: acceptance criterion 2 implies ineligible learners cannot join the waitlist at all, so eligibility is checked at join time. We should also decide whether to re-check at promotion time, because eligibility can change while someone waits. Whichever we choose must be applied consistently.

4. Dropping does not need a “policy” about who or why.
Drop reasons (self, instructor, other) are out of scope. What we need are behaviors:

Dropping an enrolled learner frees a seat and triggers promotion.
Dropping a waitlisted learner: do we allow it and remove them from the queue? We should decide.
Dropping an unknown or already-dropped learner returns a clear result, with no seat change and no promotion.

5. Duplicates need more than “remove duplicates”.
We need distinct outcomes for: already enrolled, already waitlisted (position unchanged), and re-enrolling after a drop (we should decide, for example, that they go to the back of the queue).

6. Return values need to be richer than “enrolled / dropped / waitlisted”.
We also need results for already enrolled, already waitlisted, not eligible, not found, invalid request, and “dropped and promoted X”. Explicit result types are cleaner than a mix of exceptions and strings.

7. Concurrency is missing from your summary.
Even in memory, two simultaneous enrollments for the last seat must not both succeed. The document asks us to preserve the starter’s locking pattern and add a focused concurrency test where practical. We need a thread-safe approach and one test for it.

8. Deliverables you did not list.

A design specification, kept proportionate to the feature.
An AI prompt and use record: prompts, response summaries, accepted/edited/rejected, and verification evidence.
Setup and run instructions with test commands.
A short limitations and follow-up risks summary.

9. The invariants must be stated. For example: enrolled count never exceeds capacity, a learner appears at most once across enrolled and waitlist, waitlist order is FIFO, and a failed operation leaves state unchanged.

10. Time. The suggested effort is 7-8 hours and you have less than half. The priority order should be capacity and queue correctness, then tests, then a short design doc, then the AI record. Skip everything else.

Questions for you
Is there a starter project, or are we on the fallback?
Is a database required, or is in-memory acceptable?
Do you have any hint about the prerequisite rule from the starter or elsewhere?
Should the waitlist have a size limit? I would say no, to keep it simple.

My leaning on the open decisions
Prerequisite: the learner must have completed the offering’s required course(s).
Ineligible at the front: skip and retain their position, and promote the next eligible learner.
Re-check eligibility at promotion time.
Dropping a waitlisted learner: allowed.
Re-enrolling after a drop: allowed, goes to the back of the queue.

**Response summary:** Understanding ~80% right. Flagged: check for a starter, define prerequisite, ineligible-at-front policy, richer results, drop behaviours, concurrency, invariants, missing deliverables.


**MY Decisions:**
- **No starter project, use the fallback:** Accepted. None was supplied.
- **Prerequisite = all required courses completed:** Edited. I made it an AND rule using a simple learner lookup.
- **Skip and retain an ineligible waiter, re-check eligibility at promotion:** Accepted. Eligibility can change, and this keeps FIFO fair.
- **Allow drop from the waitlist; repeated or unknown drop is a no-op:** Accepted. State is never corrupted.
- **Distinct duplicate results; re-enrol after a drop goes to the back:** Accepted. Keeps FIFO honest.
- **Result types instead of exceptions:** Accepted. Gives clear outcomes.
- **Lock plus a concurrency test:** Accepted. Required by the brief.
- **Skip the CLI:** Accepted. The brief allows it, tests are the callers, and time is short.

Prompt log

Prompt 2: exact text

no starter (fallback); in-memory as mention in docs; no prerequisite hint, so I define it; no waitlist limit.

Summarized version

Answered my four questions: no starter project (use the fallback), in-memory storage as the document allows, no prerequisite hint so we define the rule, and no waitlist size limit.

AI use record entry

Purpose: scope confirmation and policy proposal.
Response summary: locked in the fallback and in-memory approach, and proposed policies for prerequisites (all-of), eligibility timing (join and promotion), ineligible learners at the front (skip and retain), drops, duplicates, result types, locking and invariants.


## AI-2 : Answers to its questions and its policy proposals and design review

ok here are my answers to ur 3 questions, 1 yes all of prerequisites, 2 yes skip and retain, 3 yes expose a readonly snapshot. i accept ur other proposed policies except one change, Drop should return just one status "Dropped" plus a list of promoted learner ids, instead of a separate DroppedAndPromoted, to keep the status list small.

now here is my design, review it like a skeptical senior architect, still no code please.

scope: Enroll, Drop and fifo waitlist promotion for one course, inmemory, .net 10 service class and xunit tests. no ui, cli, database, catalog, payments or policy engine.

assumptions: eligible = completed ALL required courses (if none required everyone is eligible). completed courses come from a small readonly lookup interface which we fake in tests. ineligible learner is rejected at join time and never queued. at promotion time we check eligibility again, ineligible waiter is skipped but keeps his position. re-enroll after drop goes to the back. ids are trimed and case insensitive, blank or null id is InvalidRequest. unknown learner (not in lookup) is rejected.

service boundary: CourseEnrollmentService owns enrolled and waitlist state, only dependency is the lookup.

api: Enroll(id) returns Enrolled | Waitlisted(position) | AlreadyEnrolled | AlreadyWaitlisted(position) | NotEligible | UnknownLearner | InvalidRequest. Drop(id) returns Dropped(promoted ids) | RemovedFromWaitlist | NotFound | InvalidRequest. GetSnapshot() gives copy of enrolled and waitlist.

invariants: enrolled <= capacity, learner appears once across enrolled and waitlist, waitlist is fifo, rejected or noop requests change nothing, seat is never left open while an eligible learner is waiting.

promotion: after enrolled learner drops, promote earliest eligible waiter, if nobody is eligible the seat stays open.

concurrency: one lock around every operation so check then act is atomic. lookup is called inside the lock, ok for inmemory, i will write it as a limitation.

tradeoffs: no persistence, no waitlist limit, O(n) scans.

what i want from u:
1. find contradictions or gaps between my design and the acceptance criteria
2. find edge cases my design doesnt handle, specially around the "seat open while eligible learner waits" invariant and when eligibility changes
3. tell me what to cut so its proportionate for a small slice
bullet points only, dont rewrite my design and dont write code.

**RESPONSE SUMMARY**
Summarized version

Confirmed all-of prerequisites, skip-and-retain, and a read-only snapshot.
Accepted my other policies, but edited one: Drop returns a single Dropped status plus a list of promoted ids.
Submitted my own design (scope, assumptions, API, invariants, promotion, concurrency, tradeoffs).
Asked for a skeptical review: contradictions with the acceptance criteria, edge cases around the open-seat invariant and eligibility changes, and what to cut. Bullets only, no code.

AI use record entry

Purpose: design review.
Response summary: flagged the UnknownLearner vs “everyone eligible” contradiction, the unstated Enroll check order, the missing promotion trigger that breaks the open-seat invariant, queue-jumping on Enroll, the lookup-throws atomicity gap, and a list of cuts.
Status of earlier proposals:
Prerequisite all-of: accepted.
Skip and retain: accepted.
Snapshot: accepted.
Drop result: edited by you (single Dropped plus promoted ids).
Everything else from Prompt 2: accepted.

**DETAILED RESPONSE**

Skeptical architect review
1. Contradictions and gaps against the acceptance criteria
Contradiction: “everyone is eligible if no prerequisites” vs “unknown learner is rejected”.
With zero required courses, eligibility needs no lookup data at all, yet UnknownLearner requires the lookup to know who exists.
That is a learner registry, which sits close to the out-of-scope “identity integrations”.
It is also an extra status, an extra lookup method, and extra tests.
Either treat an unknown learner as having completed nothing (eligible only if no prerequisites), or accept the registry cost. I lean toward the first.
Inconsistency with your own Drop change.
You collapsed DroppedAndPromoted into Dropped(promoted ids) to keep statuses small, but the API still lists RemovedFromWaitlist.
Decide whether a waitlist drop is Dropped with an empty list or stays a distinct status, and note that an empty list alone can’t tell “enrolled learner dropped, nobody eligible” from “waiter removed”.
Unstated order of checks in Enroll.
Invalid id, then already enrolled, then already waitlisted, then eligibility, then capacity, is the natural order, but you haven’t written it down.
If eligibility comes first, a waiter who has since become ineligible gets NotEligible instead of AlreadyWaitlisted.
You must also state that NotEligible never removes an existing waiter. Criterion 2 (no duplicate records) depends on this order.
The list in Dropped(promoted ids) is unjustified as designed. One drop frees one seat, so the list holds at most one id unless seats were already open. Either explain when it can hold more (see section 2) or use a single optional id.
Capacity and prerequisite configuration is missing from the design.
It doesn’t say where capacity and required course ids are supplied, or what happens on capacity ≤ 0 or on null or blank required ids.
It doesn’t say whether course ids are normalized like learner ids.
“Position” is undefined.
Is it 1-based, and does it count ineligible waiters ahead of you?
Position isn’t required by any acceptance criterion, and the snapshot already proves ordering.
Normalization is only half specified.
Pick one rule: trim plus ordinal case-insensitive, never culture-sensitive.
Say which casing the snapshot and the lookup see, since the same id must reach the lookup in one canonical form.
2. Edge cases around “seat open while an eligible learner waits”
The invariant is not actually guaranteed.
Promotion only runs after an enrolled learner drops.
Suppose a seat opens, all waiters are ineligible, and the seat stays open. If a waiter later becomes eligible, nothing triggers promotion.
The invariant is then violated until some unrelated drop happens.
Either weaken it (“holds at the end of each operation, per the lookup at that moment”) or add a rule that every mutating operation ends with a promotion pass.
Queue-jumping on Enroll.
With a seat open and ineligible-then-eligible waiters in the queue, a new eligible learner calling Enroll sees “capacity remains” and is enrolled directly, ahead of the waiters. That breaks FIFO.
Enroll needs a rule: run the promotion pass first, then take the seat only if one remains.
How a waiter becomes ineligible at all.
Learners are only queued while eligible. So the only way to be ineligible at promotion time is for the lookup to revoke or correct a completion.
Say that in the spec, because a reviewer will ask why skip-and-retain exists. Your fake lookup needs a way to revoke completions, and that is how tests reach this path.
Waitlist drop and the promotion pass.
If a pass runs after every mutation, removing a waiter could promote someone else, which means RemovedFromWaitlist could carry promoted ids.
Decide whether a waitlist drop is allowed to promote. That choice also settles the status question in section 1.
All waiters ineligible.
The seat stays open and the queue keeps its entries, which is consistent. Add a test that a later eligible Enroll takes the seat while the ineligible waiters keep their order.
Lookup throws mid-operation.
If the drop removes the learner and then the lookup throws during promotion, state is half-changed. That contradicts “failed operations change nothing”.
Either evaluate eligibility before mutating, or state in the spec that the lookup contract is non-throwing.
Re-enrollment after a drop.
A learner dropped while a seat is open and others are waiting must not jump ahead of those waiters. The Enroll rule above covers this, but write a test for it.
Snapshot consistency.
The snapshot must take the same lock and return immutable copies, otherwise tests can observe torn state.
Sync vs async. Keep the API synchronous, since awaiting inside a lock is a trap. Say so in the spec.
3. What to cut for proportion
UnknownLearner and any learner-existence concept. Treat unknown as “no completions”.
Position values in results, unless you need them. The snapshot plus FIFO tests are enough.
The RemovedFromWaitlist status, if you fold it into Dropped.
“O(n) scans” as a tradeoff. It is a non-issue at this scale, so one clause is enough.
The scope paragraph repeating the problem statement’s exclusions. One line suffices.
Any service interface, DI, logging, repository abstraction, or async. They add no value to the criteria.
The invariant list: keep four or five, each mapped to a named test.
The concurrency test: keep exactly one.
Use a start barrier and no sleeps.
Include a same-learner-hammered-in-parallel case, because that is where duplicate records would appear.
The limitations section: three or four lines (in-lock lookup, ineligible entries lingering, no persistence, no waitlist limit).
Verdict
The design is close, and the two real defects are the missing promotion trigger and the Enroll queue-jump. Fix those, cut UnknownLearner, and the rest is proportionate.


### Prompt 3: Design review

**Exact prompt:**
> ok here are my answers to ur 3 questions, 1 yes all of prerequisites, 2 yes skip and retain, 3 yes expose a readonly snapshot. i accept ur other proposed policies except one change, Drop should return just one status "Dropped" plus a list of promoted learner ids, instead of a separate DroppedAndPromoted, to keep the status list small.
>
> now here is my design, review it like a skeptical senior architect, still no code please.
>
> [my full design draft: scope, assumptions, service boundary, api, invariants, promotion, concurrency, tradeoffs; paste your original text here]
>
> what i want from u:
> 1. find contradictions or gaps between my design and the acceptance criteria
> 2. find edge cases my design doesnt handle, specially around the "seat open while eligible learner waits" invariant and when eligibility changes
> 3. tell me what to cut so its proportionate for a small slice
> bullet points only, dont rewrite my design and dont write code.

**Summary:** Confirmed all-of prerequisites, skip-and-retain and a read-only snapshot. Edited the Drop result to a single `Dropped` plus promoted ids. Submitted my own design for a skeptical review.

**AI response summary:** Flagged the `UnknownLearner` vs "everyone eligible" contradiction, the unstated Enroll check order, the missing promotion trigger that breaks the open-seat invariant, queue-jumping on Enroll, lookup-throws atomicity, and a list of things to cut.

**Decisions:**
- Accepted: removed `UnknownLearner` (unknown learner = no completions), explicit Enroll check order, promotion pass before Enroll decides on a seat, non-throwing lookup contract, synchronous API, positions removed from results.
- Edited: invariant 5 reworded later (see Prompt 4).
- Rejected: folding `RemovedFromWaitlist` into `Dropped`. I kept it separate so "waiter removed" can be told apart from "enrolled learner dropped, nobody eligible".

**Files/decisions informed:** `docs/DESIGN.md`
**Verification:** each point was checked against the acceptance criteria and later covered by a named test (queue-jumping test, revoked-completion test).

Prompt log

Prompt 4: exact text (verbatim)

NOW LETS finialze the desing give me the final design now so how about this
(the full “Design: Course Waitlist (one offering)” document as pasted, covering Scope, Assumptions, Service boundary, State and configuration, API, Invariants, Promotion rule, Concurrency, Limitations, Provenance)

or just let this final as i dont have time

Short answer: the document does not ask for an HTTP API. We're fine on that, but the design is slightly heavier than it needs to be.

**On "API":** the document says "a small service or command-line prototype with an in-memory store is sufficient." Our "API" is just the public methods of one service class (`Enroll`, `Drop`, `GetSnapshot`). No REST, no controllers. That matches the document, so keep it. If you like, rename the heading to "Service methods" to avoid confusion.

**Are we going too deep?** Slightly, but it's all defensible. Everything maps to an acceptance criterion or a deliberate edge case. Things you could trim if time is tight:
- The test map table: keep the list, drop the "Covers" column.
- Limitations: three lines are enough.
- Provenance: one line.
- The extra "why" paragraphs: one sentence each.

**Don't trim:** the check order, the promotion rule, the skip-and-retain policy, and the invariants. Those are what reviewers grade.

**Verdict:** yes, we're good. Stop designing. Say "let's go" when you want code.

---

# Design: Course Waitlist (one offering)

## Scope
Enroll, Drop and FIFO waitlist promotion for one course. In-memory .NET 10 service class plus xUnit tests. No UI, CLI, database, catalog, payments, identity integration or policy engine.

## Assumptions
- **Eligibility:** a learner is eligible if they have completed ALL required courses. If none are required, everyone is eligible.
- **Lookup:** a small read-only lookup (`HasCompleted(learnerId, courseId)`) supplies completions. An unknown learner simply has no completions. The lookup never throws (contract), and it always receives normalized ids.
- **Join time:** an ineligible learner is rejected and never queued.
- **Promotion time:** a waiter can only become ineligible if the lookup later revokes a completion. At promotion we re-check, and an ineligible waiter is SKIPPED but RETAINED in place.
- **Re-enrollment:** a learner who re-enrolls after a drop goes to the back of the queue.
- **Normalization:** learner ids and required course ids are trimmed and compared with ordinal case-insensitive comparison. A null or blank learner id is `InvalidRequest`.
- **Construction rules:** capacity ≤ 0, a null lookup, or a null or blank required course id throws at construction (programmer error).

## Service boundary
`CourseEnrollmentService` owns the enrolled and waitlist state. Its only dependency is the lookup. The API is synchronous, because awaiting inside a lock is a trap.

## State and configuration
- **State:** an enrolled list and a waitlist (front = earliest).
- **Configuration:** capacity and the set of required course ids, fixed at construction.

## API
- **Enroll(id):** `Enrolled | Waitlisted | AlreadyEnrolled | AlreadyWaitlisted | NotEligible | InvalidRequest`
- **Drop(id):** `Dropped (+ promoted ids) | RemovedFromWaitlist | NotFound | InvalidRequest`
- **GetSnapshot():** immutable copy of enrolled and waitlist, taken under the lock.

**Why `Dropped` carries a list:** the promotion pass loops while a seat is open. One drop can therefore promote more than one learner if seats were already open because earlier waiters were ineligible. The list is usually empty or has one id.

**Why `RemovedFromWaitlist` is separate:** it distinguishes "a waiter was removed" from "an enrolled learner dropped and nobody was eligible to promote". An empty promoted list alone can't tell these apart.

**Enroll check order:**
1. Invalid id → `InvalidRequest`
2. Already enrolled → `AlreadyEnrolled`
3. Already waitlisted → `AlreadyWaitlisted`
4. Not eligible → `NotEligible`
5. Run the promotion pass
6. Seat open → `Enrolled`, otherwise append to the waitlist → `Waitlisted`

`NotEligible` never removes an existing waiter, because the waitlist check comes first.

## Invariants
1. Enrolled count ≤ capacity.
2. A learner appears at most once across enrolled and waitlist.
3. Waitlist order is FIFO.
4. Rejected or no-op requests change nothing.
5. No seat is open while an eligible waiter exists, as of the last promotion pass, per the lookup at that moment. The pass runs after every enrolled-learner drop and before every Enroll that reaches step 5.

## Promotion rule
The promotion pass runs after an enrolled learner drops and before each Enroll decides on a seat. While a seat is open, it promotes the earliest eligible waiter, scanning in FIFO order. Skipped waiters keep their position. If nobody is eligible, the seat stays open. A new eligible learner can then take it via Enroll without jumping ahead of any eligible waiter. A waitlist drop frees no seat, so it never promotes.

## Concurrency
One lock around every operation and the snapshot, so check-then-act is atomic. Concurrent collections were rejected because the invariants span two lists. The lookup is called inside the lock, which is acceptable for an in-memory fake.

## Test map
| Test | Covers |
|---|---|
| Capacity available → `Enrolled` | AC1 |
| Full → `Waitlisted` | AC2 |
| Duplicate enroll (enrolled and waitlisted) | AC2, invariant 2 |
| Drop promotes earliest eligible (FIFO) | AC3, invariant 3 |
| Prerequisite failure at join time | AC1, AC4 |
| Revoked completion: waiter skipped but retained | AC4 |
| Invalid id (null, blank) | AC5 |
| Repeated or unknown drop → `NotFound`, state unchanged | AC5, invariant 4 |
| Waitlist drop: no promotion | promotion rule |
| No queue-jumping on Enroll | invariant 5 |
| Concurrency: N parallel enrolls at capacity K, start barrier, no sleeps; plus same learner enrolled in parallel | invariants 1 and 2 |

## Limitations
- The lookup is called inside the lock, so it would need rethinking if it became I/O.
- Ineligible entries can linger in the queue indefinitely.
- No persistence, and no waitlist size limit.
- A seat can stay open after a lookup change until the next drop or Enroll triggers a promotion pass.

## Provenance
First draft written by me and revised after an AI design review. Every accept, edit or reject decision is recorded in `docs/AI_USAGE_LOG.md`.

---

# Prompt log

**Prompt 5: exact text (verbatim)**

> so giv eme fianal design now

**Summarized version**
- Asked for the final design with the Prompt 4 review edits applied.

**AI use record entry**
- **Purpose:** design finalization.
- **Response summary:** produced the final design with the three edits applied: reworded invariant 5, an explanation of why `Dropped` carries a list, and construction and normalization rules. I also added a test map and a fourth limitation (a seat can stay open until the next trigger).

Say "let's go" when you want to start the code.

# Prompt log

**Prompt 6: exact text (verbatim)**

> but in documetn do they ask to use api or just simpler nad smlaaer as possible rihgt covering all scnriaos aso aew we good wiht this desing just asking? or are we going deep just asnwer in small

**Summarized version**
- Asked whether the assessment requires an HTTP API or just a small service, whether the design covers all scenarios, and whether we are over-engineering.

**AI use record entry**
- **Purpose:** scope and proportionality check.
- **Response summary:** confirmed no HTTP API is required (a service class suffices), design is slightly heavy but justified, suggested trimming prose and keeping the core rules.

**DESING FIINILIZED**


**CODING**
Prompt log

**Prompt 7: exact text (verbatim) **

lets start coding the design is final and saved in docs/DESIGN.md (the one u gave me above, dont change it). lets go now, but step by step.

step 1 only: give me the exact commands for windows powershell to scaffold the project in my current folder, .net 10, solution plus a class library CourseWaitlist.Core under src and an xunit test project CourseWaitlist.Tests under tests, with the test project referencing the core, then dotnet build. tell me which default files to delete (like Class1.cs, UnitTest1.cs) and anything different in .net 10 (like the solution file format). commands only with one line of explanation each, no code yet. wait for my ok.

Summarized version

Design is final and saved unchanged in docs/DESIGN.md. Start implementation step by step.
Step 1 only: PowerShell commands to scaffold a .NET 10 solution, with a core class library and an xUnit test project referencing it, then build.
Also list which template files to delete and what is different in .NET 10. Commands only, no code, wait for my ok.

AI use record entry

Purpose: project scaffolding.
Response summary: provided the CLI commands, the placeholder files to delete, the .slnx default in .NET 10 with the --format sln fallback, and a recommendation to use xUnit v2.
Verification to record: paste your dotnet --version and dotnet build output here, and confirm TargetFramework is net10.0.


# Step 1: Scaffold (Windows PowerShell, run from your project root)

```powershell
dotnet --version
```
Confirms the SDK is 10.x before anything else. If it isn't, install the .NET 10 SDK first.

```powershell
dotnet new sln -n CourseWaitlist
```
Creates the solution file. In the .NET 10 SDK this produces `CourseWaitlist.slnx` (the new XML format) by default.

```powershell
dotnet new classlib -n CourseWaitlist.Core -o src/CourseWaitlist.Core
```
Creates the class library under `src`.

```powershell
dotnet new xunit -n CourseWaitlist.Tests -o tests/CourseWaitlist.Tests
```
Creates the xUnit test project under `tests`.

```powershell
dotnet sln CourseWaitlist.slnx add src/CourseWaitlist.Core/CourseWaitlist.Core.csproj tests/CourseWaitlist.Tests/CourseWaitlist.Tests.csproj
```
Adds both projects to the solution.

```powershell
dotnet add tests/CourseWaitlist.Tests/CourseWaitlist.Tests.csproj reference src/CourseWaitlist.Core/CourseWaitlist.Core.csproj
```
Makes the test project reference the core library.

```powershell
Remove-Item src/CourseWaitlist.Core/Class1.cs
Remove-Item tests/CourseWaitlist.Tests/UnitTest1.cs
```
Deletes the two template placeholder files. If either path doesn't exist, the template didn't generate it, so skip it.

```powershell
dotnet build
```
Builds the whole solution. It should finish with 0 warnings and 0 errors.

## What's different in .NET 10
- **Solution format:** the default is now `.slnx`, not `.sln`. `dotnet sln add` and `dotnet build` work with it as usual. If a tool or reviewer needs the classic format, use `dotnet new sln -n CourseWaitlist --format sln` instead, and then use `CourseWaitlist.sln` in the add command.
- **Target framework:** the templates should target `net10.0`. Open both `.csproj` files and check the `TargetFramework` line.
- **xUnit version:** `dotnet new xunit` gives the familiar xUnit v2 template. There is also `dotnet new xunit3` for v3, but I'd stay on v2 to avoid surprises with the time you have left.

## Quick checks after the build
- The folder tree should be `src/CourseWaitlist.Core`, `tests/CourseWaitlist.Tests`, `docs/DESIGN.md` and `CourseWaitlist.slnx`.
- `dotnet test` should run with 0 tests and no errors. That confirms the test project is wired up.

Reply with the build output or "ok" and I'll give step 2. I'm holding off on code until then.

---

# Prompt log

**Prompt 7: exact text (verbatim)**

> lets start coding  the design is final and saved in docs/DESIGN.md (the one u gave me above, dont change it). lets go now, but step by step.
>
> step 1 only: give me the exact commands for windows powershell to scaffold the project in my current folder, .net 10, solution plus a class library CourseWaitlist.Core under src and an xunit test project CourseWaitlist.Tests under tests, with the test project referencing the core, then dotnet build. tell me which default files to delete (like Class1.cs, UnitTest1.cs) and anything different in .net 10 (like the solution file format). commands only with one line of explanation each, no code yet. wait for my ok.

**Summarized version**
- Design is final and saved unchanged in `docs/DESIGN.md`. Start implementation step by step.
- Step 1 only: PowerShell commands to scaffold a .NET 10 solution, with a core class library and an xUnit test project referencing it, then build.
- Also list which template files to delete and what is different in .NET 10. Commands only, no code, wait for my ok.

**AI use record entry**
- **Purpose:** project scaffolding.
- **Response summary:** provided the CLI commands, the placeholder files to delete, the `.slnx` default in .NET 10 with the `--format sln` fallback, and a recommendation to use xUnit v2.
- **Verification to record:** paste your `dotnet --version` and `dotnet build` output here, and confirm `TargetFramework` is `net10.0`.


PROMPT 8 coding rules
build was sucessfull and here are the outptus PS C:\Users\Universal\course-waitlist-assessment> git add .
PS C:\Users\Universal\course-waitlist-assessment> git commit -m "docs: AI-1 requirements review with decisions"
>> 
[main c6ee8f8] docs: AI-1 requirements review with decisions
 1 file changed, 131 insertions(+)
PS C:\Users\Universal\course-waitlist-assessment> git push
Counting objects: 100% (7/7), done.
Delta compression using up to 4 threads
Compressing objects: 100% (3/3), done.
Writing objects: 100% (4/4), 4.53 KiB | 928.00 KiB/s, done.
Total 4 (delta 1), reused 0 (delta 0), pack-reused 0 (from 0)
remote: Resolving deltas: 100% (1/1), completed with 1 local object.
To https://github.com/MD-Wajeed/course-waitlist-asgit add .git
>> git commit -m "docs: final design"
>> git push\Universal\course-waitlist-assessment> 
[main 13338ad] docs: final design
 2 files changed, 326 insertions(+)
 create mode 100644 docs/DESIGN.md
Counting objects: 100% (8/8), done.
Compressing objects: 100% (5/5), done.
Writing objects: 100% (5/5), 12.28 KiB | 3.07 MiB/s, done.
remote: Resolving deltas: 100% (1/1), completed with 1 local object.
To https://github.com/MD-Wajeed/course-waitlist-assessment.git
   c6ee8f8..13338ad  main -> main
PS C:\Users\Universal\course-waitlist-assessment> dotnet --version
10.0.403
PS C:\Users\Universal\course-waitlist-assessment> dotnet new sln -n CourseWaitlist
The template "Solution File" was created successfully.
PS C:\Users\Universal\course-waitlist-assessment> dotnet new classlib -n CourseWaitlist.Core -o src/CourseWaitlist.Core
The template "Class Library" was created successfully.

Processing post-creation actions...
Restoring C:\Users\Universal\course-waitlist-assessment\src\CourseWaitlist.Core\CourseWaitlist.Core.csproj:
Restore succeeded.


The template "xUnit Test Project" was created successfully.

Processing post-creation actions...
Restore succeeded.
ts/CourseWaitlist.Tests/CourseWaitlist.Tests.csproj
Project `src\CourseWaitlist.Core\CourseWaitlist.Core.csproj` added to the solution.
Project `tests\CourseWaitlist.Tests\CourseWaitlist.Tests.csproj` added to the solution.
PS C:\Users\Universal\course-waitlist-assessment> dotnet add tests/CourseWaitlist.Tests/CourseWaitlist.Tests.csproj reference src/CourseWaitlist.Core/CourseWaitlist.Core.csproj
Reference `..\..\src\CourseWaitlist.Core\CourseWaitlist.Core.csproj` added to the project.
PS C:\Users\Universal\course-waitlist-assessment> Remove-Item src/CourseWaitlist.Core/Class1.cs
>> Remove-Item tests/CourseWaitlist.Tests/UnitTest1.cs
PS C:\Users\Universal\course-waitlist-assessment> dotnet build
Restore complete (3.1s)
  CourseWaitlist.Core net10.0 succeeded (11.7s) → src\CourseWaitlist.Core\bin\Debug\net10.0\CourseWaitlist.Core.dll
  CourseWaitlist.Tests net10.0 succeeded (8.1s) → tests\CourseWaitlist.Tests\bin\Debug\net10.0\CourseWaitlist.Tests.dll

Build succeeded in 24.3s
PS C:\Users\Universal\course-waitlist-assessment> 

and now next thing is code parts lets finsih  so 

main code rules

dont give me conmplex short codes give me undesrable code it shoild not have mutiple functions or lot of files just give it enohgh to folllw solid practices still not mutuilpe or lot of files of functions 
smple so a junior can read it few small methods, clear names, no clever linq chains, short commments only where not obvvious

Prompt log

Prompt 8: exact text (verbatim, rules part)

and now next thing is code parts lets finsih so

main code rules

dont give me conmplex short codes give me undesrable code it shoild not have mutiple functions or lot of files just give it enohgh to folllw solid practices still not mutuilpe or lot of files of functions
smple so a junior can read it few small methods, clear names, no clever linq chains, short commments only where not obvvious

(The message began with my pasted terminal output covering the git commits and the scaffold and build commands; I abbreviated it here. Paste it verbatim in your submission.)

Summarized version

Reported that the scaffold build succeeded, with the terminal output.
Asked for the core implementation next, with these rules: readable code, few files, few small methods, clear names, no clever LINQ, and short comments only where needed, while still following SOLID.

AI use record entry

Purpose: core implementation.
Files informed: src/CourseWaitlist.Core/Contracts.cs and CourseEnrollmentService.cs, following docs/DESIGN.md.
Response summary: produced two files with a single lock, a promotion pass that skips but retains ineligible waiters, and a lookup interface as the only abstraction.
Verification to record: dotnet build output with 0 warnings and 0 errors. After step 3, also record the dotnet test results.

Prompt log

Prompt 9: exact text (verbatim, the user-written part)

PS C:\Users\Universal\course-waitlist-assessment> dotnet build
Restore complete (1.9s)
CourseWaitlist.Core net10.0 succeeded (2.9s) → src\CourseWaitlist.Core\bin\Debug\net10.0\CourseWaitlist.Core.dll
CourseWaitlist.Tests net10.0 succeeded (4.2s) → tests\CourseWaitlist.Tests\bin\Debug\net10.0\CourseWaitlist.Tests.dll

Build succeeded in 10.3s
PS C:\Users\Universal\course-waitlist-assessment>

Summarized version

Pasted the successful dotnet build output for the core code from Prompt 8, with no warnings or errors.
Implicit request: continue to the next step, which is the tests.

AI use record entry

Purpose: test implementation.
Files informed: tests/CourseWaitlist.Tests/FakeLookup.cs and CourseEnrollmentServiceTests.cs, following the test map in docs/DESIGN.md.
Response summary: produced a thread-safe fake lookup and 11 tests covering capacity, full, duplicates, FIFO promotion, prerequisite failure, revoked completion (skip and retain), invalid ids, repeated or unknown drop, waitlist drop, no queue-jumping, and one concurrency test with a start gate.
Verification to record:
Step 2 result: dotnet build succeeded for the core code (output pasted in Prompt 9).
Step 3 result: paste your dotnet test output here (expected 11 passed).

Prompt log

Prompt 10: exact text (verbatim)

[xUnit.net 00:00:00.04] xUnit.net VSTest Adapter v3.1.4+50e68bbb8b (64-bit .NET 10.0.14)
[xUnit.net 00:00:01.54] Discovering: CourseWaitlist.Tests
[xUnit.net 00:00:02.13] Discovered: CourseWaitlist.Tests
[xUnit.net 00:00:02.25] Starting: CourseWaitlist.Tests
[xUnit.net 00:00:03.30] Finished: CourseWaitlist.Tests
CourseWaitlist.Tests test net10.0 succeeded (12.5s)

Test summary: total: 11, failed: 0, succeeded: 11, skipped: 0, duration: 12.3s
Build succeeded with 1 warning(s) in 17.9s

Summarized version

Pasted the dotnet test output for the step 3 tests: 11 total, 11 succeeded, 0 failed, with 1 build warning.

AI use record entry

Purpose: test verification and next-step planning.
Verification evidence: dotnet test shows 11 passed, 0 failed, 0 skipped on .NET 10.0.14 with the xUnit.net VSTest Adapter v3.1.4.
Correction to record: in Prompt 7’s response I said the xunit template gives xUnit v2. The output shows v3. Record this as an AI inaccuracy you caught through verification.
Open item: identify and resolve or document the 1 build warning.


### Prompt 11: Final deliverables planning
**Exact prompt:**
> we are running out of time left with 15 mins what next now

**Summary:** Said 15 minutes remained and asked what to do next.
**AI response summary:** Gave a prioritized checklist: check the warning, rerun tests, README with limitations, AI log, review note, commit and push.
**Decision:** Accepted. Followed the order.

### Prompt 12: Warning diagnosis
**Exact prompt:**
> [paste your original message verbatim: the `dotnet build --no-incremental` and `dotnet test` output showing warning xUnit1031 at CourseEnrollmentServiceTests.cs(209,14) and 11/11 passing]

**Summary:** Pasted the build output showing the single warning, plus a second test run with 11 of 11 passing.
**AI response summary:** Identified `Task.WaitAll` blocking in the concurrency test. Suggested `async Task` plus `await Task.WhenAll`.
**Decision:** Accepted. I checked it was not a deadlock risk (tasks only wait on a start gate) and applied the fix.
**Files changed:** `tests/CourseWaitlist.Tests/CourseEnrollmentServiceTests.cs`

### Prompt 13: Post-fix verification
**Exact prompt:**
> [paste your original message verbatim: the test output with 11 total, 11 succeeded, 0 failed, "Build succeeded in 10.6s"]

**Summary:** Pasted the test output after the fix.
**AI response summary:** Confirmed a clean build and a green run, and listed the remaining paperwork (README, AI log, commit and push).
**Verification evidence:** `dotnet test`: total 11, failed 0, succeeded 11. The build shows no warning count after the fix.
**Decision:** Accepted.

### Prompt 14: Log catch-up
**Exact prompt:**
> read me done ai usage is till 10 and promtp 3 i missed so need to update remaing along wiht 3

**Summary:** README is done. The AI log covered Prompts 1 to 10 but was missing Prompt 3, so I asked for Prompts 3 and 11 onward.
**AI response summary:** Provided the missing entries.
**Decision:** Accepted.

# Entries to add to `docs/AI_USAGE_LOG.md`

Insert Prompt 3 after Prompt 2, and append 11 to 14 at the end. If your log uses "AI-n" IDs, use your own numbering and make sure `DESIGN.md`'s provenance line still points to the right entries.

## Insert after Prompt 2: Prompt 3

```markdown
### Prompt 3: Design review

**Exact prompt:**
> ok here are my answers to ur 3 questions, 1 yes all of prerequisites, 2 yes skip and retain, 3 yes expose a readonly snapshot. i accept ur other proposed policies except one change, Drop should return just one status "Dropped" plus a list of promoted learner ids, instead of a separate DroppedAndPromoted, to keep the status list small.
>
> now here is my design, review it like a skeptical senior architect, still no code please.
>
> [my full design draft: scope, assumptions, service boundary, api, invariants, promotion, concurrency, tradeoffs; paste your original text here]
>
> what i want from u:
> 1. find contradictions or gaps between my design and the acceptance criteria
> 2. find edge cases my design doesnt handle, specially around the "seat open while eligible learner waits" invariant and when eligibility changes
> 3. tell me what to cut so its proportionate for a small slice
> bullet points only, dont rewrite my design and dont write code.

**Summary:** Confirmed all-of prerequisites, skip-and-retain and a read-only snapshot. Edited the Drop result to a single `Dropped` plus promoted ids. Submitted my own design for a skeptical review.

**AI response summary:** Flagged the `UnknownLearner` vs "everyone eligible" contradiction, the unstated Enroll check order, the missing promotion trigger that breaks the open-seat invariant, queue-jumping on Enroll, lookup-throws atomicity, and a list of things to cut.

**Decisions:**
- Accepted: removed `UnknownLearner` (unknown learner = no completions), explicit Enroll check order, promotion pass before Enroll decides on a seat, non-throwing lookup contract, synchronous API, positions removed from results.
- Edited: invariant 5 reworded later (see Prompt 4).
- Rejected: folding `RemovedFromWaitlist` into `Dropped`. I kept it separate so "waiter removed" can be told apart from "enrolled learner dropped, nobody eligible".

**Files/decisions informed:** `docs/DESIGN.md`
**Verification:** each point was checked against the acceptance criteria and later covered by a named test (queue-jumping test, revoked-completion test).
```

## Append at the end: Prompts 11 to 14

```markdown
### Prompt 11: Final deliverables planning
**Exact prompt:**
> we are running out of time left with 15 mins what next now

**Summary:** Said 15 minutes remained and asked what to do next.
**AI response summary:** Gave a prioritized checklist: check the warning, rerun tests, README with limitations, AI log, review note, commit and push.
**Decision:** Accepted. Followed the order.

### Prompt 12: Warning diagnosis
**Exact prompt:**
> [paste your original message verbatim: the `dotnet build --no-incremental` and `dotnet test` output showing warning xUnit1031 at CourseEnrollmentServiceTests.cs(209,14) and 11/11 passing]

**Summary:** Pasted the build output showing the single warning, plus a second test run with 11 of 11 passing.
**AI response summary:** Identified `Task.WaitAll` blocking in the concurrency test. Suggested `async Task` plus `await Task.WhenAll`.
**Decision:** Accepted. I checked it was not a deadlock risk (tasks only wait on a start gate) and applied the fix.
**Files changed:** `tests/CourseWaitlist.Tests/CourseEnrollmentServiceTests.cs`

### Prompt 13: Post-fix verification
**Exact prompt:**
> [paste your original message verbatim: the test output with 11 total, 11 succeeded, 0 failed, "Build succeeded in 10.6s"]

**Summary:** Pasted the test output after the fix.
**AI response summary:** Confirmed a clean build and a green run, and listed the remaining paperwork (README, AI log, commit and push).
**Verification evidence:** `dotnet test`: total 11, failed 0, succeeded 11. The build shows no warning count after the fix.
**Decision:** Accepted.

### Prompt 14: Log catch-up
**Exact prompt:**
> read me done ai usage is till 10 and promtp 3 i missed so need to update remaing along wiht 3

**Summary:** README is done. The AI log covered Prompts 1 to 10 but was missing Prompt 3, so I asked for Prompts 3 and 11 onward.
**AI response summary:** Provided the missing entries.
**Decision:** Accepted.
```

## Closing sections (add once, if not already there)

```markdown
## Decisions summary
- Accepted: all-of prerequisites, skip-and-retain, snapshot, promotion pass before Enroll, no UnknownLearner, construction rules, results instead of exceptions.
- Edited: Drop result design (my idea of a single `Dropped` plus promoted ids).
- Rejected: folding `RemovedFromWaitlist` into `Dropped`.

## AI errors caught
- AI said the xunit template gives v2. The test output showed the v3 adapter (v3.1.4). The README states v3.

## Independent review
- Build and tests were run repeatedly: 11 of 11 each time.
- The analyzer warning xUnit1031 was reviewed and fixed.
- I read the promotion loop by hand.
```

Only claim the checks you really did. If you read the promotion loop, say so, and if not, take that line out.

## Finish

```powershell
git add .
git commit -m "docs: complete AI usage log"
git push
```

Before you push, check that the verbatim blocks for Prompts 3, 4 and 12 contain your original pasted text, not the placeholders in brackets. I abbreviated those in earlier replies.

---

# Prompt log

**Prompt 14: exact text (verbatim)**

> read me done ai usage is till 10 and promtp 3 i missed so need to update remaing along wiht 3

**Summarized version**
- README is done. The AI log ends at Prompt 10 and is missing Prompt 3. I asked for the remaining entries (3, then 11 onward).

**AI use record entry**
- **Purpose:** AI log completion.
- **Response summary:** supplied the missing Prompt 3 entry, entries for Prompts 11 to 14, and the closing decisions, errors and review sections.


## Decisions summary
- Accepted: all-of prerequisites, skip-and-retain, snapshot, promotion pass before Enroll, no UnknownLearner, construction rules, results instead of exceptions.
- Edited: Drop result design (my idea of a single `Dropped` plus promoted ids).
- Rejected: folding `RemovedFromWaitlist` into `Dropped`.

## AI errors caught
- AI said the xunit template gives v2. The test output showed the v3 adapter (v3.1.4). The README states v3.

## Independent review
- Build and tests were run repeatedly: 11 of 11 each time.
- The analyzer warning xUnit1031 was reviewed and fixed.
- I read the promotion loop by hand.
