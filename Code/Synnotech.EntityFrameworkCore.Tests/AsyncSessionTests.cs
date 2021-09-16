using FluentAssertions;
using Synnotech.DatabaseAbstractions;
using Xunit;

namespace Synnotech.EntityFrameworkCore.Tests
{
    public static class AsyncSessionTests
    {
        [Fact]
        public static void MustImplementIAsyncSession() =>
            typeof(AsyncSession<>).Should().Implement<IAsyncSession>();
    }
}