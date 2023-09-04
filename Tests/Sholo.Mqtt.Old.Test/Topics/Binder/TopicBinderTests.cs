/*
using System;
using Sholo.Mqtt.Test.Helpers;
using Sholo.Mqtt.Topics.BinderFactoryBuilder;
using Xunit;

namespace Sholo.Mqtt.Test.Topics.Binder
{
    public class TopicBinderTests
    {
        [Fact]
        public void TopicBinderBuilder_WithAutoPropertyRegistrations_WorksAsExpected()
        {
            var topicBinderFactory = TopicBinderFactoryBuilder.CreateDefault<TestTopicParameters>().BuildFactory();

            var topicBinder = topicBinderFactory.CreateBinder("test/+BoolProperty/+CharProperty/+DecimalProperty/+DoubleProperty/+FloatProperty/+IntProperty/+UintProperty/+LongProperty/+UlongProperty/+ShortProperty/+UshortProperty/+GuidProperty/+StringProperty");

            var guid = Guid.NewGuid();
            var target = topicBinder.Bind($"test/{true}/{'a'}/{0.25M}/{0.123d}/{0.456f}/{int.MinValue}/{uint.MaxValue}/{long.MinValue}/{ulong.MaxValue}/{short.MinValue}/{ushort.MaxValue}/{guid}/testing");

            Assert.True(target.BoolProperty);
            Assert.Equal('a', target.CharProperty);
            Assert.Equal(0.25M, target.DecimalProperty);
            Assert.Equal(0.123d, target.DoubleProperty);
            Assert.Equal(0.456f, target.FloatProperty);
            Assert.Equal(int.MinValue, target.IntProperty);
            Assert.Equal(uint.MaxValue, target.UintProperty);
            Assert.Equal(long.MinValue, target.LongProperty);
            Assert.Equal(ulong.MaxValue, target.UlongProperty);
            Assert.Equal(short.MinValue, target.ShortProperty);
            Assert.Equal(ushort.MaxValue, target.UshortProperty);
            Assert.Equal(guid, target.GuidProperty);
            Assert.Equal("testing", target.StringProperty);
        }

        [Fact]
        public void TopicBinderBuilder_WithExplicitPropertyRegistrations_WorksAsExpected()
        {
            var topicBinderFactory = TopicBinderFactoryBuilder.Create<TestTopicParameters>()
                .WithProperty(t => t.BoolProperty)
                .WithProperty(t => t.CharProperty)
                .WithProperty(t => t.DecimalProperty)
                .WithProperty(t => t.DoubleProperty)
                .WithProperty(t => t.FloatProperty)
                .WithProperty(t => t.IntProperty)
                .WithProperty(t => t.UintProperty)
                .WithProperty(t => t.LongProperty)
                .WithProperty(t => t.UlongProperty)
                .WithProperty(t => t.ShortProperty)
                .WithProperty(t => t.UshortProperty)
                .WithProperty(t => t.GuidProperty)
                .WithProperty(t => t.StringProperty)
                .BuildFactory();

            var topicBinder = topicBinderFactory.CreateBinder("test/+BoolProperty/+CharProperty/+DecimalProperty/+DoubleProperty/+FloatProperty/+IntProperty/+UintProperty/+LongProperty/+UlongProperty/+ShortProperty/+UshortProperty/+GuidProperty/+StringProperty");

            var guid = Guid.NewGuid();
            var target = topicBinder.Bind($"test/{true}/{'a'}/{0.25M}/{0.123d}/{0.456f}/{int.MinValue}/{uint.MaxValue}/{long.MinValue}/{ulong.MaxValue}/{short.MinValue}/{ushort.MaxValue}/{guid}/testing");

            Assert.True(target.BoolProperty);
            Assert.Equal('a', target.CharProperty);
            Assert.Equal(0.25M, target.DecimalProperty);
            Assert.Equal(0.123d, target.DoubleProperty);
            Assert.Equal(0.456f, target.FloatProperty);
            Assert.Equal(int.MinValue, target.IntProperty);
            Assert.Equal(uint.MaxValue, target.UintProperty);
            Assert.Equal(long.MinValue, target.LongProperty);
            Assert.Equal(ulong.MaxValue, target.UlongProperty);
            Assert.Equal(short.MinValue, target.ShortProperty);
            Assert.Equal(ushort.MaxValue, target.UshortProperty);
            Assert.Equal(guid, target.GuidProperty);
            Assert.Equal("testing", target.StringProperty);
        }
    }
}
*/
