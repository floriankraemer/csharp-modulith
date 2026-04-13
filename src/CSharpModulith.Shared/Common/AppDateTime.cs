namespace App.Shared.Common;

/// <summary>
/// Testable clock, mirroring freeze/restore semantics of <c>App\Shared\Generic\DateTime\DateTime</c> in the PHP backend.
/// </summary>
public static class AppDateTime
{
    private sealed record FrozenClock(DateTime UtcInstant, TimeZoneInfo Zone);

    private static FrozenClock? _frozen;

    public static bool IsFrozen => _frozen is not null;

    /// <summary>
    /// Freezes the clock using a wall-clock value interpreted in <paramref name="timeZone"/> (PHP <c>DateTime</c> semantics).
    /// </summary>
    public static void FreezeTime(
        DateTime dateTime,
        TimeZoneInfo timeZone)
    {
        var utcInstant = dateTime.Kind == DateTimeKind.Utc
            ? dateTime
            : TimeZoneInfo.ConvertTimeToUtc(
                dateTime: DateTime.SpecifyKind(
                    value: dateTime,
                    kind: DateTimeKind.Unspecified),
                sourceTimeZone: timeZone);

        _frozen = new FrozenClock(
            UtcInstant: utcInstant,
            Zone: timeZone);
    }

    /// <summary>
    /// Freezes the clock to a UTC instant; wall-clock digits for <see cref="GetUtcNow"/> with an explicit zone use UTC.
    /// </summary>
    public static void FreezeTimeUtc(DateTime utcInstant)
    {
        if (utcInstant.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException(
                message: "Kind must be Utc.",
                paramName: nameof(utcInstant));
        }

        _frozen = new FrozenClock(
            UtcInstant: utcInstant,
            Zone: TimeZoneInfo.Utc);
    }

    public static void RestoreTime()
    {
        _frozen = null;
    }

    /// <summary>
    /// Current UTC instant. When time is frozen and <paramref name="wallClockZone"/> is <see langword="null"/>,
    /// returns the frozen instant. When time is frozen and <paramref name="wallClockZone"/> is set, copies the
    /// frozen wall-clock from the frozen zone and interprets those digits in <paramref name="wallClockZone"/>,
    /// matching PHP <c>new DateTime('now', $timezone)</c> with a frozen baseline.
    /// </summary>
    public static DateTime GetUtcNow(TimeZoneInfo? wallClockZone = null)
    {
        if (_frozen is null)
        {
            return DateTime.UtcNow;
        }

        if (wallClockZone is null)
        {
            return _frozen.UtcInstant;
        }

        var wallInFrozenZone = TimeZoneInfo.ConvertTimeFromUtc(
            dateTime: _frozen.UtcInstant,
            destinationTimeZone: _frozen.Zone);

        var sameWallDigits = new DateTime(
            year: wallInFrozenZone.Year,
            month: wallInFrozenZone.Month,
            day: wallInFrozenZone.Day,
            hour: wallInFrozenZone.Hour,
            minute: wallInFrozenZone.Minute,
            second: wallInFrozenZone.Second,
            millisecond: wallInFrozenZone.Millisecond,
            kind: DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(
            dateTime: sameWallDigits,
            sourceTimeZone: wallClockZone);
    }
}
