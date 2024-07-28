// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.ResourceError
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "Error")]
  [ExcludeFromCodeCoverage]
  public class ResourceError
  {
    public ResourceError()
    {
    }

    public ResourceError(HttpStatusCode code, string message)
    {
      this.Code = code.ToString();
      this.Message = message;
    }

    public ResourceError(string code, string message)
    {
      this.Code = code;
      this.Message = message;
    }

    [DataMember]
    public string Code { get; set; }

    [DataMember]
    public string Message { get; set; }
  }
}
