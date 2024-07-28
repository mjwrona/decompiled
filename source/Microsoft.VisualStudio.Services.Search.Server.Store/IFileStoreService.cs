// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Store.IFileStoreService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Store, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 88C5F689-5CBE-419A-B234-7228E63B94DF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Store.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Search.Server.Store
{
  public interface IFileStoreService
  {
    void AddFile(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath,
      byte[] fileContent);

    string GetFile(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath);

    bool FileExists(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath);

    void DeleteFile(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath);
  }
}
