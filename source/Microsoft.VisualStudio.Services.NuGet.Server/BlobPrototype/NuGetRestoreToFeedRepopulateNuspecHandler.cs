// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRestoreToFeedRepopulateNuspecHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRestoreToFeedRepopulateNuspecHandler : 
    IHandler<(INuGetMetadataEntryWriteable entry, INuGetRestoreToFeedOperationData op), INuGetMetadataEntryWriteable>
  {
    public INuGetMetadataEntryWriteable Handle(
      (INuGetMetadataEntryWriteable entry, INuGetRestoreToFeedOperationData op) request)
    {
      request.entry.SetNuspecBytes(request.op.NuspecBytes, false);
      return request.entry;
    }
  }
}
