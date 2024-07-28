// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.IIndexingStatusService
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [DefaultServiceImplementation(typeof (IndexingStatusService))]
  public interface IIndexingStatusService : IVssFrameworkService
  {
    bool Supports(IEntityType entityType);

    CollectionIndexingStatus GetCollectionIndexingStatus(IEntityType entityType);

    bool GetIndexingStatusDetails(
      IVssRequestContext requestContext,
      IEntityType entityType,
      string projectName,
      string repoName,
      string branchName);
  }
}
