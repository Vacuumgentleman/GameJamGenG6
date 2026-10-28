using System;

namespace GameJam
{
    public interface ILevelGoal
    {
        bool IsCompleted { get; }

        event Action Completed;
    }
}
