using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.PnPApi.Tests
{
    public class ReadOnlyPropertyFixture
    {
        static string Stringify(object o) => System.Text.Json.JsonSerializer.Serialize(o);

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
            Assert.Equal(Stringify(expectedPayload), Stringify(updaterMock.ReceivedPayload));
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
            Assert.Equal(Stringify(expectedPayload), Stringify(updaterMock.ReceivedPayload));
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
            Assert.Equal(Stringify(expectedPayload), Stringify(updaterMock.ReceivedPayload));
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
            Assert.Equal(Stringify(expectedPayload), Stringify(updaterMock.ReceivedPayload));
        }
    }
}
