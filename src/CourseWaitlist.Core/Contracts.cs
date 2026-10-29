namespace CourseWaitlist.Core;

// Supplies learner completions. The contract: never throws.
public interface ICompletedCourseLookup
{
    bool HasCompleted(string learnerId, string courseId);
}

public enum EnrollStatus
{
    Enrolled,
    Waitlisted,
    AlreadyEnrolled,
    AlreadyWaitlisted,
    NotEligible,
    InvalidRequest
}

public enum DropStatus
{
    Dropped,
    RemovedFromWaitlist,
    NotFound,
    InvalidRequest
}

// PromotedIds is empty unless an enrolled learner was dropped and a waiter got the seat.
public record DropResult(DropStatus Status, IReadOnlyList<string> PromotedIds);

// A copy of the state at one moment. Changing it does not affect the service.
public record EnrollmentSnapshot(IReadOnlyList<string> Enrolled, IReadOnlyList<string> Waitlist);