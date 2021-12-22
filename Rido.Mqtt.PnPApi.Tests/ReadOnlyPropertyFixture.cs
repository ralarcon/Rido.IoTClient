using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.PnPApi.Tests
{
    public class ReadOnlyPropertyFixture
    {
        [Fact]
        public async Task ReportSimpleRootProperty()
        {
            MockUpdateBinder updaterMock = new();
            ReadOnlyProperty<double> rop = new(updaterMock, "aProp");
            await rop.ReportPropertyAsync(0.4);

            var expectedPayload = new Dictionary<string, object>
            {
                {
                    "aProp", 0.4
                }
            };
            Assert.Equal(expectedPayload["aProp"], updaterMock.ReceivedPayload["aProp"]);
        }

        [Fact]
        public async Task ReportComplexRootProperty()
        {
            MockUpdateBinder updaterMock = new();
            ReadOnlyProperty<AComplexObj> rop = new(updaterMock, "aComplexObj");
            await rop.ReportPropertyAsync(new AComplexObj() { AStringProp = "hello", AIntProp = 23});

            var expectedPayload = new Dictionary<string, object>
            {
                {
                    "aComplexObj", new AComplexObj() { AStringProp = "hello", AIntProp=23}
                }
            };
            Assert.Equal(expectedPayload["aComplexObj"], updaterMock.ReceivedPayload["aComplexObj"]);
        }

        [Fact]
        public async Task ReportSimplePropertyInComponent()
        {
            MockUpdateBinder updaterMock = new();
            ReadOnlyProperty<double> rop = new(updaterMock, "aProp", "c1");
            await rop.ReportPropertyAsync(0.4, true);

            var expectedPayload = new Dictionary<string, object>
            {
                {
                    "c1", new Dictionary<string, object>
                    {
                        {
                            "__t", "c"
                        },
                        { 
                            "aProp", 0.4
                        }
                    }
                }
            };
            Assert.Equal(expectedPayload["c1"], updaterMock.ReceivedPayload["c1"]);
        }

        [Fact]
        public async Task ReportComplexPropertyInComponent()
        {
            MockUpdateBinder updaterMock = new();
            ReadOnlyProperty<AComplexObj> rop = new(updaterMock, "aComplexObj", "c1");
            await rop.ReportPropertyAsync(new AComplexObj() { AStringProp ="hello", AIntProp =23}, true);

            var expectedPayload = new Dictionary<string, object>
            {
                {
                    "c1", new Dictionary<string, object>
                    {
                        {
                            "__t", "c"
                        },
                        {
                            "aComplexObj", new AComplexObj() { AStringProp ="hello", AIntProp=23}
                        }
                    }
                }
            };
            Assert.Equal(expectedPayload["c1"], updaterMock.ReceivedPayload["c1"]);
        }
    }
}
