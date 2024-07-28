// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Index.ICustomCodeForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Index, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C10BA9-319D-46E2-AA64-F18680226A42
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Index.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Index
{
  public interface ICustomCodeForwarder
  {
    BulkCodeIndexResponse ForwardBulkIndexRequest(
      IVssRequestContext requestContext,
      BulkCodeIndexRequest request);

    string ForwardGetFileContentRequest(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath);

    FilesMetadataResponse ForwardFilesMetadataRequest(
      IVssRequestContext requestContext,
      IEnumerable<IndexInfo> indexInfo,
      string projectName,
      string repositoryName,
      List<string> branchName,
      FilesMetadataRequest filesMetadataRequest,
      DocumentContractType contractType);

    OperationStatus ForwardGetOperationStatusRequest(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string trackingId);
  }
}
