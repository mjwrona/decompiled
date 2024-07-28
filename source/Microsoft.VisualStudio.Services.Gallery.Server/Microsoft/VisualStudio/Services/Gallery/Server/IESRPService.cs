// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IESRPService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using MS.Ess.EsrpClient.Contracts.Common;
using MS.Ess.EsrpClient.Contracts.Scan.V1;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (ESRPService))]
  internal interface IESRPService : IVssFrameworkService
  {
    FileInfo PrepareStreamForScan(
      IVssRequestContext requestContext,
      Stream stream,
      Guid scanId,
      string fileName);

    void CleanupBlobstore(IVssRequestContext requestContext, string blobURL);

    ScanBatchSubmissionResponse SubmitScan(
      IVssRequestContext requestContext,
      List<FileInfo> filesToScan);

    ScanBatchCompletionResponse GetScanStatus(
      IVssRequestContext requestContext,
      List<Guid> operationIds);

    IReadOnlyList<EsrpStartOperationResponse> SubmitFilesForSigning(
      IVssRequestContext requestContext,
      IReadOnlyList<EsrpSignRequest> requestData);

    IReadOnlyList<EsrpGetOperationStatusResponse> GetSignStatus(
      IVssRequestContext requestContext,
      IReadOnlyList<string> operationIds);

    bool IsScanTimeOut(
      IVssRequestContext requestContext,
      DateTime startTime,
      StatusCode currentStatus);

    TimeSpan GetRecheckTimespan(IVssRequestContext requestContext, StatusCode currentStatus);

    bool IsSigningTimeOut(
      IVssRequestContext requestContext,
      DateTimeOffset startTime,
      EsrpOperationStatus currentStatus);

    TimeSpan GetRecheckTimespanForSigning(
      IVssRequestContext requestContext,
      StatusCode currentStatus);

    string UploadFileToBlobStore(
      IVssRequestContext requestContext,
      string filePath,
      string blobName,
      Guid scanId);

    bool DoesFileExistInBlobStore(IVssRequestContext requestContext, string fileName, Guid scanId);

    string GetSigningKeyCode();
  }
}
