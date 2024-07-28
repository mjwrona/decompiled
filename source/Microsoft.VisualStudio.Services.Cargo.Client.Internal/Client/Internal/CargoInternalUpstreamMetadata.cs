// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Client.Internal.CargoInternalUpstreamMetadata
// Assembly: Microsoft.VisualStudio.Services.Cargo.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2A5EDF53-498F-4A63-B7BC-FF484C198E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Client.Internal.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cargo.Client.Internal
{
  [DataContract]
  public class CargoInternalUpstreamMetadata
  {
    [DataMember]
    public string IngestionManifestBytes { get; set; }

    [DataMember]
    public bool PublishManifestIsReal { get; set; }

    [DataMember]
    public string IndexRowBytes { get; set; }

    [DataMember]
    public IEnumerable<UpstreamSourceInfo> SourceChain { get; set; }

    [DataMember]
    public string Sha256 { get; set; }

    [DataMember]
    public string Version { get; set; }
  }
}
