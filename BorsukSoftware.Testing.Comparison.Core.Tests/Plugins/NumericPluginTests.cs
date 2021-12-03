using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    public class NumericPluginTests
    {
        public static IEnumerable<object[]> CheckNullsAreTreatedAsZeros_Differences_Data
        {
            get
            {
                // TODO - Add coverage for floats
                // TODO - Add coverage for decimals
                {
                    var plugin = new Plugins.DoubleComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    var @double = 23.345;
                    yield return new object[]
                    {
                        plugin,
                        @double,
                        @double,
                        0- @double
                    };
                }

                {
                    var plugin = new Plugins.BigIntegerComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    var bigint = new System.Numerics.BigInteger(new byte[] { 234, 231, 56, 1, 6, 1, 7, 1, 7, 2, 5, 2 });
                    yield return new object[] { plugin,
                        bigint,
                        bigint,
                        new System.Numerics.BigInteger(0) - bigint };
                }

                {
                    var plugin = new Plugins.SByteComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin,
                        (sbyte) -32,
                        (int) -32,
                        (int) 32};
                }

                {
                    var plugin = new Plugins.Int16ComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin,
                        (short) 32,
                        (int) 32,
                        (int) -32};
                }

                {
                    var plugin = new Plugins.Int32ComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin,
                        (int) 32,
                        (int) 32,
                        (int) -32};

                }

                {
                    var plugin = new Plugins.Int64ComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin,
                        32L,
                        32L,
                        -32L};
                }

                {
                    var plugin = new Plugins.ByteComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin,
                        (byte) 32,
                        (int) 32,
                        (int) -32};
                }

                {
                    var plugin = new Plugins.UInt16ComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin,
                        (ushort) 32,
                        (int) 32,
                        (int) -32};
                }

                {
                    var plugin = new Plugins.UInt32ComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    // Yes, tihs is hideous, but we're validating behaviour here and we're matching what .net will do
                    unchecked
                    {
                        yield return new object[] { plugin,
                        (uint) 32,
                        (uint) (int) 32,
                        (uint) (int) -32};
                    }

                }

                {
                    var plugin = new Plugins.UInt64ComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    unchecked
                    {
                        yield return new object[] { plugin,
                        (ulong) 32L,
                        (ulong) 32L,
                        (ulong) -32L};
                    }
                }

                {
                    var plugin = new Plugins.IntComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin,
                        (sbyte) 32,
                        (long) 32,
                        (long) -32};

                    yield return new object[] { plugin,
                        (short) 32,
                        (long) 32,
                        (long) -32};

                    yield return new object[] { plugin,
                        (int) 3287,
                        (long) 3287,
                        (long) -3287};

                    yield return new object[] { plugin,
                        (long) 322345L,
                        (long) 322345L,
                        (long) -322345L};
                }

                unchecked
                {
                    var plugin = new Plugins.UIntComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin,
                        (byte) 32,
                        (ulong) 32,
                        (ulong) -32};

                    yield return new object[] { plugin,
                        (ushort) 32,
                        (ulong) 32,
                        (ulong) -32};

                    yield return new object[] { plugin,
                        (uint) 3287,
                        (ulong) 3287,
                        (ulong) -3287};

                    yield return new object[] { plugin,
                        (ulong) 322345L,
                        (ulong) 322345L,
                        (ulong) -322345L};
                }
            }
        }

        [MemberData(nameof(CheckNullsAreTreatedAsZeros_Differences_Data))]
        [Theory]
        public void CheckNullsAreTreatedAsZeros_Differences(IObjectComparerPlugin plugin, object value, object expectedDifferenceWhenExpectedIsNull, object expectedDifferenceWhenActualIsNull)
        {
            {
                var output = plugin.TryCompare("key", value, null);

                Assert.Equal(ComparisonResultType.Different, output.ComparisonResultType);
                Assert.NotNull(output.Payload);
                Assert.Equal(expectedDifferenceWhenActualIsNull, output.Payload);
            }

            {
                var output = plugin.TryCompare("key", null, value);

                Assert.Equal(ComparisonResultType.Different, output.ComparisonResultType);
                Assert.NotNull(output.Payload);
                Assert.Equal(expectedDifferenceWhenExpectedIsNull, output.Payload);
            }
        }

        public static IEnumerable<object[]> CheckNullsAreTreatedAsZeros_Equality_Data
        {
            get
            {
                {
                    var plugin = new Plugins.DoubleComparerPlugin
                    {
                        TreatMissingValuesAsZero = true
                    };

                    yield return new object[] { plugin, 0.0 };
                }

                // Within tolerance
                yield return new object[]
                {
                    new Plugins.DoubleComparerPlugin
                    {
                        TreatMissingValuesAsZero = true,
                        ToleranceModes= FloatingPointToleranceModes.AbsoluteTolerance,
                        AbsoluteTolerance = 0.1
                    },
                    0.1
                };
            }
        }

        [MemberData(nameof(CheckNullsAreTreatedAsZeros_Equality_Data))]
        [Theory]
        public void CheckNullsAreTreatedAsZeros_Equality(IObjectComparerPlugin plugin, object value)
        {
            {
                var output = plugin.TryCompare("key", value, null);

                Assert.Equal(ComparisonResultType.Equal, output.ComparisonResultType);
            }

            {
                var output = plugin.TryCompare("key", null, value);

                Assert.Equal(ComparisonResultType.Equal, output.ComparisonResultType);
            }
        }


        public static IEnumerable<object[]> CheckNumericOverflowBehaviour_Data
        {
            get
            {
                {
                    var plugin = new Plugins.Int16ComparerPlugin();
                    yield return new object[]
                    {
                        plugin,
                        short.MaxValue,
                        short.MinValue,
                        short.MinValue - short.MaxValue
                    };

                    yield return new object[]
                    {
                        plugin,
                        short.MinValue,
                        short.MaxValue,
                        short.MaxValue - short.MinValue
                    };
                }

                unchecked
                {
                    var plugin = new Plugins.Int32ComparerPlugin();
                    yield return new object[]
                    {
                        plugin,
                        int.MaxValue,
                        int.MinValue,
                        int.MinValue - int.MaxValue
                    };

                    yield return new object[]
                    {
                        plugin,
                        int.MinValue,
                        int.MaxValue,
                        int.MaxValue - int.MinValue
                    };
                }

                unchecked
                {
                    var plugin = new Plugins.Int64ComparerPlugin();
                    yield return new object[]
                    {
                        plugin,
                        long.MaxValue,
                        long.MinValue,
                        long.MinValue - long.MaxValue
                    };

                    yield return new object[]
                    {
                        plugin,
                        long.MinValue,
                        long.MaxValue,
                        long.MaxValue - long.MinValue
                    };
                }

                unchecked
                {
                    var plugin = new Plugins.IntComparerPlugin();
                    yield return new object[]
                    {
                        plugin,
                        short.MaxValue,
                        short.MinValue,
                        (long)short.MinValue - (long)short.MaxValue
                    };

                    yield return new object[]
                    {
                        plugin,
                        short.MinValue,
                        short.MaxValue,
                        (long)short.MaxValue - (long)short.MinValue

                    };
                    yield return new object[]
                    {
                        plugin,
                        int.MaxValue,
                        int.MinValue,
                        (long)int.MinValue - (long)int.MaxValue
                    };

                    yield return new object[]
                    {
                        plugin,
                        int.MinValue,
                        int.MaxValue,
                        (long)int.MaxValue - (long)int.MinValue
                    };

                    yield return new object[]
                    {
                        plugin,
                        long.MaxValue,
                        long.MinValue,
                        long.MinValue - long.MaxValue
                    };

                    yield return new object[]
                    {
                        plugin,
                        long.MinValue,
                        long.MaxValue,
                        long.MaxValue - long.MinValue
                    };
                }
            }
        }

        [MemberData(nameof(CheckNumericOverflowBehaviour_Data))]
        [Theory]
        public void CheckNumericOverflowBehaviour(IObjectComparerPlugin plugin, object expected, object actual, object expectedDifference)
        {
            var output = plugin.TryCompare("key", expected, actual);

            Assert.Equal(ComparisonResultType.Different, output.ComparisonResultType);
            Assert.Equal(expectedDifference, output.Payload);
        }

        #region Standard Behaviour

        public static IEnumerable<object[]> StandardBehaviour_Data
        {
            get
            {
                // Signed values
                yield return new object[]
                {
                    new SByteComparerPlugin(),
                    (sbyte) 32,
                    (sbyte) 17,
                    ((sbyte) 17) - ((sbyte)32)
                };

                yield return new object[]
                {
                    new Int16ComparerPlugin(),
                    (short) 32,
                    (short) 17,
                    ((short) 17) - ((short)32)
                };

                yield return new object[]
                {
                    new Int32ComparerPlugin(),
                    (int) 32,
                    (int) 17,
                    ((int) 17) - ((int)32)
                };

                yield return new object[]
                {
                    new Int64ComparerPlugin(),
                    (long) 32,
                    (long) 17,
                    ((long) 17) - ((long)32)
                };

                // Unsigned values
                unchecked
                {
                    yield return new object[]
                    {
                        new ByteComparerPlugin(),
                            (byte) 32,
                            (byte) 17,
                            ((byte) 17) - ((byte)32)
                    };

                    yield return new object[]
                    {
                        new UInt16ComparerPlugin(),
                            (ushort) 32,
                            (ushort) 17,
                            ((ushort) 17) - ((ushort)32)
                    };

                    yield return new object[]
                    {
                        new UInt32ComparerPlugin(),
                            (uint) 32,
                            (uint) 17,
                            ((uint) 17) - ((uint)32)
                    };

                    yield return new object[]
                    {
                        new UInt64ComparerPlugin(),
                            (ulong) 32,
                            (ulong) 17,
                            ((ulong) 17) - ((ulong)32)
                    };
                }
            }
        }

        [MemberData(nameof(StandardBehaviour_Data))]
        [Theory]
        public void StandardBehaviour(IObjectComparerPlugin plugin, object expected, object actual, object expectedDifference)
        {
            var result = plugin.TryCompare("value", expected, actual);

            Assert.Equal(ComparisonResultType.Different, result.ComparisonResultType);
            Assert.Equal(result.Payload, expectedDifference);
        }

        #endregion
    }
}
