using CourseWaitlist.Core;

namespace CourseWaitlist.Tests;

// In-memory stand-in for the completions lookup. Locked so parallel tests are safe.
public class FakeLookup : ICompletedCourseLookup
{
    private readonly object _lock = new();
    private readonly HashSet<string> _completed = new(StringComparer.OrdinalIgnoreCase);

    public void Complete(string learnerId, string courseId)
    {
        lock (_lock) { _completed.Add(learnerId + "|" + courseId); }
    }

    public void Revoke(string learnerId, string courseId)
    {
        lock (_lock) { _completed.Remove(learnerId + "|" + courseId); }
    }

    public bool HasCompleted(string learnerId, string courseId)
    {
        lock (_lock) { return _completed.Contains(learnerId + "|" + courseId); }
    }
}