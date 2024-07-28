// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.PlatformTfsVersionControlService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Tfs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3EB20FA-6669-4C21-BA19-EC9C2EBF5243
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Tfs.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  internal class PlatformTfsVersionControlService : 
    IPipelineTfsVersionControlService,
    IVssFrameworkService
  {
    public string GetItemContent(IVssRequestContext requestContext, string path, int changesetId)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsVersionControlService), nameof (GetItemContent)))
      {
        using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryFileContents(requestContext, path, 0, (VersionSpec) new ChangesetVersionSpec(changesetId)))
        {
          using (StreamReader streamReader = new StreamReader((Stream) new LimitReadStream(foundationDataReader.Current<Stream>(), 524288L)))
            return streamReader.ReadToEnd();
        }
      }
    }

    public CommitInfo GetChangeset(
      IVssRequestContext requestContext,
      IList<TfvcMappingFilter> mappings,
      int changesetId)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsVersionControlService), nameof (GetChangeset)))
      {
        TeamFoundationVersionControlService service1 = requestContext.GetService<TeamFoundationVersionControlService>();
        IdentityService service2 = requestContext.GetService<IdentityService>();
        IVssRequestContext requestContext1 = requestContext;
        int[] changesets = new int[1]{ changesetId };
        Changeset changeset = service1.QueryChangesets(requestContext1, changesets).FirstOrDefault<Changeset>();
        if (changeset == null)
          return (CommitInfo) null;
        return new CommitInfo()
        {
          Info = new VersionInfo()
          {
            Author = service2.GetIdentity(requestContext, changeset.OwnerId)?.DisplayName,
            Message = changeset.Comment
          },
          CommitId = changeset.ChangesetId.ToString()
        };
      }
    }

    public CommitInfo GetLatestChangeset(
      IVssRequestContext requestContext,
      IList<TfvcMappingFilter> mappings)
    {
      using (new MethodScope(requestContext, nameof (PlatformTfsVersionControlService), nameof (GetLatestChangeset)))
      {
        TeamFoundationVersionControlService service1 = requestContext.GetService<TeamFoundationVersionControlService>();
        IdentityService service2 = requestContext.GetService<IdentityService>();
        Changeset changeset = service1.QueryChangesetRange(requestContext, (IEnumerable<TfvcMappingFilter>) mappings, (VersionSpec) null, (VersionSpec) null, 1, false).FirstOrDefault<Changeset>();
        if (changeset == null)
        {
          int latestChangeset = service1.GetLatestChangeset(requestContext);
          changeset = service1.QueryChangesets(requestContext, new int[1]
          {
            latestChangeset
          }).FirstOrDefault<Changeset>();
        }
        if (changeset == null)
          return (CommitInfo) null;
        return new CommitInfo()
        {
          Info = new VersionInfo()
          {
            Author = service2.GetIdentity(requestContext, changeset.OwnerId)?.DisplayName,
            Message = changeset.Comment
          },
          CommitId = changeset.ChangesetId.ToString()
        };
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
