using App.Shared.Domain;

namespace App.Shared.Tests.Domain;

public sealed class AggregateRootTests
{
    private sealed record SampleEvent : EventInterface;

    private sealed class SampleAggregate : AggregateRoot
    {
        public void Emit() => Raise(new SampleEvent());

        public void EmitTwice()
        {
            Raise(new SampleEvent());
            Raise(new SampleEvent());
        }
    }

    [Fact]
    public void raise_adds_pending_events_in_order()
    {
        // Arrange
        var aggregate = new SampleAggregate();

        // Act
        aggregate.EmitTwice();

        // Assert
        Assert.Equal(2, aggregate.PendingEvents.Count);
        Assert.All(aggregate.PendingEvents, e => Assert.IsType<SampleEvent>(e));
    }

    [Fact]
    public void clearPendingEvents_removes_all_pending()
    {
        // Arrange
        var aggregate = new SampleAggregate();
        aggregate.Emit();

        // Act
        aggregate.ClearPendingEvents();

        // Assert
        Assert.Empty(aggregate.PendingEvents);
    }
}
