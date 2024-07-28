// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.GitFailedItemsPatchDescriptionCreator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class GitFailedItemsPatchDescriptionCreator : 
    AbstractCodeFailedItemsPatchDescriptionCreator
  {
    private readonly GitHttpClientWrapper m_gitHttpClientWrapper;

    internal GitFailedItemsPatchDescriptionCreator(
      IndexingExecutionContext indexingExecutionContext,
      TraceMetaData traceMetaData)
      : this(new GitHttpClientWrapper((ExecutionContext) indexingExecutionContext, traceMetaData), traceMetaData)
    {
    }

    internal GitFailedItemsPatchDescriptionCreator(
      GitHttpClientWrapper gitHttpClientWrapper,
      TraceMetaData traceMetaData)
      : base(traceMetaData)
    {
      this.m_gitHttpClientWrapper = gitHttpClientWrapper;
    }

    internal override VersionControlType GetVersionControlType() => VersionControlType.Git;

    internal override void CreatePatchDescription(
      IndexingExecutionContext iexContext,
      string filePath,
      string branchName,
      out List<string> recordsToBeAdded,
      out List<string> recordsToBeDeleted,
      out string patchFile)
    {
      recordsToBeAdded = new List<string>();
      recordsToBeDeleted = new List<string>();
      patchFile = (string) null;
      List<string> branchesToIndex = ((GitCodeRepoTFSAttributes) iexContext.RepositoryIndexingUnit.TFSEntityAttributes).BranchesToIndex;
      try
      {
        if (branchesToIndex.Contains(branchName))
        {
          GitCommit latestCommit = this.m_gitHttpClientWrapper.GetLatestCommit(branchName);
          GitVersionDescriptor versionDescriptor = new GitVersionDescriptor()
          {
            Version = latestCommit.CommitId,
            VersionType = GitVersionType.Commit
          };
          bool isDeleted;
          GitItem gitItem1 = this.m_gitHttpClientWrapper.GetItem(filePath, versionDescriptor, out isDeleted);
          if (gitItem1 != null && gitItem1.IsFolder && !isDeleted)
          {
            foreach (GitItem gitItem2 in this.m_gitHttpClientWrapper.GetItemsAsync(versionDescriptor, filePath, VersionControlRecursionType.OneLevel))
            {
              if (gitItem2.Path != gitItem1.Path)
              {
                string fileSystemPath = Helpers.ModifyGitItemPathToFileSystemPath(gitItem2.Path);
                recordsToBeAdded.Add(fileSystemPath);
              }
            }
            recordsToBeDeleted.Add(filePath);
          }
          else
            patchFile = filePath;
        }
        else
          recordsToBeDeleted.Add(filePath);
      }
      catch (Exception ex)
      {
        recordsToBeAdded.Clear();
        recordsToBeDeleted.Clear();
        recordsToBeAdded.Add(filePath);
        Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Exception while creating patch description for filePath: {0}. Exception: {1}", (object) filePath, (object) ex)));
      }
    }
  }
}
