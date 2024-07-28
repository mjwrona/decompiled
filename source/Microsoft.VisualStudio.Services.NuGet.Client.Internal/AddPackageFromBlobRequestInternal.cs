// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.AddPackageFromBlobRequestInternal
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  [DataContract]
  public class AddPackageFromBlobRequestInternal
  {
    [DataMember(EmitDefaultValue = false)]
    public string BlobId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Blob Blob { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DropName { get; set; }
  }
}
