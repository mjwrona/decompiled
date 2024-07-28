// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.EsrpGetOperationStatusResponse
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class EsrpGetOperationStatusResponse
  {
    public readonly string OperationId;
    public readonly EsrpOperationStatus OperationStatus;
    public readonly string ErrorCode;
    public readonly string ErrorDetail;
    public readonly string SignedFileUrl;

    public EsrpGetOperationStatusResponse(
      string operationId,
      EsrpOperationStatus operationStatus,
      string errorCode,
      string errorDetail,
      string signedFileUrl)
    {
      this.OperationId = operationId;
      this.OperationStatus = operationStatus;
      this.ErrorCode = errorCode;
      this.ErrorDetail = errorDetail;
      this.SignedFileUrl = signedFileUrl;
    }
  }
}
