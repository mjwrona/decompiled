// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.AccessTokenErrorResponse
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.OAuth
{
  [DataContract]
  public sealed class AccessTokenErrorResponse
  {
    public AccessTokenErrorResponse()
    {
    }

    public AccessTokenErrorResponse(string error) => this.Error = error;

    [DataMember(Name = "error")]
    public string Error { get; set; }
  }
}
