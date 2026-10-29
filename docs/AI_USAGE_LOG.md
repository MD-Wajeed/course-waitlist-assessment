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
Status: all proposals pending your accept, edit or reject decision.

