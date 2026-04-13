using System.Diagnostics.CodeAnalysis;

using App.Shared;

namespace App.Shared.Application;

/// <summary>
/// Discriminated success/failure value. <typeparamref name="TValue"/> is the success payload;
/// <typeparamref name="TError"/> is the failure payload (for example a capability failure enum).
/// </summary>
public readonly struct Result<TValue, TError> : IEquatable<Result<TValue, TError>>
{
    private readonly bool _isSuccess;
    private readonly TValue _value;
    private readonly TError _error;

    private Result(
        bool isSuccess,
        TValue value,
        TError error)
    {
        _isSuccess = isSuccess;
        _value = value;
        _error = error;
    }

    public bool IsSuccess => _isSuccess;

    public bool IsFailure => !_isSuccess;

    public static Result<TValue, TError> Success(TValue value) =>
        new Result<TValue, TError>(
            isSuccess: true,
            value: value,
            error: default!);

    public static Result<TValue, TError> Failure(TError error) =>
        new Result<TValue, TError>(
            isSuccess: false,
            value: default!,
            error: error);

    public TValue GetValue()
    {
        if (!_isSuccess)
        {
            throw BaseException.WithMessage("Cannot read Value from a failed Result.");
        }

        return _value;
    }

    public TError GetError()
    {
        if (_isSuccess)
        {
            throw BaseException.WithMessage("Cannot read Error from a successful Result.");
        }

        return _error;
    }

    public bool TryGetValue([NotNullWhen(true)] out TValue? value)
    {
        if (_isSuccess)
        {
            value = _value!;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetError([NotNullWhen(true)] out TError? error)
    {
        if (!_isSuccess)
        {
            error = _error!;
            return true;
        }

        error = default;
        return false;
    }

    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<TError, TResult> onFailure)
    {
        if (_isSuccess)
        {
            return onSuccess(_value);
        }

        return onFailure(_error);
    }

    public Result<TNext, TError> Map<TNext>(Func<TValue, TNext> map)
    {
        if (!_isSuccess)
        {
            return Result<TNext, TError>.Failure(_error);
        }

        return Result<TNext, TError>.Success(map(_value));
    }

    public Result<TNext, TError> Bind<TNext>(Func<TValue, Result<TNext, TError>> bind)
    {
        if (!_isSuccess)
        {
            return Result<TNext, TError>.Failure(_error);
        }

        return bind(_value);
    }

    public bool Equals(Result<TValue, TError> other)
    {
        if (_isSuccess != other._isSuccess)
        {
            return false;
        }

        if (_isSuccess)
        {
            return EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }

        return EqualityComparer<TError>.Default.Equals(_error, other._error);
    }

    public override bool Equals(object? obj) =>
        obj is Result<TValue, TError> other && Equals(other);

    public override int GetHashCode()
    {
        if (_isSuccess)
        {
            return HashCode.Combine(true, _value);
        }

        return HashCode.Combine(false, _error);
    }

    public static bool operator ==(Result<TValue, TError> left, Result<TValue, TError> right) =>
        left.Equals(right);

    public static bool operator !=(Result<TValue, TError> left, Result<TValue, TError> right) =>
        !left.Equals(right);
}
