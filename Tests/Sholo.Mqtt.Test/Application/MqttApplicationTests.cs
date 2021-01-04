using System;
using System.Threading.Tasks;
using Moq;
using Sholo.Mqtt.ApplicationBuilder;
using Sholo.Mqtt.Test.Helpers;
using Xunit;

namespace Sholo.Mqtt.Test.Application
{
    public class MqttApplicationTests
    {
        [Theory]
        [AutoMoqData]
        public async Task RequestDelegate_WhenEmpty_DoesNotProcessMqttMessage(
            MqttApplicationBuilder mqttApplicationBuilder,
            Mock<IMqttRequestContext> mockRequestContext
        )
        {
            var mqttApplication = mqttApplicationBuilder.Build();
            var result = await mqttApplication.RequestDelegate.Invoke(mockRequestContext.Object);

            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task RequestDelegate_WhenUseReturnsTrue_ProcessesMqttMessage(
            MqttApplicationBuilder mqttApplicationBuilder
        )
        {
            var mqttRequestContext = new TestMqttRequestContext
            {
                Topic = "test/topic"
            };

            mqttApplicationBuilder.Use(MqttRequestDelegateHelper.HandlesRequest());

            var mqttApplication = mqttApplicationBuilder.Build();
            var result = await mqttApplication.RequestDelegate.Invoke(mqttRequestContext);

            Assert.True(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task RequestDelegate_WhenUseReturnsFalse_DoesNotProcessMqttMessage(
            MqttApplicationBuilder mqttApplicationBuilder
        )
        {
            var mqttRequestContext = new TestMqttRequestContext
            {
                Topic = "test/topic"
            };

            mqttApplicationBuilder.Use(MqttRequestDelegateHelper.DoesNotHandleRequest());

            var mqttApplication = mqttApplicationBuilder.Build();
            var result = await mqttApplication.RequestDelegate.Invoke(mqttRequestContext);

            Assert.False(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task RequestDelegate_WhenContextMatchesTopicInUse_ProcessesMqttMessage(
            MqttApplicationBuilder mqttApplicationBuilder
        )
        {
            var mqttRequestContext = new TestMqttRequestContext
            {
                Topic = "test/topic"
            };

            mqttApplicationBuilder.Use(MqttRequestDelegateHelper.Create(ctx => ctx.Topic.Equals("test/topic")));

            var mqttApplication = mqttApplicationBuilder.Build();
            var result = await mqttApplication.RequestDelegate.Invoke(mqttRequestContext);

            Assert.True(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task RequestDelegate_WithMultipleMappedHandlers_BehavesAsExpected(
            MqttApplicationBuilder mqttApplicationBuilder
        )
        {
            var mqttRequestContext1 = new TestMqttRequestContext { Topic = "test/1" };
            var mqttRequestContext2 = new TestMqttRequestContext { Topic = "test/2" };
            var mqttRequestContext3 = new TestMqttRequestContext { Topic = "test/3" };
            var mqttRequestContext4 = new TestMqttRequestContext { Topic = "test/4" };

            mqttApplicationBuilder.Map("test/1", MqttRequestDelegateHelper.Throws(new Exception("1")));
            mqttApplicationBuilder.Map("test/2", MqttRequestDelegateHelper.Throws(new Exception("2")));
            mqttApplicationBuilder.Map("test/3", MqttRequestDelegateHelper.Throws(new Exception("3")));
            mqttApplicationBuilder.Use(MqttRequestDelegateHelper.HandlesRequest());

            var mqttApplication = mqttApplicationBuilder.Build();

            Assert.Collection(
                mqttApplication.TopicFilters,
                f1 => Assert.Equal("test/1", f1.Topic),
                f2 => Assert.Equal("test/2", f2.Topic),
                f3 => Assert.Equal("test/3", f3.Topic),
                f4 => Assert.Equal("#", f4.Topic));

            var exception1 = await Assert.ThrowsAsync<Exception>(() => mqttApplication.RequestDelegate.Invoke(mqttRequestContext1));
            var exception2 = await Assert.ThrowsAsync<Exception>(() => mqttApplication.RequestDelegate.Invoke(mqttRequestContext2));
            var exception3 = await Assert.ThrowsAsync<Exception>(() => mqttApplication.RequestDelegate.Invoke(mqttRequestContext3));
            var result4 = await mqttApplication.RequestDelegate.Invoke(mqttRequestContext4);

            Assert.Equal("1", exception1.Message);
            Assert.Equal("2", exception2.Message);
            Assert.Equal("3", exception3.Message);

            Assert.True(result4);
        }

        [Theory]
        [AutoMoqData]
        public async Task RequestDelegate_WithMultipleMappedParameterizedHandlers_BehavesAsExpected(
            MqttApplicationBuilder mqttApplicationBuilder
        )
        {
            var mqttRequestContext1 = new TestMqttRequestContext<TestTopicParameters> { Topic = "test/1/one" };
            var mqttRequestContext2 = new TestMqttRequestContext<TestTopicParameters> { Topic = "test/2/two" };
            var mqttRequestContext3 = new TestMqttRequestContext<TestTopicParameters> { Topic = "test/3/three" };
            var mqttRequestContext4 = new TestMqttRequestContext<TestTopicParameters> { Topic = "test/4/four" };

            mqttApplicationBuilder.Map(
                "test/1/+StringProperty",
                MqttRequestDelegateHelper.Create<TestTopicParameters>(
                    ctx =>
                    {
                        Assert.Equal("one", ctx.TopicParameters.StringProperty);
                        return true;
                    }));

            mqttApplicationBuilder.Map(
                "test/2/+StringProperty",
                MqttRequestDelegateHelper.Create<TestTopicParameters>(
                    ctx =>
                    {
                        Assert.Equal("two", ctx.TopicParameters.StringProperty);
                        return true;
                    }));

            mqttApplicationBuilder.Map(
                "test/3/+StringProperty",
                MqttRequestDelegateHelper.Create<TestTopicParameters>(
                    ctx =>
                    {
                        Assert.Equal("three", ctx.TopicParameters.StringProperty);
                        return true;
                    }));

            mqttApplicationBuilder.Use(MqttRequestDelegateHelper.DoesNotHandleRequest());

            var mqttApplication = mqttApplicationBuilder.Build();

            var result1 = await mqttApplication.RequestDelegate.Invoke(mqttRequestContext1);
            var result2 = await mqttApplication.RequestDelegate.Invoke(mqttRequestContext2);
            var result3 = await mqttApplication.RequestDelegate.Invoke(mqttRequestContext3);
            var result4 = await mqttApplication.RequestDelegate.Invoke(mqttRequestContext4);

            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
            Assert.False(result4);
        }

        [Theory]
        [AutoMoqData]
        public void TopicFilters_WithMultipleMappedHandlersAndADefaultMapping_ContainsExpectedFilters(
            MqttApplicationBuilder mqttApplicationBuilder
        )
        {
            mqttApplicationBuilder.Map("test/1", MqttRequestDelegateHelper.Throws(new Exception("1")));
            mqttApplicationBuilder.Map("test/2", MqttRequestDelegateHelper.Throws(new Exception("2")));
            mqttApplicationBuilder.Map("test/3", MqttRequestDelegateHelper.Throws(new Exception("3")));
            mqttApplicationBuilder.Use(MqttRequestDelegateHelper.HandlesRequest());

            var mqttApplication = mqttApplicationBuilder.Build();

            Assert.Collection(
                mqttApplication.TopicFilters,
                f1 => Assert.Equal("test/1", f1.Topic),
                f2 => Assert.Equal("test/2", f2.Topic),
                f3 => Assert.Equal("test/3", f3.Topic),
                f4 => Assert.Equal("#", f4.Topic));
        }

        [Theory]
        [AutoMoqData]
        public void TopicFilters_WithMultipleMappedHandlersAndNoDefaultMapping_ContainsExpectedFilters(
            MqttApplicationBuilder mqttApplicationBuilder
        )
        {
            mqttApplicationBuilder.Map("test/1", MqttRequestDelegateHelper.Throws(new Exception("1")));
            mqttApplicationBuilder.Map("test/2", MqttRequestDelegateHelper.Throws(new Exception("2")));
            mqttApplicationBuilder.Map("test/3", MqttRequestDelegateHelper.Throws(new Exception("3")));

            var mqttApplication = mqttApplicationBuilder.Build();

            Assert.Collection(
                mqttApplication.TopicFilters,
                f1 => Assert.Equal("test/1", f1.Topic),
                f2 => Assert.Equal("test/2", f2.Topic),
                f3 => Assert.Equal("test/3", f3.Topic));
        }
    }
}
