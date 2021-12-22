using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.PnPApi.Tests
{

    class MyComponent : Component
    {
        public ReadOnlyProperty<string> MyReadOnlyProp;
        public WritableProperty<AComplexObj> MyWritableComplexObj;
        public MyComponent(IPropertyStoreWriter updater, string name) : base(updater, name)
        {
            MyReadOnlyProp = new ReadOnlyProperty<string>(updater, "Name", name);
            MyWritableComplexObj = new WritableProperty<AComplexObj>(updater, null, "MyWritableComplexObj", name);
        }

        public override Dictionary<string, object> ToJsonDict()
        {
            var res = new Dictionary<string, object>();
            res.Add(MyReadOnlyProp.Name, MyReadOnlyProp.PropertyValue);
            return res;
        }
    }

    public class ComponentFixture
    {
        [Fact]
        public async Task ReportPropertySerializesWithComponentFlag()
        {
            MockUpdateBinder updaterMock = new();
            var myComp = new MyComponent(updaterMock, "myComp");
            await myComp.ReportPropertyAsync();
            Assert.Equal("", JsonSerializer.Serialize(updaterMock.ReceivedPayload));
        }
    }
}
