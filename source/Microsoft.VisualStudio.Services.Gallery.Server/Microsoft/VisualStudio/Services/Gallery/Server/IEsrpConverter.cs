// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IEsrpConverter
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using MS.Ess.EsrpClient.Contracts.Common;
using MS.Ess.EsrpClient.Contracts.Sign.V1;
using MS.Ess.EsrpClient.Contracts.Sign.V1.Response;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public interface IEsrpConverter
  {
    StaticSigningData CreateSignRequest(IReadOnlyCollection<EsrpSignRequest> requests);

    SignStatusRequest CreateGetSignStatusRequest(
      IReadOnlyCollection<EsrpGetOperationStatusRequest> requests);

    IReadOnlyList<EsrpStartOperationResponse> CreateEsrpSignResponse(
      SignBatchSubmissionResponse response);

    IReadOnlyList<EsrpGetOperationStatusResponse> CreateGetSignStatusResponse(
      SignBatchCompletionResponse response);

    EsrpOperationStatus ConvertEsrpOperationStatus(StatusCode statusCode);
  }
}
