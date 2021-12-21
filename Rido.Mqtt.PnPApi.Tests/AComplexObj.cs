namespace Rido.Mqtt.PnPApi.Tests
{
    class AComplexObj
    {
        public string AStringProp { get; set; } = string.Empty;
        public int AIntProp { get; set; }

        public override bool Equals(object obj)
        {
            var o = obj as AComplexObj;
            return AStringProp == o.AStringProp && 
                   AIntProp == o.AIntProp;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
