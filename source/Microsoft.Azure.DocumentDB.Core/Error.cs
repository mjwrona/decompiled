// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Error
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  public class Error : Resource
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
