using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

public static class MockLoggerExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, Times times) where T : class
    {
        loggerMock.Verify(m => m.Log(
            level,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times);
    }

    public static Mock<ILogger<T>> VerifyLogMatch<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times, string pattern)
    {
        Func<object, Type, bool> test = (value, _) => value != null && Regex.IsMatch(value.ToString() ?? "", pattern);

        logger.Verify(m => m.Log(
            level,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => test(v, t)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times);

        return logger;
    }
}