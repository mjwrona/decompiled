// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.JustInTimeFileProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class JustInTimeFileProvider : IFileProvider
  {
    private readonly string m_layer = nameof (JustInTimeFileProvider);
    private IReadOnlyList<ConfigurationFile> m_files;
    private readonly IVssRequestContext m_requestContext;

    public JustInTimeFileProvider(
      IVssRequestContext requestContext,
      IReadOnlyList<ConfigurationFile> files,
      RepositoryResource repository)
    {
      ArgumentUtility.CheckForNull<IReadOnlyList<ConfigurationFile>>(files, nameof (files));
      ArgumentUtility.CheckForNull<RepositoryResource>(repository, nameof (repository));
      this.m_files = files;
      this.m_requestContext = requestContext;
      this.Repository = repository;
    }

    public RepositoryResource Repository { get; }

    public string GetDirectoryName(string path)
    {
      int length = path.LastIndexOf('/');
      return length < 0 ? string.Empty : path.Substring(0, length);
    }

    public string GetFileContent(string path)
    {
      ConfigurationFile configurationFile = this.m_files.FirstOrDefault<ConfigurationFile>((Func<ConfigurationFile, bool>) (f => string.Equals(f.Filename, path, StringComparison.OrdinalIgnoreCase)));
      if (configurationFile != null)
        return configurationFile.Contents;
      this.m_requestContext.TraceError(this.m_layer, "Could not find file '{0}'", (object) path);
      return string.Empty;
    }

    public string GetFileContentByUuid(string uuid) => string.Empty;

    public string GetFileName(string path)
    {
      int num = path.LastIndexOf('/');
      if (num < 0)
        return path;
      return num + 1 > path.Length ? string.Empty : path.Substring(num + 1);
    }

    public string GetFileUuid(string path) => string.Empty;

    public string ResolvePath(string defaultRoot, string path) => path;
  }
}
