// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Internal.PackageIndexEntryResponse
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.Internal
{
  [DataContract]
  public class PackageIndexEntryResponse
  {
    [DataMember]
    public Guid PackageId { get; set; }

    [DataMember]
    public Guid PackageVersionId { get; set; }

    [DataMember]
    public bool Created { get; set; }
  }
}
