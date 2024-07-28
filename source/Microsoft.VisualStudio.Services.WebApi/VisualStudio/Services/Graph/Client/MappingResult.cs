// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.MappingResult
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  public class MappingResult
  {
    public MappingResult(HttpStatusCode? code, string errorMessage)
    {
      this.Code = code?.ToString();
      this.ErrorMessage = errorMessage;
    }

    public MappingResult(HttpStatusCode? code)
      : this(code, (string) null)
    {
    }

    public MappingResult()
      : this(new HttpStatusCode?(), (string) null)
    {
    }

    [DataMember]
    public string Code { set; get; }

    [DataMember]
    public string ErrorMessage { set; get; }
  }
}
