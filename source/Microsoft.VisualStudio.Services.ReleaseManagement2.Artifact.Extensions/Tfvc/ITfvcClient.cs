// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc.ITfvcClient
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc
{
  public interface ITfvcClient
  {
    IList<TfvcChangesetRef> GetChangesets(IVssRequestContext context, Guid projectId, int count);

    IList<TfvcItem> GetItems(
      IVssRequestContext context,
      Guid projectId,
      string scopePath,
      VersionControlRecursionType recursionLevel,
      TfvcVersionDescriptor versionDescriptor);

    IList<TfvcItem> GetItems(
      IVssRequestContext context,
      string scopePath,
      VersionControlRecursionType recursionLevel,
      TfvcVersionDescriptor versionDescriptor);

    string GetItemContent(IVssRequestContext context, string path);
  }
}
