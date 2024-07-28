// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.NuGetMigrationJobConstants
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public static class NuGetMigrationJobConstants
  {
    public static readonly Guid MigrationKickerJobId = Guid.Parse("1e0739d4-c2d4-4016-84ca-f2de761d7d69");
    public static readonly JobCreationInfo MigrationKickerJobCreationInfo = new JobCreationInfo("MigrationKickerJob", "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.BlobPrototype.MigrationKickerJob", TeamFoundationHostType.Deployment);
    public static readonly JobCreationInfo NuGetMigrationProcessingJobCreationInfo = new JobCreationInfo("FeedMigrationProcessingJob", "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.BlobPrototype.MigrationProcessingJob", TeamFoundationHostType.ProjectCollection);
  }
}
