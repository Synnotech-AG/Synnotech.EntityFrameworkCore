using FluentAssertions;
using Synnotech.DatabaseAbstractions;
using Xunit;

namespace Synnotech.EntityFrameworkCore.Tests
{
    public static class AsyncTransactionalSessionTests
    {
        [Fact]
        public static void MustImplementIAsyncTransactionalSession() =>
            typeof(AsyncTransactionalSession<>).Should().Implement<IAsyncTransactionalSession>();

        [Fact]
        public static void MustDeriveFromAsyncSession() =>
            typeof(AsyncTransactionalSession<>).Should().BeDerivedFrom(typeof(AsyncSession<>));
    }
}