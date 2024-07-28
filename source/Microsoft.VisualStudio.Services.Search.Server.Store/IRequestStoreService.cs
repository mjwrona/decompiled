// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Store.IRequestStoreService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Store, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 88C5F689-5CBE-419A-B234-7228E63B94DF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Store.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Search.Server.Store
{
  public interface IRequestStoreService
  {
    string AddRequest<T>(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      T requestContent,
      string branchName)
      where T : class;

    T GetRequest<T>(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string requestId,
      string branchName)
      where T : class;

    bool RequestExists(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string requestId,
      string branchName);

    void DeleteRequest(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string requestId,
      string branchName);
  }
}
