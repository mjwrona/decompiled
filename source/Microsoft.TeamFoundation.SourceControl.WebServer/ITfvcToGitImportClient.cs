// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.ITfvcToGitImportClient
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public interface ITfvcToGitImportClient
  {
    IEnumerable<TfvcBranch> GetTfvcBranches(string scopePath);

    TeamFoundationDataReader QueryTfvcItems(
      string scopePath,
      TfvcVersionDescriptor versionDescriptor);

    IEnumerable<int> QueryTfvcChangesetIds(string itemPath, DateTime fromDate);

    IEnumerable<int> QueryTfvcChangesetIds(string itemPath, int toId, int top);

    TfvcChangesetRef QueryChangesetRef(int changeSetId);

    IEnumerable<TfvcChange> QueryChangeSetChanges(int changesetId, int pageSize, ItemSpec lastItem);
  }
}
