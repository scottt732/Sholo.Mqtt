using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sholo.Mqtt.ModelBinding.TypeConverters;
using Sholo.Mqtt.Test.Specimens;
using Xunit;

namespace Sholo.Mqtt.Test.TypeConverters;

public class DefaultTypeConverterTests
{
    [Fact]
    public void TryConvert_FromValidStringToString_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (string?)null);
        TestTryConvertStringHappyPath(string.Empty, (string?)string.Empty);
        TestTryConvertStringHappyPath("test", (string?)"test");
    }

    [Fact]
    public void TryConvert_FromStringToNonDefaultType_ReturnsFalseWithNullResult()
    {
        TestTryConvertStringSadPath<IPAddress>(null);
        TestTryConvertStringSadPath<IPAddress>(string.Empty);
        TestTryConvertStringSadPath<IPAddress>("127.0.0.1");

        TestTryConvertStringSadPath<IPAddress?>(null);
        TestTryConvertStringSadPath<IPAddress?>(string.Empty);
        TestTryConvertStringSadPath<IPAddress?>("127.0.0.1");
    }

    [Fact]
    public void TryConvert_FromValidStringToEnum_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("Unknown", TestLightState.Unknown);
        TestTryConvertStringHappyPath("On", TestLightState.On);
        TestTryConvertStringHappyPath("Off", TestLightState.Off);
        TestTryConvertStringHappyPath("Unavailable", TestLightState.Unavailable);

        TestTryConvertStringHappyPath("0", TestLightState.Unknown);
        TestTryConvertStringHappyPath("1", TestLightState.On);
        TestTryConvertStringHappyPath("2", TestLightState.Off);
        TestTryConvertStringHappyPath("3", TestLightState.Unavailable);

        // Enums don't guarantee that values are in range. Leave that responsibility to validation
        TestTryConvertStringHappyPath("123", (TestLightState)123);
    }

    [Fact]
    public void TryConvert_FromValidStringToByte_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("0", 0x00);
        TestTryConvertStringHappyPath("255", 0xff);
    }

    [Fact]
    public void TryConvert_FromValidStringToBoolean_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("true", true);
        TestTryConvertStringHappyPath("false", false);
        TestTryConvertStringHappyPath("TrUe", true);
        TestTryConvertStringHappyPath("FaLse", false);
    }

    [Fact]
    public void TryConvert_FromValidStringToChar_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(" ", ' ');
        TestTryConvertStringHappyPath("a", 'a');
    }

    [Fact]
    public void TryConvert_FromValidStringToDecimal_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("12.25", 12.25M);
    }

    [Fact]
    public void TryConvert_FromValidStringToDouble_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(
            "123.456789",
            123.456789,
            (d1, d2) => Math.Abs(d1 - d2) < double.Epsilon
        );
    }

    [Fact]
    public void TryConvert_FromValidStringToFloat_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(
            "123.456789",
            123.456789f,
            (f1, f2) => Math.Abs(f1 - f2) < float.Epsilon
        );
    }

    [Fact]
    public void TryConvert_FromValidStringToInt_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("-1", -1);
        TestTryConvertStringHappyPath("0", 0);
        TestTryConvertStringHappyPath("1", 1);
    }

    [Fact]
    public void TryConvert_FromValidStringToUnsignedInt_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("0", 0U);
        TestTryConvertStringHappyPath("1", 1U);
    }

    [Fact]
    public void TryConvert_FromValidStringToLong_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("-1", -1L);
        TestTryConvertStringHappyPath("0", 0L);
        TestTryConvertStringHappyPath("1", 1L);
    }

    [Fact]
    public void TryConvert_FromValidStringToUnsignedLong_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("0", 0UL);
        TestTryConvertStringHappyPath("1", 1UL);
    }

    [Fact]
    public void TryConvert_FromValidStringToShort_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("-1", (short)-1);
        TestTryConvertStringHappyPath("0", (short)0);
        TestTryConvertStringHappyPath("1", (short)1);
    }

    [Fact]
    public void TryConvert_FromValidStringToUnsignedShort_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("0", (ushort)0);
        TestTryConvertStringHappyPath("1", (ushort)1);
    }

    [Fact]
    public void TryConvert_FromValidStringToGuid_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath("85cac371-d447-4f2f-9847-dec30712a3b3", Guid.Parse("85cac371-d447-4f2f-9847-dec30712a3b3"));
        TestTryConvertStringHappyPath("{85cac371-d447-4f2f-9847-dec30712a3b3}", Guid.Parse("85cac371-d447-4f2f-9847-dec30712a3b3"));
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableEnum_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (TestLightState?)null);

        TestTryConvertStringHappyPath("Unknown", (TestLightState?)TestLightState.Unknown);
        TestTryConvertStringHappyPath("On", (TestLightState?)TestLightState.On);
        TestTryConvertStringHappyPath("Off", (TestLightState?)TestLightState.Off);
        TestTryConvertStringHappyPath("Unavailable", (TestLightState?)TestLightState.Unavailable);

        TestTryConvertStringHappyPath("0", (TestLightState?)TestLightState.Unknown);
        TestTryConvertStringHappyPath("1", (TestLightState?)TestLightState.On);
        TestTryConvertStringHappyPath("2", (TestLightState?)TestLightState.Off);
        TestTryConvertStringHappyPath("3", (TestLightState?)TestLightState.Unavailable);

        // Enums don't guarantee that values are in range. Leave that responsibility to validation
        TestTryConvertStringHappyPath("123", (TestLightState?)123);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableByte_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (byte?)null);
        TestTryConvertStringHappyPath("0", 0x00);
        TestTryConvertStringHappyPath("255", 0xff);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableBoolean_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (bool?)null);
        TestTryConvertStringHappyPath("true", (bool?)true);
        TestTryConvertStringHappyPath("false", (bool?)false);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableChar_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (char?)null);
        TestTryConvertStringHappyPath(" ", (char?)' ');
        TestTryConvertStringHappyPath("a", (char?)'a');
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableDecimal_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (decimal?)null);
        TestTryConvertStringHappyPath("12.25", (decimal?)12.25M);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableDouble_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(
            null,
            (double?)null,
            (d1, d2) => (!d1.HasValue && !d2.HasValue) || (d1.HasValue && d2.HasValue && Math.Abs(d1.Value - d2.Value) < double.Epsilon)
        );
        TestTryConvertStringHappyPath(
            "123.456789",
            (double?)123.456789,
            (d1, d2) => (!d1.HasValue && !d2.HasValue) || (d1.HasValue && d2.HasValue && Math.Abs(d1.Value - d2.Value) < double.Epsilon)
        );
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableFloat_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(
            null,
            (float?)null,
            (f1, f2) => (!f1.HasValue && !f2.HasValue) || (f1.HasValue && f2.HasValue && Math.Abs(f1.Value - f2.Value) < float.Epsilon)
        );
        TestTryConvertStringHappyPath(
            "-123.456789",
            (float?)-123.456789f,
            (f1, f2) => (!f1.HasValue && !f2.HasValue) || (f1.HasValue && f2.HasValue && Math.Abs(f1.Value - f2.Value) < float.Epsilon)
        );
        TestTryConvertStringHappyPath(
            "123.456789",
            (float?)123.456789f,
            (f1, f2) => (!f1.HasValue && !f2.HasValue) || (f1.HasValue && f2.HasValue && Math.Abs(f1.Value - f2.Value) < float.Epsilon)
        );
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableInt_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (int?)null);
        TestTryConvertStringHappyPath("-1", (int?)-1);
        TestTryConvertStringHappyPath("0", (int?)0);
        TestTryConvertStringHappyPath("1", (int?)1);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableUnsignedInt_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (uint?)null);
        TestTryConvertStringHappyPath("0", (uint?)0);
        TestTryConvertStringHappyPath("1", (uint?)1);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableLong_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (long?)null);
        TestTryConvertStringHappyPath("-1", -1L);
        TestTryConvertStringHappyPath("0", 0L);
        TestTryConvertStringHappyPath("1", 1L);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableUnsignedLong_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (ulong?)null);
        TestTryConvertStringHappyPath("0", (ulong?)0);
        TestTryConvertStringHappyPath("1", (ulong?)1);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableShort_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (short?)null);
        TestTryConvertStringHappyPath("-1", (short?)-1);
        TestTryConvertStringHappyPath("0", (short?)0);
        TestTryConvertStringHappyPath("1", (short?)1);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableUnsignedShort_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (ushort?)null);
        TestTryConvertStringHappyPath("0", (ushort?)0);
        TestTryConvertStringHappyPath("1", (ushort?)1);
    }

    [Fact]
    public void TryConvert_FromValidStringToNullableGuid_ReturnsExpectedResults()
    {
        TestTryConvertStringHappyPath(null, (Guid?)null);
        TestTryConvertStringHappyPath("85cac371-d447-4f2f-9847-dec30712a3b3", (Guid?)Guid.Parse("85cac371-d447-4f2f-9847-dec30712a3b3"));
        TestTryConvertStringHappyPath("{85cac371-d447-4f2f-9847-dec30712a3b3}", (Guid?)Guid.Parse("85cac371-d447-4f2f-9847-dec30712a3b3"));
    }

    // Sad Path
    [Fact]
    public void TryConvert_FromStringToNonEnum_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<TestLight>(null);
        TestTryConvertStringSadPath<TestLight>(string.Empty);
        TestTryConvertStringSadPath<TestLight>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToByte_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<byte>(null);
        TestTryConvertStringSadPath<byte>(string.Empty);
        TestTryConvertStringSadPath<byte>("-1");
        TestTryConvertStringSadPath<byte>("256");
        TestTryConvertStringSadPath<byte>("a");
        TestTryConvertStringSadPath<byte>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToBoolean_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<bool>(null);
        TestTryConvertStringSadPath<bool>(string.Empty);
        TestTryConvertStringSadPath<bool>("1");
        TestTryConvertStringSadPath<bool>("0");
        TestTryConvertStringSadPath<bool>("y");
        TestTryConvertStringSadPath<bool>("n");
        TestTryConvertStringSadPath<bool>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToChar_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<char>(null);
        TestTryConvertStringSadPath<char>(string.Empty);
        TestTryConvertStringSadPath<char>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToDecimal_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<decimal>(null);
        TestTryConvertStringSadPath<decimal>(string.Empty);
        TestTryConvertStringSadPath<decimal>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToDouble_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<double>(null);
        TestTryConvertStringSadPath<double>(string.Empty);
        TestTryConvertStringSadPath<double>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToFloat_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<float>(null);
        TestTryConvertStringSadPath<float>(string.Empty);
        TestTryConvertStringSadPath<float>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToInt_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<int>(null);
        TestTryConvertStringSadPath<int>(string.Empty);
        TestTryConvertStringSadPath<int>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToUnsignedInt_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<uint>(null);
        TestTryConvertStringSadPath<uint>(string.Empty);
        TestTryConvertStringSadPath<uint>("-1");
        TestTryConvertStringSadPath<uint>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToLong_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<long>(null);
        TestTryConvertStringSadPath<long>(string.Empty);
        TestTryConvertStringSadPath<long>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToUnsignedLong_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<ulong>(null);
        TestTryConvertStringSadPath<ulong>(string.Empty);
        TestTryConvertStringSadPath<ulong>("-1");
        TestTryConvertStringSadPath<ulong>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToShort_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<short>(null);
        TestTryConvertStringSadPath<short>(string.Empty);
        TestTryConvertStringSadPath<short>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToUnsignedShort_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<ushort>(null);
        TestTryConvertStringSadPath<ushort>(string.Empty);
        TestTryConvertStringSadPath<ushort>("-1");
        TestTryConvertStringSadPath<ushort>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToGuid_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<Guid>(null);
        TestTryConvertStringSadPath<Guid>(string.Empty);
        TestTryConvertStringSadPath<Guid>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableEnum_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<TestLightState?>(string.Empty);

        TestTryConvertStringSadPath<TestLightState?>("NotInEnum");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableByte_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<byte?>(string.Empty);
        TestTryConvertStringSadPath<byte?>("-1");
        TestTryConvertStringSadPath<byte?>("256");
        TestTryConvertStringSadPath<byte?>("a");
        TestTryConvertStringSadPath<byte?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableBoolean_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<bool?>(string.Empty);
        TestTryConvertStringSadPath<bool?>("test");
        TestTryConvertStringSadPath<bool?>("123");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableChar_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<char?>(string.Empty);
        TestTryConvertStringSadPath<char?>("test");
        TestTryConvertStringSadPath<char?>("123");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableDecimal_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<decimal?>(string.Empty);
        TestTryConvertStringSadPath<decimal?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableDouble_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<double?>(string.Empty);
        TestTryConvertStringSadPath<double?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableFloat_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<float?>(string.Empty);
        TestTryConvertStringSadPath<float?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableInt_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<int?>(string.Empty);
        TestTryConvertStringSadPath<int?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableUnsignedInt_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<uint?>(string.Empty);
        TestTryConvertStringSadPath<uint?>("-1");
        TestTryConvertStringSadPath<uint?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableLong_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<long?>(string.Empty);
        TestTryConvertStringSadPath<long?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableUnsignedLong_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<ulong?>(string.Empty);
        TestTryConvertStringSadPath<ulong?>("-1");
        TestTryConvertStringSadPath<ulong?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableShort_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<short?>(string.Empty);
        TestTryConvertStringSadPath<short?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableUnsignedShort_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<ushort?>(string.Empty);
        TestTryConvertStringSadPath<ushort?>("-1");
        TestTryConvertStringSadPath<ushort?>("test");
    }

    [Fact]
    public void TryConvert_FromInvalidStringToNullableGuid_ReturnsExpectedResults()
    {
        TestTryConvertStringSadPath<Guid?>(string.Empty);
        TestTryConvertStringSadPath<Guid?>("abc");
        TestTryConvertStringSadPath<Guid?>("{123}");
    }

    [Fact]
    public void TryConvert_ArraySegmentOfBytesToArraySegmentOfBytes_ReturnsExpectedResult()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, ArraySegment<byte>.Empty, (b1, b2) => b1.SequenceEqual(b2));
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, Array.Empty<byte>(), (b1, b2) => b1.SequenceEqual(b2));
    }

    [Fact]
    public void TryConvert_ArraySegmentOfBytesToNonDefaultType_ReturnsFalseWithNullResult()
    {
        TestTryConvertArraySegmentOfBytesSadPath<IPAddress>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<IPAddress>("127.0.0.1"u8.ToArray());

        TestTryConvertArraySegmentOfBytesSadPath<IPAddress?>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<IPAddress?>("127.0.0.1"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToString_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (string?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (string?)string.Empty);
        TestTryConvertArraySegmentOfBytesHappyPath("test"u8.ToArray(), (string?)"test");
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToEnum_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("Unknown"u8.ToArray(), TestLightState.Unknown);
        TestTryConvertArraySegmentOfBytesHappyPath("On"u8.ToArray(), TestLightState.On);
        TestTryConvertArraySegmentOfBytesHappyPath("Off"u8.ToArray(), TestLightState.Off);
        TestTryConvertArraySegmentOfBytesHappyPath("Unavailable"u8.ToArray(), TestLightState.Unavailable);

        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), TestLightState.Unknown);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), TestLightState.On);
        TestTryConvertArraySegmentOfBytesHappyPath("2"u8.ToArray(), TestLightState.Off);
        TestTryConvertArraySegmentOfBytesHappyPath("3"u8.ToArray(), TestLightState.Unavailable);

        // Enums don't guarantee that values are in range. Leave that responsibility to validation
        TestTryConvertArraySegmentOfBytesHappyPath("123"u8.ToArray(), (TestLightState)123);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToByte_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), 0x00);
        TestTryConvertArraySegmentOfBytesHappyPath("255"u8.ToArray(), 0xff);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToBoolean_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("true"u8.ToArray(), true);
        TestTryConvertArraySegmentOfBytesHappyPath("false"u8.ToArray(), false);
        TestTryConvertArraySegmentOfBytesHappyPath("TrUe"u8.ToArray(), true);
        TestTryConvertArraySegmentOfBytesHappyPath("FaLse"u8.ToArray(), false);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToChar_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(" "u8.ToArray(), ' ');
        TestTryConvertArraySegmentOfBytesHappyPath("a"u8.ToArray(), 'a');
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToDecimal_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("12.25"u8.ToArray(), 12.25M);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToDouble_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(
            "123.456789"u8.ToArray(),
            123.456789,
            (d1, d2) => Math.Abs(d1 - d2) < double.Epsilon
        );
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToFloat_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(
            "123.456789"u8.ToArray(),
            123.456789f,
            (f1, f2) => Math.Abs(f1 - f2) < float.Epsilon
        );
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToInt_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("-1"u8.ToArray(), -1);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), 0);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), 1);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToUnsignedInt_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), 0U);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), 1U);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToLong_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("-1"u8.ToArray(), -1L);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), 0L);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), 1L);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToUnsignedLong_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), 0UL);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), 1UL);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToShort_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("-1"u8.ToArray(), (short)-1);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (short)0);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), (short)1);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToUnsignedShort_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (ushort)0);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), (ushort)1);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToGuid_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath("85cac371-d447-4f2f-9847-dec30712a3b3"u8.ToArray(), Guid.Parse("85cac371-d447-4f2f-9847-dec30712a3b3"));
        TestTryConvertArraySegmentOfBytesHappyPath("{85cac371-d447-4f2f-9847-dec30712a3b3}"u8.ToArray(), Guid.Parse("85cac371-d447-4f2f-9847-dec30712a3b3"));
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableEnum_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (TestLightState?)null);

        TestTryConvertArraySegmentOfBytesHappyPath("Unknown"u8.ToArray(), (TestLightState?)TestLightState.Unknown);
        TestTryConvertArraySegmentOfBytesHappyPath("On"u8.ToArray(), (TestLightState?)TestLightState.On);
        TestTryConvertArraySegmentOfBytesHappyPath("Off"u8.ToArray(), (TestLightState?)TestLightState.Off);
        TestTryConvertArraySegmentOfBytesHappyPath("Unavailable"u8.ToArray(), (TestLightState?)TestLightState.Unavailable);

        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (TestLightState?)TestLightState.Unknown);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), (TestLightState?)TestLightState.On);
        TestTryConvertArraySegmentOfBytesHappyPath("2"u8.ToArray(), (TestLightState?)TestLightState.Off);
        TestTryConvertArraySegmentOfBytesHappyPath("3"u8.ToArray(), (TestLightState?)TestLightState.Unavailable);

        // Enums don't guarantee that values are in range. Leave that responsibility to validation
        TestTryConvertArraySegmentOfBytesHappyPath("123"u8.ToArray(), (TestLightState?)123);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableByte_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (byte?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (byte?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (byte?)0x00);
        TestTryConvertArraySegmentOfBytesHappyPath("255"u8.ToArray(), (byte?)0xff);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableBoolean_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (bool?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (bool?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("true"u8.ToArray(), (bool?)true);
        TestTryConvertArraySegmentOfBytesHappyPath("false"u8.ToArray(), (bool?)false);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableChar_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (char?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (char?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(" "u8.ToArray(), (char?)' ');
        TestTryConvertArraySegmentOfBytesHappyPath("a"u8.ToArray(), (char?)'a');
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableDecimal_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (decimal?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (decimal?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("12.25"u8.ToArray(), (decimal?)12.25M);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableDouble_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(
            ArraySegment<byte>.Empty,
            (double?)null,
            (d1, d2) => (!d1.HasValue && !d2.HasValue) || (d1.HasValue && d2.HasValue && Math.Abs(d1.Value - d2.Value) < double.Epsilon)
        );
        TestTryConvertArraySegmentOfBytesHappyPath(
            null!,
            (double?)null,
            (d1, d2) => (!d1.HasValue && !d2.HasValue) || (d1.HasValue && d2.HasValue && Math.Abs(d1.Value - d2.Value) < double.Epsilon)
        );
        TestTryConvertArraySegmentOfBytesHappyPath(
            "123.456789"u8.ToArray(),
            (double?)123.456789,
            (d1, d2) => (!d1.HasValue && !d2.HasValue) || (d1.HasValue && d2.HasValue && Math.Abs(d1.Value - d2.Value) < double.Epsilon)
        );
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableFloat_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(
            ArraySegment<byte>.Empty,
            (float?)null,
            (f1, f2) => (!f1.HasValue && !f2.HasValue) || (f1.HasValue && f2.HasValue && Math.Abs(f1.Value - f2.Value) < float.Epsilon)
        );
        TestTryConvertArraySegmentOfBytesHappyPath(
            null!,
            (float?)null,
            (f1, f2) => (!f1.HasValue && !f2.HasValue) || (f1.HasValue && f2.HasValue && Math.Abs(f1.Value - f2.Value) < float.Epsilon)
        );
        TestTryConvertArraySegmentOfBytesHappyPath(
            "-123.456789"u8.ToArray(),
            (float?)-123.456789f,
            (f1, f2) => (!f1.HasValue && !f2.HasValue) || (f1.HasValue && f2.HasValue && Math.Abs(f1.Value - f2.Value) < float.Epsilon)
        );
        TestTryConvertArraySegmentOfBytesHappyPath(
            "123.456789"u8.ToArray(),
            (float?)123.456789f,
            (f1, f2) => (!f1.HasValue && !f2.HasValue) || (f1.HasValue && f2.HasValue && Math.Abs(f1.Value - f2.Value) < float.Epsilon)
        );
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableInt_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (int?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (int?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("-1"u8.ToArray(), (int?)-1);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (int?)0);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), (int?)1);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableUnsignedInt_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (uint?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (uint?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (uint?)0);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), (uint?)1);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableLong_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (long?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (long?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("-1"u8.ToArray(), -1L);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), 0L);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), 1L);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableUnsignedLong_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (ulong?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (ulong?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (ulong?)0);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), (ulong?)1);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableShort_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (short?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (short?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("-1"u8.ToArray(), (short?)-1);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (short?)0);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), (short?)1);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableUnsignedShort_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (ushort?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (ushort?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("0"u8.ToArray(), (ushort?)0);
        TestTryConvertArraySegmentOfBytesHappyPath("1"u8.ToArray(), (ushort?)1);
    }

    [Fact]
    public void TryConvert_FromValidByteArrayToNullableGuid_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesHappyPath(ArraySegment<byte>.Empty, (Guid?)null);
        TestTryConvertArraySegmentOfBytesHappyPath(null!, (Guid?)null);
        TestTryConvertArraySegmentOfBytesHappyPath("85cac371-d447-4f2f-9847-dec30712a3b3"u8.ToArray(), (Guid?)Guid.Parse("85cac371-d447-4f2f-9847-dec30712a3b3"));
        TestTryConvertArraySegmentOfBytesHappyPath("{85cac371-d447-4f2f-9847-dec30712a3b3}"u8.ToArray(), (Guid?)Guid.Parse("85cac371-d447-4f2f-9847-dec30712a3b3"));
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToByte_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<byte>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<byte>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<byte>("-1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<byte>("256"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<byte>("a"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<byte>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToBoolean_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<bool>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<bool>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<bool>("1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<bool>("0"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<bool>("y"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<bool>("n"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<bool>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToChar_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<char>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<char>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<char>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToDecimal_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<decimal>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<decimal>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<decimal>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToDouble_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<double>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<double>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<double>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToFloat_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<float>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<float>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<float>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToInt_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<int>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<int>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<int>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToUnsignedInt_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<uint>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<uint>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<uint>("-1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<uint>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToLong_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<long>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<long>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<long>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToUnsignedLong_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<ulong>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<ulong>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<ulong>("-1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<ulong>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToShort_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<short>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<short>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<short>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToUnsignedShort_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<ushort>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<ushort>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<ushort>("-1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<ushort>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToGuid_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<Guid>(null!);
        TestTryConvertArraySegmentOfBytesSadPath<Guid>(ArraySegment<byte>.Empty);
        TestTryConvertArraySegmentOfBytesSadPath<Guid>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableByte_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<byte?>("-1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<byte?>("256"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<byte?>("a"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<byte?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableBoolean_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<bool?>("test"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<bool?>("123"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableChar_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<char?>("test"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<char?>("123"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableDecimal_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<decimal?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableDouble_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<double?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableFloat_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<float?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableInt_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<int?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableUnsignedInt_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<uint?>("-1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<uint?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableLong_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<long?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableUnsignedLong_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<ulong?>("-1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<ulong?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableShort_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<short?>("test"u8.ToArray());
    }

    [Fact]
    public void TryConvert_FromInvalidByteArrayToNullableUnsignedShort_ReturnsExpectedResults()
    {
        TestTryConvertArraySegmentOfBytesSadPath<ushort?>("-1"u8.ToArray());
        TestTryConvertArraySegmentOfBytesSadPath<ushort?>("test"u8.ToArray());
    }

    private void TestTryConvertStringHappyPath<T>(string? payload, T expectedResult, IEqualityComparer<T>? equalityComparer = null)
    {
        IEqualityComparer<T> useEqualityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        TestTryConvertStringHappyPath(payload, expectedResult, (o1, o2) => useEqualityComparer.Equals(o1, o2));
    }

    private void TestTryConvertStringHappyPath<T>(string? payload, T expectedResult, Func<T, T, bool> equalityComparer)
    {
        var converted = DefaultTypeConverters.TryConvert(payload, typeof(T), out var result);

        Assert.True(converted);

        var underlyingType = Nullable.GetUnderlyingType(typeof(T));
        if (underlyingType != null || typeof(T).IsClass)
        {
            if (result == null)
            {
                Assert.Null(expectedResult);
            }
            else
            {
                if (underlyingType != null)
                {
                    Assert.IsType(underlyingType, result);
                }

                var typedResult = (T)result;
                Assert.Equal(expectedResult, typedResult, equalityComparer);
            }
        }
        else
        {
            Assert.NotNull(result);

            var typedResult = Assert.IsType<T>(result);
            Assert.Equal(expectedResult, typedResult, equalityComparer);
        }
    }

    private void TestTryConvertStringSadPath<T>(string? payload)
    {
        var converted = DefaultTypeConverters.TryConvert(payload, typeof(T), out var result);
        Assert.False(converted);
        Assert.Null(result);
    }

    private void TestTryConvertArraySegmentOfBytesHappyPath<T>(ArraySegment<byte> payload, T expectedResult, IEqualityComparer<T>? equalityComparer = null)
    {
        IEqualityComparer<T> useEqualityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        TestTryConvertArraySegmentOfBytesHappyPath(payload, expectedResult, (o1, o2) => useEqualityComparer.Equals(o1, o2));
    }

    private void TestTryConvertArraySegmentOfBytesHappyPath<T>(ArraySegment<byte> payload, T expectedResult, Func<T, T, bool> equalityComparer)
    {
        var converted = DefaultTypeConverters.TryConvert(payload, typeof(T), out var result);

        Assert.True(converted);

        var underlyingType = Nullable.GetUnderlyingType(typeof(T));
        if (underlyingType != null || typeof(T).IsClass)
        {
            if (result == null)
            {
                Assert.Null(expectedResult);
            }
            else
            {
                if (underlyingType != null)
                {
                    Assert.IsType(underlyingType, result);
                }

                var typedResult = (T)result;
                Assert.Equal(expectedResult, typedResult, equalityComparer);
            }
        }
        else
        {
            Assert.NotNull(result);

            var typedResult = Assert.IsType<T>(result);
            Assert.Equal(expectedResult, typedResult, equalityComparer);
        }
    }

    private void TestTryConvertArraySegmentOfBytesSadPath<T>(ArraySegment<byte> payload)
    {
        var converted = DefaultTypeConverters.TryConvert(payload, typeof(T), out var result);
        Assert.False(converted);
        Assert.Null(result);
    }
}
