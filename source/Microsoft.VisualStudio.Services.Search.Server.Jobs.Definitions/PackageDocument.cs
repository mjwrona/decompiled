// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.PackageDocument
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class PackageDocument : CorePipelineDocument<PackageDocumentId>
  {
    public PackageDocument(Guid feedId, PackageChange packageChange)
      : base(new PackageDocumentId(feedId, packageChange.Package.Id, packageChange.PackageVersionChange.PackageVersion.Id))
    {
    }

    public override string ToString() => this.Id.ToString();

    public override bool Equals(object obj) => obj is PackageDocument packageDocument && this.Id.Equals(packageDocument.Id);

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
