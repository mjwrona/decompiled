// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestLogClientStore
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ITestLogClientStore
  {
    Task<TestLogStatus> UploadAsync(
      IVssRequestContext requestContext,
      string logFileSourcePath,
      string blobName,
      bool allowDuplicate,
      Dictionary<string, string> metaData,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogUploadOptions testLogUploadOptions,
      CancellationToken cancellationToken);

    Task<TestLogStatus> UploadAsync(
      IVssRequestContext requestContext,
      Stream stream,
      string blobName,
      bool allowDuplicate,
      Dictionary<string, string> metaData,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogUploadOptions testLogUploadOptions,
      CancellationToken cancellationToken);

    Task<TestLogStatus> DownloadAsync(
      IVssRequestContext requestContext,
      string blobName,
      Stream stream,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogDownloadOptions testLogDownloadOptions,
      CancellationToken cancellationToken);
  }
}
