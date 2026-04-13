using App.Shared.Common;

namespace App.Shared.Tests;

public sealed class AppDateTimeTests : IDisposable
{
    private static readonly TimeZoneInfo Berlin = TimeZoneInfo.FindSystemTimeZoneById(id: "Europe/Berlin");
    private static readonly TimeZoneInfo NewYork = TimeZoneInfo.FindSystemTimeZoneById(id: "America/New_York");

    public void Dispose()
    {
        AppDateTime.RestoreTime();
    }

    [Fact]
    public void freeze_time_utc_throws_when_kind_is_not_utc()
    {
        // Arrange
        var local = new DateTime(
            year: 2025,
            month: 1,
            day: 15,
            hour: 10,
            minute: 30,
            second: 0,
            kind: DateTimeKind.Local);

        // Act
        var act = () => AppDateTime.FreezeTimeUtc(utcInstant: local);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void freeze_time_returns_fixed_instant_for_get_utc_now()
    {
        // Arrange
        var frozenUtc = new DateTime(
            year: 2025,
            month: 1,
            day: 15,
            hour: 10,
            minute: 30,
            second: 0,
            kind: DateTimeKind.Utc);
        AppDateTime.FreezeTimeUtc(utcInstant: frozenUtc);

        // Act
        var utcNow = AppDateTime.GetUtcNow();

        // Assert
        Assert.Equal(expected: frozenUtc, actual: utcNow);
    }

    [Fact]
    public void freeze_time_preserves_wall_clock_in_frozen_zone()
    {
        // Arrange
        var wallBerlin = new DateTime(
            year: 2025,
            month: 6,
            day: 15,
            hour: 14,
            minute: 0,
            second: 0,
            kind: DateTimeKind.Unspecified);
        AppDateTime.FreezeTime(
            dateTime: wallBerlin,
            timeZone: Berlin);

        // Act
        var utc = AppDateTime.GetUtcNow();
        var roundTripBerlin = TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: utc,
            destinationTimeZone: Berlin);

        // Assert
        Assert.Equal(expected: 14, actual: roundTripBerlin.Hour);
        Assert.Equal(expected: 0, actual: roundTripBerlin.Minute);
    }

    [Fact]
    public void restore_time_allows_normal_behavior()
    {
        // Arrange
        AppDateTime.FreezeTimeUtc(
            utcInstant: new DateTime(
                year: 2025,
                month: 1,
                day: 15,
                hour: 10,
                minute: 30,
                second: 0,
                kind: DateTimeKind.Utc));
        AppDateTime.RestoreTime();

        // Act
        var before = DateTime.UtcNow;
        var subject = AppDateTime.GetUtcNow();
        var after = DateTime.UtcNow;

        // Assert
        Assert.True(condition: subject >= before);
        Assert.True(condition: subject <= after);
    }

    [Fact]
    public void is_frozen_returns_true_when_time_frozen()
    {
        // Arrange
        AppDateTime.FreezeTimeUtc(
            utcInstant: new DateTime(
                year: 2025,
                month: 1,
                day: 15,
                hour: 10,
                minute: 30,
                second: 0,
                kind: DateTimeKind.Utc));

        // Act
        var frozen = AppDateTime.IsFrozen;

        // Assert
        Assert.True(frozen);
    }

    [Fact]
    public void is_frozen_returns_false_when_time_not_frozen()
    {
        // Assert
        Assert.False(AppDateTime.IsFrozen);
    }

    [Fact]
    public void is_frozen_returns_false_after_restore()
    {
        // Arrange
        AppDateTime.FreezeTimeUtc(
            utcInstant: new DateTime(
                year: 2025,
                month: 1,
                day: 15,
                hour: 10,
                minute: 30,
                second: 0,
                kind: DateTimeKind.Utc));

        // Act
        AppDateTime.RestoreTime();

        // Assert
        Assert.False(AppDateTime.IsFrozen);
    }

    [Fact]
    public void explicit_time_zone_overrides_frozen_zone_for_wall_clock_digits()
    {
        // Arrange
        var wallBerlin = new DateTime(
            year: 2025,
            month: 1,
            day: 15,
            hour: 10,
            minute: 30,
            second: 0,
            kind: DateTimeKind.Unspecified);
        AppDateTime.FreezeTime(
            dateTime: wallBerlin,
            timeZone: Berlin);

        // Act
        var utc = AppDateTime.GetUtcNow(wallClockZone: NewYork);
        var wallNewYork = TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: utc,
            destinationTimeZone: NewYork);

        // Assert — same wall-clock digits as in Berlin, but labeled America/New_York (PHP test semantics)
        Assert.Equal(expected: 10, actual: wallNewYork.Hour);
        Assert.Equal(expected: 30, actual: wallNewYork.Minute);
    }
}
