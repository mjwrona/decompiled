// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Error
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal class Error : Resource
  {
    [JsonProperty(PropertyName = "code")]
    public string Code
    {
      get => this.GetValue<string>("code");
      set => this.SetValue("code", (object) value);
    }

    [JsonProperty(PropertyName = "message")]
    public string Message
    {
      get => this.GetValue<string>("message");
      set => this.SetValue("message", (object) value);
    }

    [JsonProperty(PropertyName = "errorDetails")]
    internal string ErrorDetails
    {
      get => this.GetValue<string>("errorDetails");
      set => this.SetValue("errorDetails", (object) value);
    }

    [JsonProperty(PropertyName = "additionalErrorInfo")]
    internal string AdditionalErrorInfo
    {
      get => this.GetValue<string>("additionalErrorInfo");
      set => this.SetValue("additionalErrorInfo", (object) value);
    }
  }
}
