// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Client.Internal.PyPiInternalUpstreamMetadata
// Assembly: Microsoft.VisualStudio.Services.PyPi.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2072801D-0EB4-49B3-8929-AFF365507D86
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Client.Internal.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PyPi.Client.Internal
{
  [DataContract]
  public class PyPiInternalUpstreamMetadata
  {
    [DataMember]
    public IReadOnlyDictionary<string, string[]> RawFileMetadata { get; set; }

    [DataMember]
    public List<UpstreamSourceInfo> SourceChain { get; set; }

    [DataMember]
    public string Base64ZippedGpgSignature { get; set; }
  }
}
