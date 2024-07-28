// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.InternalSchemaProperties
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class InternalSchemaProperties : JsonSerializable, ICloneable
  {
    [JsonProperty(PropertyName = "useSchemaForAnalyticsOnly")]
    internal bool UseSchemaForAnalyticsOnly
    {
      get => this.GetValue<bool>("useSchemaForAnalyticsOnly");
      set => this.SetValue("useSchemaForAnalyticsOnly", (object) value);
    }

    public object Clone() => (object) new InternalSchemaProperties()
    {
      UseSchemaForAnalyticsOnly = this.UseSchemaForAnalyticsOnly
    };
  }
}
