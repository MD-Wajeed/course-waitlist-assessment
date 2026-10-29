namespace CourseWaitlist.Core;

public class CourseEnrollmentService
{
    private static readonly IReadOnlyList<string> NoIds = Array.Empty<string>();

    private readonly object _lock = new();
    private readonly int _capacity;
    private readonly List<string> _requiredCourseIds = new();
    private readonly ICompletedCourseLookup _lookup;

    private readonly List<string> _enrolled = new();
    private readonly List<string> _waitlist = new(); // front = earliest

    public CourseEnrollmentService(
        int capacity,
        IEnumerable<string> requiredCourseIds,
        ICompletedCourseLookup lookup)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");
        ArgumentNullException.ThrowIfNull(requiredCourseIds);
        ArgumentNullException.ThrowIfNull(lookup);

        foreach (string courseId in requiredCourseIds)
        {
            if (string.IsNullOrWhiteSpace(courseId))
                throw new ArgumentException("Required course ids must not be blank.", nameof(requiredCourseIds));

            string trimmed = courseId.Trim();
            if (!Contains(_requiredCourseIds, trimmed))
                _requiredCourseIds.Add(trimmed);
        }

        _capacity = capacity;
        _lookup = lookup;
    }

    public EnrollStatus Enroll(string? learnerId)
    {
        if (string.IsNullOrWhiteSpace(learnerId))
            return EnrollStatus.InvalidRequest;

        string id = learnerId.Trim();

        lock (_lock)
        {
            if (Contains(_enrolled, id)) return EnrollStatus.AlreadyEnrolled;
            if (Contains(_waitlist, id)) return EnrollStatus.AlreadyWaitlisted;
            if (!IsEligible(id)) return EnrollStatus.NotEligible;

            // Fill open seats from the queue first so a new learner cannot jump ahead.
            PromoteWaiters();

            if (_enrolled.Count < _capacity)
            {
                _enrolled.Add(id);
                return EnrollStatus.Enrolled;
            }

            _waitlist.Add(id);
            return EnrollStatus.Waitlisted;
        }
    }

    public DropResult Drop(string? learnerId)
    {
        if (string.IsNullOrWhiteSpace(learnerId))
            return new DropResult(DropStatus.InvalidRequest, NoIds);

        string id = learnerId.Trim();

        lock (_lock)
        {
            // A waiter leaving frees no seat, so nobody is promoted.
            if (Remove(_waitlist, id))
                return new DropResult(DropStatus.RemovedFromWaitlist, NoIds);

            if (Remove(_enrolled, id))
            {
                List<string> promoted = PromoteWaiters();
                return new DropResult(DropStatus.Dropped, promoted);
            }

            return new DropResult(DropStatus.NotFound, NoIds);
        }
    }

    public EnrollmentSnapshot GetSnapshot()
    {
        lock (_lock)
        {
            return new EnrollmentSnapshot(
                new List<string>(_enrolled).AsReadOnly(),
                new List<string>(_waitlist).AsReadOnly());
        }
    }

    // Must be called inside the lock.
    private List<string> PromoteWaiters()
    {
        var promoted = new List<string>();
        int position = 0;

        while (_enrolled.Count < _capacity && position < _waitlist.Count)
        {
            string waiter = _waitlist[position];

            if (IsEligible(waiter))
            {
                _waitlist.RemoveAt(position);
                _enrolled.Add(waiter);
                promoted.Add(waiter);
            }
            else
            {
                position++; // skipped, but keeps their place in the queue
            }
        }

        return promoted;
    }

    private bool IsEligible(string learnerId)
    {
        foreach (string courseId in _requiredCourseIds)
        {
            if (!_lookup.HasCompleted(learnerId, courseId))
                return false;
        }
        return true;
    }

    private static bool Contains(List<string> ids, string id)
    {
        return ids.Contains(id, StringComparer.OrdinalIgnoreCase);
    }

    private static bool Remove(List<string> ids, string id)
    {
        int index = ids.FindIndex(x => string.Equals(x, id, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;

        ids.RemoveAt(index);
        return true;
    }
}