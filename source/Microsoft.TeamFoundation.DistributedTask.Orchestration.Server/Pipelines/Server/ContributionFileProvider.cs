// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.ContributionFileProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class ContributionFileProvider : IFileProvider
  {
    private readonly IContributionService m_contributionService;
    private readonly IVssRequestContext m_requestContext;
    private readonly string m_contributionId;
    private readonly string m_layer = nameof (ContributionFileProvider);
    private readonly string ContributionMemoryCacheFeatureFlag = "Microsoft.Azure.DevOps.ContributionFileContent.MemoryCacheEnabled";
    private static readonly string s_layer = nameof (ContributionFileProvider);

    public ContributionFileProvider(IVssRequestContext requestContext, string contributionId)
    {
      this.m_requestContext = requestContext;
      this.m_contributionService = this.m_requestContext.GetService<IContributionService>();
      this.m_contributionId = contributionId;
    }

    public RepositoryResource Repository
    {
      get
      {
        RepositoryResource repository = new RepositoryResource();
        repository.Id = string.Empty;
        repository.Alias = string.Empty;
        repository.Endpoint = (ServiceEndpointReference) null;
        repository.Type = string.Empty;
        repository.Url = (Uri) null;
        repository.Version = string.Empty;
        return repository;
      }
    }

    public string GetDirectoryName(string path) => string.Empty;

    public string GetFileContent(string path)
    {
      if (!this.m_requestContext.IsFeatureEnabled(this.ContributionMemoryCacheFeatureFlag))
        return this.ReadFileContentFromURL(path);
      string empty = string.Empty;
      ContributionContentMemoryCacheService service = this.m_requestContext.GetService<ContributionContentMemoryCacheService>();
      if (service.TryGetValue(this.m_requestContext, path, out empty))
      {
        this.m_requestContext.TraceInfo(10016172, ContributionFileProvider.s_layer, "Contribution file content memory cache hit. Key={0}", (object) path);
        return empty;
      }
      this.m_requestContext.TraceInfo(10016173, ContributionFileProvider.s_layer, "Contribution file content memory cache miss. Key={0}", (object) path);
      string fileContent = this.ReadFileContentFromURL(path);
      if (!string.IsNullOrEmpty(fileContent))
        service.Set(this.m_requestContext, path, fileContent);
      return fileContent;
    }

    public string GetFileContentByUuid(string uuid) => string.Empty;

    public string GetFileName(string path) => string.Empty;

    public string GetFileUuid(string path) => string.Empty;

    public string ResolvePath(string defaultRoot, string path) => path;

    private string ReadFileContentFromURL(string path)
    {
      string str = string.Empty;
      using (Stream stream = this.m_contributionService.QueryAsset(this.m_requestContext, this.m_contributionId, path).SyncResult<Stream>())
      {
        if (stream != null)
        {
          using (StreamReader streamReader = new StreamReader(stream))
            str = streamReader.ReadToEnd();
        }
        else
          this.m_requestContext.TraceError(this.m_layer, "File content stream for {0} was null.", (object) path);
      }
      return str;
    }
  }
}
