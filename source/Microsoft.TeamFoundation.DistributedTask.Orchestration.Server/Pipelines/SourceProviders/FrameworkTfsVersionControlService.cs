// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.FrameworkTfsVersionControlService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  internal class FrameworkTfsVersionControlService : 
    IPipelineTfsVersionControlService,
    IVssFrameworkService
  {
    public string GetItemContent(IVssRequestContext requestContext, string path, int changesetId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkTfsVersionControlService), nameof (GetItemContent)))
      {
        TfvcVersionDescriptor versionDescriptor1 = new TfvcVersionDescriptor()
        {
          Version = changesetId.ToString(),
          VersionType = TfvcVersionType.Changeset
        };
        TfvcHttpClient client = requestContext.GetClient<TfvcHttpClient>();
        string path1 = path;
        TfvcVersionDescriptor versionDescriptor2 = versionDescriptor1;
        bool? download = new bool?();
        VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?();
        TfvcVersionDescriptor versionDescriptor3 = versionDescriptor2;
        bool? includeContent = new bool?();
        CancellationToken cancellationToken = new CancellationToken();
        using (Stream result = client.GetItemContentAsync(path1, (string) null, download, (string) null, recursionLevel, versionDescriptor3, includeContent, (object) null, cancellationToken).Result)
        {
          using (StreamReader streamReader = new StreamReader((Stream) new LimitReadStream(result, 524288L)))
            return streamReader.ReadToEnd();
        }
      }
    }

    public CommitInfo GetChangeset(
      IVssRequestContext requestContext,
      IList<TfvcMappingFilter> mappings,
      int changesetId)
    {
      TfvcChangesetSearchCriteria changesetSearchCriteria1 = new TfvcChangesetSearchCriteria();
      changesetSearchCriteria1.Mappings.AddRange<TfvcMappingFilter, IList<TfvcMappingFilter>>((IEnumerable<TfvcMappingFilter>) mappings);
      TfvcHttpClient client = requestContext.GetClient<TfvcHttpClient>();
      TfvcHttpClient tfvcHttpClient = client;
      int id = changesetId;
      TfvcChangesetSearchCriteria changesetSearchCriteria2 = changesetSearchCriteria1;
      int? maxChangeCount = new int?();
      bool? includeDetails = new bool?();
      bool? includeWorkItems = new bool?();
      int? maxCommentLength = new int?();
      bool? includeSourceRename = new bool?();
      int? skip = new int?();
      int? top = new int?();
      TfvcChangesetSearchCriteria searchCriteria = changesetSearchCriteria2;
      CancellationToken cancellationToken = new CancellationToken();
      TfvcChangeset tfvcChangeset = tfvcHttpClient.GetChangesetAsync(id, maxChangeCount, includeDetails, includeWorkItems, maxCommentLength, includeSourceRename, skip, top, searchCriteria: searchCriteria, cancellationToken: cancellationToken).Result ?? client.GetChangesetAsync(changesetId).Result;
      if (tfvcChangeset == null)
        return (CommitInfo) null;
      return new CommitInfo()
      {
        CommitId = tfvcChangeset.ChangesetId.ToString(),
        Info = new VersionInfo()
        {
          Author = tfvcChangeset.Author?.DisplayName,
          Message = tfvcChangeset.Comment
        }
      };
    }

    public CommitInfo GetLatestChangeset(
      IVssRequestContext requestContext,
      IList<TfvcMappingFilter> mappings)
    {
      TfvcChangesetSearchCriteria changesetSearchCriteria1 = new TfvcChangesetSearchCriteria();
      changesetSearchCriteria1.Mappings.AddRange<TfvcMappingFilter, IList<TfvcMappingFilter>>((IEnumerable<TfvcMappingFilter>) mappings);
      TfvcHttpClient client = requestContext.GetClient<TfvcHttpClient>();
      TfvcHttpClient tfvcHttpClient1 = client;
      int? nullable = new int?(1);
      TfvcChangesetSearchCriteria changesetSearchCriteria2 = changesetSearchCriteria1;
      int? maxCommentLength1 = new int?();
      int? skip1 = new int?();
      int? top1 = nullable;
      TfvcChangesetSearchCriteria searchCriteria = changesetSearchCriteria2;
      CancellationToken cancellationToken1 = new CancellationToken();
      TfvcChangesetRef tfvcChangesetRef = tfvcHttpClient1.GetChangesetsAsync(maxCommentLength1, skip1, top1, searchCriteria: searchCriteria, cancellationToken: cancellationToken1).Result.FirstOrDefault<TfvcChangesetRef>();
      if (tfvcChangesetRef == null)
      {
        TfvcHttpClient tfvcHttpClient2 = client;
        nullable = new int?(1);
        int? maxCommentLength2 = new int?();
        int? skip2 = new int?();
        int? top2 = nullable;
        CancellationToken cancellationToken2 = new CancellationToken();
        tfvcChangesetRef = tfvcHttpClient2.GetChangesetsAsync(maxCommentLength2, skip2, top2, cancellationToken: cancellationToken2).Result.FirstOrDefault<TfvcChangesetRef>();
      }
      if (tfvcChangesetRef == null)
        return (CommitInfo) null;
      return new CommitInfo()
      {
        CommitId = tfvcChangesetRef.ChangesetId.ToString(),
        Info = new VersionInfo()
        {
          Author = tfvcChangesetRef.Author?.DisplayName,
          Message = tfvcChangesetRef.Comment
        }
      };
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
