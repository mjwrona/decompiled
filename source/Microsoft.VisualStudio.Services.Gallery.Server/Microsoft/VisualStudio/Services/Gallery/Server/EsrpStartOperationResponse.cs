// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.EsrpStartOperationResponse
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class EsrpStartOperationResponse
  {
    public EsrpOperationStatus OperationStatus { get; }

    public string OperationId { get; }

    public string ErrorCode { get; }

    public string ErrorDetail { get; }

    public EsrpStartOperationResponse(
      EsrpOperationStatus operationStatus,
      string operationId,
      string errorCode,
      string errorDetail)
    {
      this.OperationStatus = operationStatus;
      this.OperationId = operationId;
      this.ErrorCode = errorCode;
      this.ErrorDetail = errorDetail;
    }
  }
}
