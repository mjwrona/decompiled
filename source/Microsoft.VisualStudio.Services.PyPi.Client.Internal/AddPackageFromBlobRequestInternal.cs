// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Client.Internal.AddPackageFromBlobRequestInternal
// Assembly: Microsoft.VisualStudio.Services.PyPi.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2072801D-0EB4-49B3-8929-AFF365507D86
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Client.Internal.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PyPi.Client.Internal
{
  [DataContract]
  public class AddPackageFromBlobRequestInternal
  {
    [DataMember]
    public Blob Blob { get; set; }

    [DataMember]
    public long Length { get; set; }

    [DataMember]
    public string FileName { get; set; }

    [DataMember]
    public Dictionary<string, string[]> PackageMetadata { get; set; }
  }
}
