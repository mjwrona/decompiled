// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitItemsBatchController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("Items")]
  public class GitItemsBatchController : GitApiController
  {
    [HttpPost]
    [ClientExample("POST__git_repositories__repositoryId__itemsBatch.json", "Multiple items", null, null)]
    [ClientLocationId("630FD2E4-FB88-4F85-AD21-13F3FD1FBCA9")]
    [ClientResponseType(typeof (List<List<GitItem>>), null, null)]
    [PublicProjectRequestRestrictions]
    public IList<GitItemsCollection> GetItemsBatch(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      GitItemRequestData requestData,
      [ClientIgnore] string projectId = null)
    {
      if (requestData == null || requestData.ItemDescriptors == null || requestData.ItemDescriptors.Length < 1)
        throw new ArgumentException(Resources.Get("ErrorPathsNotSpecified")).Expected("git");
      ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId);
      this.AddDisposableResource((IDisposable) tfsGitRepository);
      IList<GitItemsCollection> source = (IList<GitItemsCollection>) new List<GitItemsCollection>();
      ISecuredObject securedObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
      foreach (GitItemDescriptor itemDescriptor in requestData.ItemDescriptors)
      {
        if (string.IsNullOrEmpty(itemDescriptor?.Path))
          throw new ArgumentException(Resources.Format(itemDescriptor == null ? "ErrorDescriptorNull" : "ErrorPathEmpty")).Expected("git");
        GitItemsCollection gitItemsCollection;
        try
        {
          gitItemsCollection = GitItemUtility.RetrieveItemModels(this.TfsRequestContext, this.Url, tfsGitRepository, itemDescriptor, requestData.IncludeContentMetadata, requestData.IncludeLinks);
          if (requestData.LatestProcessedChange)
          {
            if (gitItemsCollection.Any<GitItem>())
              GitItemUtility.PopulateLastChangedCommits(this.TfsRequestContext, this.Url, tfsGitRepository, gitItemsCollection, itemDescriptor.RecursionLevel);
          }
        }
        catch (GitItemVersionException ex)
        {
          if (requestData.ItemDescriptors.Length == 1)
            throw;
          else
            gitItemsCollection = new GitItemsCollection();
        }
        gitItemsCollection.ForEach((Action<GitItem>) (item => item.SetSecuredObject(securedObject)));
        source.Add(gitItemsCollection);
      }
      return !source.All<GitItemsCollection>((Func<GitItemsCollection, bool>) (collection => !collection.Any<GitItem>())) ? source : throw new ArgumentException(Resources.Format("ErrorItemBatchNotFound")).Expected("git");
    }
  }
}
