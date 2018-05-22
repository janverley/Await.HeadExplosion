#if NETCOREAPP2_1
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

sealed class CustomLongTaskSource : IValueTaskSource<long>
{
    private readonly int every;
    private long counter;

    public CustomLongTaskSource(int every)
    {
        this.every = every;
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        var increment = Interlocked.Increment(ref counter);
        if (increment % every == 0)
        {
            Console.WriteLine("Scheduling");
            return ValueTaskSourceStatus.Pending;
        }
        Console.Write("Cached | ");
        return ValueTaskSourceStatus.Succeeded;
    }

    public void OnCompleted(Action<object> continuation, object state, short token,
        ValueTaskSourceOnCompletedFlags flags)
    {
        Task.Delay(1000)
            .ConfigureAwait(flags == ValueTaskSourceOnCompletedFlags.UseSchedulingContext)
            .GetAwaiter()
            .OnCompleted(() => continuation(state));
    }

    public long GetResult(short token)
    {
        return counter;
    }
}
#endif