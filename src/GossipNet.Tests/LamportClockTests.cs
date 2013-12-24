using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace GossipNet
{
    public class LamportClockTests
    {
        [Fact]
        public void Current_should_be_0_after_construction()
        {
            var subject = new LamportClock();

            subject.Current.Should().Be(0);
        }

        [Fact]
        public void Current_should_be_incremented_after_increment()
        {
            var subject = new LamportClock();

            var current = subject.Current;
            var result = subject.Increment();

            result.Should().Be(current + 1);
            subject.Current.Should().Be(result);
        }

        [Fact]
        public void Witness_should_return_value_plus_1()
        {
            var subject = new LamportClock();

            var result = subject.Witness(20);

            result.Should().Be(21);
            subject.Current.Should().Be(result);
        }

    }
}