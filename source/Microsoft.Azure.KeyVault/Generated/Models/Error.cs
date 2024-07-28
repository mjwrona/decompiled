// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.Error
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class Error
  {
    public Error()
    {
    }

    public Error(string code = null, string message = null, Error innerError = null)
    {
      this.Code = code;
      this.Message = message;
      this.InnerError = innerError;
    }

    [JsonProperty(PropertyName = "code")]
    public string Code { get; private set; }

    [JsonProperty(PropertyName = "message")]
    public string Message { get; private set; }

    [JsonProperty(PropertyName = "innererror")]
    public Error InnerError { get; private set; }
  }
}
