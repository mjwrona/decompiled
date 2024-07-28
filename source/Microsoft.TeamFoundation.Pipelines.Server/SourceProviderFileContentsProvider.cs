// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.SourceProviderFileContentsProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class SourceProviderFileContentsProvider : IFileContentsProvider
  {
    private readonly Guid m_projectId;
    private readonly string m_repositoryType;
    private readonly string m_repository;
    private readonly Guid? m_connectionId;
    private readonly string m_branch;

    public SourceProviderFileContentsProvider(
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(repositoryType, nameof (repositoryType));
      ArgumentUtility.CheckStringForNullOrEmpty(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrEmpty(branch, nameof (branch));
      this.m_projectId = projectId;
      this.m_repositoryType = repositoryType;
      this.m_repository = repository;
      this.m_connectionId = connectionId;
      this.m_branch = branch;
    }

    public string GetFileContents(IVssRequestContext requestContext, string filePath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(filePath, nameof (filePath));
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, this.m_repositoryType);
      try
      {
        return sourceProvider.GetFileContent(requestContext, this.m_projectId, this.m_connectionId, this.m_repository, this.m_branch, filePath);
      }
      catch (ExternalSourceProviderException ex)
      {
        throw new FileContentsProviderException(PipelinesResources.FileContentProviderFailed((object) filePath, (object) this.m_repositoryType, (object) this.m_repository, (object) this.m_branch, (object) ex.Message), (Exception) ex);
      }
    }
  }
}
