using App.Shared;
using App.Shared.Application;

namespace App.Shared.Tests;

public sealed class ResultTests
{
    private enum SampleFailure
    {
        NotFound,
        BadInput,
    }

    [Fact]
    public void success_holds_value_and_reports_success()
    {
        // Arrange
        var sut = Result<int, SampleFailure>.Success(7);

        // Act
        var isSuccess = sut.IsSuccess;
        var value = sut.GetValue();

        // Assert
        Assert.True(isSuccess);
        Assert.False(sut.IsFailure);
        Assert.Equal(7, value);
    }

    [Fact]
    public void failure_holds_error_and_reports_failure()
    {
        // Arrange
        var sut = Result<int, SampleFailure>.Failure(SampleFailure.NotFound);

        // Act
        var isFailure = sut.IsFailure;
        var error = sut.GetError();

        // Assert
        Assert.True(isFailure);
        Assert.False(sut.IsSuccess);
        Assert.Equal(SampleFailure.NotFound, error);
    }

    [Fact]
    public void get_value_on_failure_throws()
    {
        // Arrange
        var sut = Result<int, string>.Failure("x");

        // Act
        Action act = () => _ = sut.GetValue();

        // Assert
        Assert.Throws<BaseException>(act);
    }

    [Fact]
    public void get_error_on_success_throws()
    {
        // Arrange
        var sut = Result<int, string>.Success(1);

        // Act
        Action act = () => _ = sut.GetError();

        // Assert
        Assert.Throws<BaseException>(act);
    }

    [Fact]
    public void try_get_value_returns_true_when_success()
    {
        // Arrange
        var sut = Result<int, string>.Success(3);

        // Act
        var ok = sut.TryGetValue(out var value);

        // Assert
        Assert.True(ok);
        Assert.Equal(3, value);
    }

    [Fact]
    public void try_get_value_returns_false_when_failure()
    {
        // Arrange
        var sut = Result<int, string>.Failure("e");

        // Act
        var ok = sut.TryGetValue(out var value);

        // Assert
        Assert.False(ok);
        Assert.Equal(0, value);
    }

    [Fact]
    public void map_on_success_transforms_value_and_preserves_error_type()
    {
        // Arrange
        var sut = Result<int, string>.Success(2);

        // Act
        var next = sut.Map(x => x * 3);

        // Assert
        Assert.True(next.IsSuccess);
        Assert.Equal(6, next.GetValue());
    }

    [Fact]
    public void map_on_failure_short_circuits()
    {
        // Arrange
        var sut = Result<int, string>.Failure("bad");

        // Act
        var next = sut.Map(x => x * 3);

        // Assert
        Assert.True(next.IsFailure);
        Assert.Equal("bad", next.GetError());
    }

    [Fact]
    public void bind_on_success_chains_and_propagates_failure()
    {
        // Arrange
        var sut = Result<int, string>.Success(4);

        // Act
        var next = sut.Bind(x => Result<string, string>.Success((x + 1).ToString()));

        // Assert
        Assert.True(next.IsSuccess);
        Assert.Equal("5", next.GetValue());
    }

    [Fact]
    public void bind_on_failure_short_circuits()
    {
        // Arrange
        var sut = Result<int, string>.Failure("err");

        // Act
        var next = sut.Bind(x => Result<string, string>.Success(x.ToString()));

        // Assert
        Assert.True(next.IsFailure);
        Assert.Equal("err", next.GetError());
    }

    [Fact]
    public void bind_can_return_failure_from_inner_step()
    {
        // Arrange
        var sut = Result<int, string>.Success(1);

        // Act
        var next = sut.Bind(_ => Result<string, string>.Failure("inner"));

        // Assert
        Assert.True(next.IsFailure);
        Assert.Equal("inner", next.GetError());
    }

    [Fact]
    public void match_invokes_correct_branch()
    {
        // Arrange
        var ok = Result<int, string>.Success(10);
        var fail = Result<int, string>.Failure("nope");

        // Act
        var fromOk = ok.Match(v => v * 2, _ => 0);
        var fromFail = fail.Match(_ => 0, e => e.Length);

        // Assert
        Assert.Equal(20, fromOk);
        Assert.Equal(4, fromFail);
    }

    [Fact]
    public void equality_compares_discriminant_and_payload()
    {
        // Arrange
        var a = Result<int, string>.Success(1);
        var b = Result<int, string>.Success(1);
        var c = Result<int, string>.Success(2);
        var d = Result<int, string>.Failure("e");
        var e = Result<int, string>.Failure("e");

        // Assert
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.NotEqual(a, d);
        Assert.Equal(d, e);
    }
}
