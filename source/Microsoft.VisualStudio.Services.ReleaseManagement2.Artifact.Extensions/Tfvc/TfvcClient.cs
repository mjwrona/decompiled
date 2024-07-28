// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc.TfvcClient
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Tfvc
{
  public class TfvcClient : ITfvcClient
  {
    public IList<TfvcChangesetRef> GetChangesets(
      IVssRequestContext context,
      Guid projectId,
      int count)
    {
      TfvcHttpClient tfvcHttpClient = context.GetClient<TfvcHttpClient>();
      Func<Task<List<TfvcChangesetRef>>> func = (Func<Task<List<TfvcChangesetRef>>>) (() =>
      {
        TfvcHttpClient tfvcHttpClient1 = tfvcHttpClient;
        Guid project = projectId;
        int? nullable = new int?(count);
        int? maxCommentLength = new int?();
        int? skip = new int?();
        int? top = nullable;
        CancellationToken cancellationToken = new CancellationToken();
        return tfvcHttpClient1.GetChangesetsAsync(project, maxCommentLength, skip, top, cancellationToken: cancellationToken);
      });
      return (IList<TfvcChangesetRef>) context.ExecuteAsyncAndSyncResult<List<TfvcChangesetRef>>(func);
    }

    public IList<TfvcItem> GetItems(
      IVssRequestContext context,
      Guid projectId,
      string scopePath,
      VersionControlRecursionType recursionLevel,
      TfvcVersionDescriptor versionDescriptor)
    {
      TfvcHttpClient tfvcHttpClient = context.GetClient<TfvcHttpClient>();
      Func<Task<List<TfvcItem>>> func = (Func<Task<List<TfvcItem>>>) (() => tfvcHttpClient.GetItemsAsync(projectId, scopePath, new VersionControlRecursionType?(recursionLevel), new bool?(false), versionDescriptor));
      return (IList<TfvcItem>) context.ExecuteAsyncAndSyncResult<List<TfvcItem>>(func);
    }

    public IList<TfvcItem> GetItems(
      IVssRequestContext context,
      string scopePath,
      VersionControlRecursionType recursionLevel,
      TfvcVersionDescriptor versionDescriptor)
    {
      TfvcHttpClient tfvcHttpClient = context.GetClient<TfvcHttpClient>();
      Func<Task<List<TfvcItem>>> func = (Func<Task<List<TfvcItem>>>) (() => tfvcHttpClient.GetItemsAsync(scopePath, new VersionControlRecursionType?(recursionLevel), new bool?(false), versionDescriptor, (object) null, new CancellationToken()));
      return (IList<TfvcItem>) context.ExecuteAsyncAndSyncResult<List<TfvcItem>>(func);
    }

    public string GetItemContent(IVssRequestContext context, string path)
    {
      string itemContent = (string) null;
      if (!string.IsNullOrEmpty(path))
      {
        TfvcHttpClient tfvcHttpClient = context.GetClient<TfvcHttpClient>();
        Func<Task<Stream>> func = (Func<Task<Stream>>) (() => tfvcHttpClient.GetItemContentAsync(path, (string) null, new bool?(), (string) null, new VersionControlRecursionType?(), (TfvcVersionDescriptor) null, new bool?(), (object) null, new CancellationToken()));
        using (StreamReader streamReader = new StreamReader(context.ExecuteAsyncAndGetResult<Stream>(func)))
          itemContent = streamReader.ReadToEnd();
      }
      return itemContent;
    }
  }
}
