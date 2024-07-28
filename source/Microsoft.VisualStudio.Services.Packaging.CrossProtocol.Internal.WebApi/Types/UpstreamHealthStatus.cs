// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types.UpstreamHealthStatus
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 208E7E0C-C249-4CB0-B738-E2A4534A31E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types
{
  [DataContract]
  public class UpstreamHealthStatus
  {
    [DataMember]
    public string UpstreamId { get; set; }

    [DataMember]
    public StatusDetails ConfidentStatus { get; set; }

    [DataMember]
    public StatusDetails PartialStatus { get; set; }
  }
}
