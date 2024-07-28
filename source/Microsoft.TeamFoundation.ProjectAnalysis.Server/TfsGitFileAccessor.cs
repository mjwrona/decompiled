// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.TfsGitFileAccessor
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public sealed class TfsGitFileAccessor : 
    TfsFileAccessorBase<TfsGitRepositoryDescriptor>,
    IFileAccessor
  {
    private const string c_layer = "TfsGitFileAccessor";
    private const string c_gitAttributesFile = ".gitattributes";
    public const int c_batchSize = 50000;

    public TfsGitFileAccessor(
      IVssRequestContext requestContext,
      TfsGitRepositoryDescriptor repositoryDescriptor)
      : base(requestContext, repositoryDescriptor)
    {
    }

    public bool IsValid() => !string.IsNullOrWhiteSpace(this.m_repositoryDescriptor.Branch) && this.m_repositoryDescriptor.CommitId.HasValue;

    public Stream GetContent(string filePath)
    {
      ArgumentUtility.CheckForNull<string>(filePath, nameof (filePath));
      if (!this.IsValid())
        return (Stream) null;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository())
      {
        GitItemDescriptor itemDescriptor = new GitItemDescriptor()
        {
          Path = filePath,
          VersionType = GitVersionType.Commit,
          Version = this.m_repositoryDescriptor.CommitId.ToString(),
          RecursionLevel = VersionControlRecursionType.None
        };
        Exception exception;
        try
        {
          GitItem gitItem = GitItemUtility.RetrieveItemModel(this.TfsRequestContext, (UrlHelper) null, tfsGitRepository, itemDescriptor);
          if (gitItem != null)
          {
            Sha1Id objectId = new Sha1Id(gitItem.ObjectId);
            return GitFileUtility.GetFileContentStream(tfsGitRepository, objectId);
          }
          exception = (Exception) new FileNotFoundException(ProjectAnalysisResources.Format("GitFilePathNotFound", (object) filePath, (object) tfsGitRepository, (object) this.m_repositoryDescriptor.CommitId));
        }
        catch (GitItemNotFoundException ex)
        {
          exception = (Exception) ex;
        }
        if (exception != null)
          this.TfsRequestContext.Trace(15281505, TraceLevel.Verbose, "ProjectAnalysisService", nameof (TfsGitFileAccessor), exception.Message);
        return (Stream) null;
      }
    }

    protected override void InitializeDescriptor()
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository())
      {
        TfsGitRef defaultBranch = this.GetDefaultBranch(tfsGitRepository);
        this.m_repositoryDescriptor.Name = tfsGitRepository.Name;
        this.m_repositoryDescriptor.Branch = defaultBranch?.Name;
        this.m_repositoryDescriptor.CommitId = defaultBranch?.ObjectId;
      }
    }

    private TfsGitRef GetDefaultBranch(ITfsGitRepository repo)
    {
      TfsGitRef defaultBranch = repo.Refs?.GetDefault();
      if (defaultBranch != null)
        return defaultBranch;
      this.TfsRequestContext.Trace(15281503, TraceLevel.Info, "ProjectAnalysisService", nameof (TfsGitFileAccessor), new NoDefaultBranchException(this.RepositoryDescriptor.Id).Message);
      return (TfsGitRef) null;
    }

    public IEnumerable<string> GetFilePaths() => this.GetGitBlobEntries().Select<TreeEntryAndPath, string>((Func<TreeEntryAndPath, string>) (x => x.RelativePath));

    internal IEnumerable<TreeEntryAndPath> GetGitBlobEntries(int depth = 100)
    {
      TfsGitFileAccessor tfsGitFileAccessor = this;
      using (ITfsGitRepository repo = tfsGitFileAccessor.GetTfsGitRepository())
      {
        foreach (TreeEntryAndPath gitBlobEntry in repo.LookupObject<TfsGitCommit>(tfsGitFileAccessor.m_repositoryDescriptor.CommitId.Value).GetTree().GetTreeEntriesRecursive(depth).Where<TreeEntryAndPath>((Func<TreeEntryAndPath, bool>) (x => x.Entry.ObjectType == GitObjectType.Blob)))
          yield return gitBlobEntry;
      }
    }

    private ITfsGitRepository GetTfsGitRepository() => this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(this.TfsRequestContext, this.RepositoryDescriptor.Id) ?? throw new RepositoryNotFoundException(this.RepositoryDescriptor.Id);

    public ILanguageConfigurator GetConfigurator()
    {
      try
      {
        using (Stream content = this.GetContent(".gitattributes"))
        {
          if (content != null)
            return (ILanguageConfigurator) new TfsGitAttributesLanguageConfigurator(this.TfsRequestContext, content);
        }
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(15281509, "ProjectAnalysisService", nameof (TfsGitFileAccessor), ex);
      }
      return (ILanguageConfigurator) new DefaultLanguageConfigurator(this.TfsRequestContext);
    }
  }
}
