// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UserDefinedFunction
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal class UserDefinedFunction : Resource
  {
    [JsonProperty(PropertyName = "body")]
    public string Body
    {
      get => this.GetValue<string>("body");
      set => this.SetValue("body", (object) value);
    }
  }
}
