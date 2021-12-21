using Rido.Mqtt.PnPApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.PnPApi.Tests
{
    public class WritablePropertyFixture
    {
        static string Stringify(object o) => System.Text.Json.JsonSerializer.Serialize(o);

        //readonly IMqttConnection connection;
        readonly MockUpdateBinder<string> updaterMock;
        public WritablePropertyFixture()
        {
            //connection = new MockMqttConnection();
            updaterMock = new MockUpdateBinder<string>();
        }

        [Fact]
        public async Task InitEmptyTwinReportsDefaultValues()
        {
            MockUpdateCallback<double> updateCallbackMock = new();
            WritableProperty<double> wp = new(updaterMock, updateCallbackMock, "blah");
            string twin = Stringify(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            await wp.InitPropertyAsync(twin, 0.2);

            var expectedPayload = new Dictionary<string, object>
            {
                { "blah",
                    new PropertyAck<double>("blah")
                    {
                        Value = 0.2,
                        Description = "Init from default value",
                        Status = 203,
                        Version = 0
                    }
                }
            };

            Assert.Equal(Stringify(expectedPayload["blah"]), Stringify(updaterMock.ReceivedPayload["blah"]));
            Assert.Equal(0.2, wp.PropertyValue.Value);
            Assert.Equal(0, wp.PropertyValue.Version);
            Assert.Equal(203, wp.PropertyValue.Status);
        }

        [Fact]
        public async Task InitTwinWithReported_Does_Not_TriggerCallback()
        {
            MockUpdateCallback<double> updateCallbackMock = new();
            WritableProperty<double> wp = new(updaterMock, updateCallbackMock, "myProp");
            string twin = Stringify(new
            {
                reported = new
                {
                    myProp = new
                    {
                        ac = 203,
                        av = 1,
                        value = 4.3
                    }
                },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            bool callbackReceived = false;
            updateCallbackMock.OnProperty_Updated = async p =>
            {
                callbackReceived = true;
                return await Task.FromResult(p);
            };

            await wp.InitPropertyAsync(twin, 0.2);

            Assert.False(callbackReceived);
            Assert.Equal(4.3, wp.PropertyValue.Value);
            Assert.Equal(1, wp.PropertyValue.Version);
            Assert.Equal(203, wp.PropertyValue.Status);
        }

        [Fact]
        public async Task InitTwinWithDesiredTriggersUpdate()
        {
            MockUpdateCallback<double> updateCallbackMock = new();
            WritableProperty<double> wp = new(updaterMock, updateCallbackMock, "myDouble");
            Assert.Equal(0, wp.PropertyValue.Value);
            bool received = false;
            wp.OnProperty_Updated = async p =>
            {
                received = true;
                p.Status = 200;
                return await Task.FromResult(p);
            };
            string twin = Stringify(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() { { "$version", 2 }, { "myDouble", 2.3 } }
            });



            await wp.InitPropertyAsync(twin, 1);
            Assert.True(received);
            Assert.Equal(2.3, wp.PropertyValue.Value);
        }

        [Fact]
        public async Task InitReportedDoubleInComponent()
        {
            MockUpdateCallback<double> updateCallbackMock = new();
            var wpWithComp = new WritableProperty<double>(updaterMock, updateCallbackMock, "myProp", "myComp");
            string twin = Stringify(new
            {
                reported = new
                {
                    myComp = new
                    {
                        __t = "c",
                        myProp = new
                        {
                            ac = 203,
                            av = 1,
                            value = 4.3
                        }
                    }
                },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(4.3, wpWithComp.PropertyValue.Value);
            Assert.Equal(1, wpWithComp.PropertyValue.Version);
            Assert.Equal(203, wpWithComp.PropertyValue.Status);
        }

        [Fact]
        public async Task InitTwinWithReportedComplex()
        {
            MockUpdateCallback<AComplexObj> updateCallbackMock = new();
            var wpComplexObj = new WritableProperty<AComplexObj>(updaterMock, updateCallbackMock, "myComplexObj");
            string twin = Stringify(new
            {
                reported = new
                {
                    myComplexObj = new
                    {
                        ac = 203,
                        av = 1,
                        value = new
                        {
                            MyProperty = "fake twin value"
                        }
                    }
                },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            await wpComplexObj.InitPropertyAsync(twin, new AComplexObj());
            Assert.Equal(string.Empty, wpComplexObj.PropertyValue.Value.AStringProp);
            Assert.Equal(1, wpComplexObj.PropertyValue.Version);
            Assert.Equal(203, wpComplexObj.PropertyValue.Status);
        }



        [Fact]
        public async Task InitTwinComplexWithDesiredTriggersUpdate()
        {
            MockUpdateCallback<AComplexObj> updateCallbackMock = new();
            WritableProperty<AComplexObj> wp = new(updaterMock, updateCallbackMock, "myComplexObj");
            Assert.Null(wp.PropertyValue.Value);
            wp.OnProperty_Updated = async p =>
            {
                p.Status = 200;
                return await Task.FromResult(p);
            };
            string twin = Stringify(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() { { "$version", 2 }, { "myComplexObj", new AComplexObj { AStringProp = "twinValue" } } }
            });
            await wp.InitPropertyAsync(twin, new AComplexObj());
            Assert.Equal("twinValue", wp.PropertyValue.Value.AStringProp);
            Assert.Equal(200, wp.PropertyValue.Status);
            Assert.Equal(2, wp.PropertyValue.Version);
            Assert.Equal(2, wp.PropertyValue.DesiredVersion);
        }


        [Fact]
        public async Task InitComponentTwinWithDesiredComponent()
        {
            MockUpdateCallback<double> updateCallbackMock = new();
            var wpWithComp = new WritableProperty<double>(updaterMock, updateCallbackMock, "myProp", "myComp");
            string twin = Stringify(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() {
                    {
                        "$version", 2
                    },
                    {

                        "myComp", new Dictionary<string, object>() {
                            {
                                "__t", "c"
                            },
                            {
                                "myProp", 3.4
                            }
                        }
                    }
                }
            });
            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(3.4, wpWithComp.PropertyValue.Value);
            Assert.Null(wpWithComp.PropertyValue.Description);
            Assert.Equal(0, wpWithComp.PropertyValue.Status);
            Assert.Equal(2, wpWithComp.PropertyValue.DesiredVersion);
        }

        [Fact]
        public async Task InitComplexComponentTwinWithDesiredComponent()
        {
            MockUpdateCallback<AComplexObj> updateCallbackMock = new();
            var wpWithComp = new WritableProperty<AComplexObj>(updaterMock, updateCallbackMock, "myComplexObj", "myComp");
            string twin = Stringify(new
            {
                reported = new Dictionary<string, object>() { { "$version", 1 } },
                desired = new Dictionary<string, object>() {
                    {
                        "$version", 2
                    },
                    {

                        "myComp", new Dictionary<string, object>() {
                            {
                                "__t", "c"
                            },
                            {
                                "myComplexObj", new AComplexObj { AStringProp = "twinValue"}
                            }
                        }
                    }
                }
            });

            bool receivedUpdate = false;
            wpWithComp.OnProperty_Updated = async p =>
            {
                receivedUpdate = true;
                return await Task.FromResult(p);
            };

            await wpWithComp.InitPropertyAsync(twin, new AComplexObj());
            Assert.True(receivedUpdate);
            Assert.Equal("twinValue", wpWithComp.PropertyValue.Value.AStringProp);
            Assert.Null(wpWithComp.PropertyValue.Description);
            Assert.Equal(0, wpWithComp.PropertyValue.Status);
            Assert.Equal(2, wpWithComp.PropertyValue.DesiredVersion);
            Assert.Equal(2, wpWithComp.PropertyValue.Version);
        }


        [Fact]
        public async Task InitTwinWithReportedInComponent()
        {
            MockUpdateCallback<double> updateCallbackMock = new();
            var wpWithComp = new WritableProperty<double>(updaterMock, updateCallbackMock, "myProp", "myComp");
            string twin = Stringify(new
            {
                desired = new Dictionary<string, object>()
                {
                    { "$version", 3 }
                },
                reported = new Dictionary<string, object>()
                {
                    {
                        "$version", 4
                    },
                    {
                        "myComp", new
                        {
                            __t = "c",
                            myProp = new
                            {
                                ac = 200,
                                av = 3 ,
                                ad = "desc",
                                value = 3.4
                            }
                        }
                    }
                }
            });


            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(3.4, wpWithComp.PropertyValue.Value);
            Assert.Equal("desc", wpWithComp.PropertyValue.Description);
            Assert.Equal(200, wpWithComp.PropertyValue.Status);
            Assert.Equal(0, wpWithComp.PropertyValue.DesiredVersion);
        }

        [Fact]
        public async Task InitTwinWithReportedInComponentWithoutFlag()
        {
            MockUpdateCallback<double> updateCallbackMock = new();
            var wpWithComp = new WritableProperty<double>(updaterMock, updateCallbackMock, "myProp", "myComp");
            string twin = Stringify(new
            {
                desired = new Dictionary<string, object>()
                {
                    { "$version", 3 }
                },
                reported = new Dictionary<string, object>()
                {
                    {
                        "$version", 4
                    },
                    {
                        "myComp", new
                        {
                            myProp = new
                            {
                                ac = 200,
                                av = 3 ,
                                ad = "desc",
                                value = 3.4
                            }
                        }
                    }
                }
            });

            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(0.2, wpWithComp.PropertyValue.Value);
            Assert.Equal("Init from default value", wpWithComp.PropertyValue.Description);
            Assert.Equal(203, wpWithComp.PropertyValue.Status);
            Assert.Equal(0, wpWithComp.PropertyValue.DesiredVersion);
        }



        [Fact]
        public async Task InitReportedDoubleInComponentWithouFlag()
        {
            MockUpdateCallback<double> updateCallbackMock = new();
            var wpWithComp = new WritableProperty<double>(updaterMock, updateCallbackMock, "myProp", "myComp");
            string twin = Stringify(new
            {
                reported = new
                {
                    myComp = new
                    {
                        myProp = new
                        {
                            ac = 203,
                            av = 1,
                            value = 4.3
                        }
                    }
                },
                desired = new Dictionary<string, object>() { { "$version", 1 } },
            });

            await wpWithComp.InitPropertyAsync(twin, 0.2);
            Assert.Equal(0.2, wpWithComp.PropertyValue.Value);
            Assert.Equal(0, wpWithComp.PropertyValue.Version);
            Assert.Equal(203, wpWithComp.PropertyValue.Status);
        }

    }
}
