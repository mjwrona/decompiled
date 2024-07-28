// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.SourceProviderBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  public abstract class SourceProviderBase : ISourceProvider, ICachingSourceProvider
  {
    public abstract SourceProviderAttributes Attributes { get; }

    public virtual string GetDirectoryName(string path)
    {
      path = path ?? string.Empty;
      path = path.Replace('\\', '/');
      bool flag = path.StartsWith("/");
      string[] source = path.Split(new char[1]{ '/' }, StringSplitOptions.RemoveEmptyEntries);
      string directoryName = string.Join("/", ((IEnumerable<string>) source).Take<string>(source.Length - 1));
      if (flag && !directoryName.StartsWith("/"))
        directoryName = "/" + directoryName;
      return directoryName;
    }

    public virtual string GetFileName(string path)
    {
      path = path?.Replace('\\', '/');
      return ((IEnumerable<string>) (path ?? string.Empty).Split('/', '\\')).Last<string>();
    }

    public virtual string ResolvePath(string defaultRoot, string path)
    {
      path = path ?? string.Empty;
      path = path.Replace('\\', '/');
      if (!path.StartsWith("/"))
        path = (defaultRoot ?? string.Empty) + "/" + path;
      string[] strArray = path.Split(new char[1]{ '/' }, StringSplitOptions.RemoveEmptyEntries);
      Stack<string> values = new Stack<string>(strArray.Length);
      int num = 0;
      for (int index = strArray.Length - 1; index >= 0; --index)
      {
        string a = strArray[index];
        if (string.Equals(a, "..", StringComparison.Ordinal))
          ++num;
        else if (num > 0)
          --num;
        else if (!string.Equals(a, ".", StringComparison.Ordinal))
          values.Push(a);
      }
      if (num > 0)
        throw new DistributedTaskException("The file path " + path + " is invalid");
      return "/" + string.Join("/", (IEnumerable<string>) values);
    }

    public virtual string GetFileContentByUuid(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      ServiceEndpoint endpoint,
      string uuid)
    {
      return string.Empty;
    }

    public virtual string GetFileUuid(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      ServiceEndpoint endpoint,
      string path)
    {
      return string.Empty;
    }

    public abstract string GetFileContent(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      ServiceEndpoint endpoint,
      string path);

    public abstract void ResolveVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      ServiceEndpoint endpoint);

    public abstract void PopulateJobData(
      IVssRequestContext requestContext,
      JobExecutionContext context,
      RepositoryResource repository,
      ServiceEndpoint endpoint);

    public virtual void Validate(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      ServiceEndpoint endpoint)
    {
      this.ValidateType(repository);
    }

    protected void ValidateEndpoint(
      RepositoryResource repository,
      ServiceEndpoint endpoint,
      string expectedType)
    {
      if (repository.Endpoint == null)
        throw new ArgumentException("Repository " + repository.Alias + " requires an endpoint", nameof (endpoint));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(endpoint, nameof (endpoint));
      if (!string.Equals(endpoint.Type, expectedType, StringComparison.OrdinalIgnoreCase))
        throw new ResourceValidationException(string.Format("Repository {0} specifies endpoint {1} which is of type {2} but an endpoint of type {3} is required", (object) repository.Alias, (object) repository.Endpoint, (object) endpoint.Type, (object) expectedType), nameof (endpoint));
    }

    protected void ValidateType(RepositoryResource repository)
    {
      if (!string.Equals(repository?.Type, this.Attributes.Name, StringComparison.OrdinalIgnoreCase))
        throw new NotSupportedException("Incorrect repository type " + repository?.Type + " provided to source provider " + this.Attributes.Name);
    }
  }
}
