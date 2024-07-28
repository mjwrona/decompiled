// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc.TfvcHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc
{
  public class TfvcHelper
  {
    private readonly ITfvcClient tfvcClient;

    public TfvcHelper(ITfvcClient tfvcClient) => this.tfvcClient = tfvcClient;

    public IList<TfvcChangesetRef> GetChangesets(
      IVssRequestContext context,
      Guid projectId,
      int count)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return this.tfvcClient.GetChangesets(context, projectId, count);
    }

    public IList<TfvcItem> GetItems(
      IVssRequestContext context,
      Guid projectId,
      string scopePath,
      VersionControlRecursionType recursionLevel,
      TfvcVersionDescriptor versionDescriptor)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return this.tfvcClient.GetItems(context, projectId, scopePath, recursionLevel, versionDescriptor);
    }

    public IList<TfvcItem> GetItems(
      IVssRequestContext context,
      string scopePath,
      VersionControlRecursionType recursionLevel,
      TfvcVersionDescriptor versionDescriptor)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return this.tfvcClient.GetItems(context, scopePath, recursionLevel, versionDescriptor);
    }

    public string GetItemContent(IVssRequestContext context, string path)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return this.tfvcClient.GetItemContent(context, path);
    }
  }
}
