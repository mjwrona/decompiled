// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.InternalSchemaProperties
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;

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
