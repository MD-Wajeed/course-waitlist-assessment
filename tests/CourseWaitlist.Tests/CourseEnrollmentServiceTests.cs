using CourseWaitlist.Core;
using Xunit;

namespace CourseWaitlist.Tests;

public class CourseEnrollmentServiceTests
{
    // No prerequisites: everyone is eligible.
    private static CourseEnrollmentService NewOpenCourse(int capacity)
    {
        return new CourseEnrollmentService(capacity, new string[0], new FakeLookup());
    }

    // One prerequisite (CS101): a learner must be completed in the fake first.
    private static CourseEnrollmentService NewCourseWithPrerequisite(int capacity, FakeLookup lookup)
    {
        return new CourseEnrollmentService(capacity, new[] { "CS101" }, lookup);
    }

    [Fact]
    public void Enroll_WhenSeatAvailable_Enrolls()
    {
        var service = NewOpenCourse(2);

        Assert.Equal(EnrollStatus.Enrolled, service.Enroll("alice"));

        var snapshot = service.GetSnapshot();
        Assert.Equal(new[] { "alice" }, snapshot.Enrolled);
        Assert.Empty(snapshot.Waitlist);
    }

    [Fact]
    public void Enroll_WhenFull_Waitlists()
    {
        var service = NewOpenCourse(1);
        service.Enroll("alice");

        Assert.Equal(EnrollStatus.Waitlisted, service.Enroll("bob"));

        var snapshot = service.GetSnapshot();
        Assert.Equal(new[] { "alice" }, snapshot.Enrolled);
        Assert.Equal(new[] { "bob" }, snapshot.Waitlist);
    }

    [Fact]
    public void Enroll_Duplicate_DoesNotCreateSecondRecord()
    {
        var service = NewOpenCourse(1);
        service.Enroll("alice");
        service.Enroll("bob");

        Assert.Equal(EnrollStatus.AlreadyEnrolled, service.Enroll(" ALICE "));
        Assert.Equal(EnrollStatus.AlreadyWaitlisted, service.Enroll("Bob"));

        var snapshot = service.GetSnapshot();
        Assert.Single(snapshot.Enrolled);
        Assert.Single(snapshot.Waitlist);
    }

    [Fact]
    public void Drop_PromotesEarliestEligibleWaiter()
    {
        var service = NewOpenCourse(1);
        service.Enroll("alice");
        service.Enroll("bob");
        service.Enroll("carol");

        DropResult result = service.Drop("alice");

        Assert.Equal(DropStatus.Dropped, result.Status);
        Assert.Equal(new[] { "bob" }, result.PromotedIds);

        var snapshot = service.GetSnapshot();
        Assert.Equal(new[] { "bob" }, snapshot.Enrolled);
        Assert.Equal(new[] { "carol" }, snapshot.Waitlist);
    }

    [Fact]
    public void Enroll_WhenPrerequisiteMissing_IsNotEligibleAndNotQueued()
    {
        var lookup = new FakeLookup();
        var service = NewCourseWithPrerequisite(1, lookup);

        Assert.Equal(EnrollStatus.NotEligible, service.Enroll("alice"));

        var snapshot = service.GetSnapshot();
        Assert.Empty(snapshot.Enrolled);
        Assert.Empty(snapshot.Waitlist);
    }

    [Fact]
    public void Drop_WhenWaiterLostPrerequisite_SkipsButRetainsThem()
    {
        var lookup = new FakeLookup();
        lookup.Complete("alice", "CS101");
        lookup.Complete("bob", "CS101");
        lookup.Complete("carol", "CS101");
        var service = NewCourseWithPrerequisite(1, lookup);
        service.Enroll("alice");
        service.Enroll("bob");
        service.Enroll("carol");

        lookup.Revoke("bob", "CS101");
        DropResult result = service.Drop("alice");

        Assert.Equal(new[] { "carol" }, result.PromotedIds);

        var snapshot = service.GetSnapshot();
        Assert.Equal(new[] { "carol" }, snapshot.Enrolled);
        Assert.Equal(new[] { "bob" }, snapshot.Waitlist); // bob kept his place
    }

    [Fact]
    public void Enroll_WithNullOrBlankId_IsInvalidAndChangesNothing()
    {
        var service = NewOpenCourse(1);

        Assert.Equal(EnrollStatus.InvalidRequest, service.Enroll(null));
        Assert.Equal(EnrollStatus.InvalidRequest, service.Enroll(""));
        Assert.Equal(EnrollStatus.InvalidRequest, service.Enroll("   "));
        Assert.Equal(DropStatus.InvalidRequest, service.Drop(null).Status);
        Assert.Equal(DropStatus.InvalidRequest, service.Drop("  ").Status);

        var snapshot = service.GetSnapshot();
        Assert.Empty(snapshot.Enrolled);
        Assert.Empty(snapshot.Waitlist);
    }

    [Fact]
    public void Drop_UnknownOrRepeated_ReturnsNotFoundAndChangesNothing()
    {
        var service = NewOpenCourse(1);
        service.Enroll("alice");
        service.Enroll("bob");

        Assert.Equal(DropStatus.NotFound, service.Drop("zed").Status);

        service.Drop("alice"); // bob is promoted here
        Assert.Equal(DropStatus.NotFound, service.Drop("alice").Status);

        var snapshot = service.GetSnapshot();
        Assert.Equal(new[] { "bob" }, snapshot.Enrolled);
        Assert.Empty(snapshot.Waitlist);
    }

    [Fact]
    public void Drop_OfWaiter_RemovesThemWithoutPromoting()
    {
        var service = NewOpenCourse(1);
        service.Enroll("alice");
        service.Enroll("bob");
        service.Enroll("carol");

        DropResult result = service.Drop("bob");

        Assert.Equal(DropStatus.RemovedFromWaitlist, result.Status);
        Assert.Empty(result.PromotedIds);

        var snapshot = service.GetSnapshot();
        Assert.Equal(new[] { "alice" }, snapshot.Enrolled);
        Assert.Equal(new[] { "carol" }, snapshot.Waitlist);
    }

    [Fact]
    public void Enroll_WhenSeatIsOpenAndWaiterIsEligible_DoesNotJumpTheQueue()
    {
        // Seat opens while the only waiter (bob) is ineligible, so it stays open.
        // Then bob becomes eligible again. A new learner must not take the seat first.
        var lookup = new FakeLookup();
        lookup.Complete("alice", "CS101");
        lookup.Complete("bob", "CS101");
        lookup.Complete("dave", "CS101");
        var service = NewCourseWithPrerequisite(1, lookup);
        service.Enroll("alice");
        service.Enroll("bob");

        lookup.Revoke("bob", "CS101");
        service.Drop("alice");              // seat stays open, bob skipped
        lookup.Complete("bob", "CS101");    // bob is eligible again

        Assert.Equal(EnrollStatus.Waitlisted, service.Enroll("dave"));

        var snapshot = service.GetSnapshot();
        Assert.Equal(new[] { "bob" }, snapshot.Enrolled);
        Assert.Equal(new[] { "dave" }, snapshot.Waitlist);
    }

    [Fact]
    public async Task ParallelEnrolls_NeverExceedCapacityOrDuplicateAsync()
    {
        const int capacity = 5;
        const int learners = 40;
        var service = NewOpenCourse(capacity);
        using var startGate = new ManualResetEventSlim(false);
        var tasks = new List<Task>();

        // 40 different learners, plus the same learner "same" from 10 threads.
        for (int i = 0; i < learners; i++)
        {
            string id = "learner" + i;
            tasks.Add(Task.Run(() => { startGate.Wait(); service.Enroll(id); }));
        }
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() => { startGate.Wait(); service.Enroll("same"); }));
        }

        startGate.Set();
        await Task.WhenAll(tasks);

        var snapshot = service.GetSnapshot();
        var everyone = new List<string>(snapshot.Enrolled);
        everyone.AddRange(snapshot.Waitlist);

        Assert.Equal(capacity, snapshot.Enrolled.Count);
        Assert.Equal(learners + 1, snapshot.Waitlist.Count + snapshot.Enrolled.Count);
        Assert.Equal(everyone.Count, everyone.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}