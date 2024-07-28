// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionOperationResultInternal
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class ExtensionOperationResultInternal
  {
    public Guid AccountId { get; }

    public Guid UserId { get; }

    public string ExtensionId { get; }

    public ExtensionOperation Operation { get; }

    public ExtensionAssignmentStatus Status { get; }

    public OperationResult Result { get; }

    public string Message { get; }

    public ExtensionOperationResultInternal(
      Guid accountId,
      Guid userId,
      string extensionId,
      ExtensionOperation operation,
      ExtensionAssignmentStatus status,
      OperationResult result,
      string message)
    {
      this.AccountId = accountId;
      this.UserId = userId;
      this.ExtensionId = extensionId;
      this.Operation = operation;
      this.Status = status;
      this.Result = result;
      this.Message = message;
    }

    public ExtensionOperationResult ToExtensionOperationResult() => new ExtensionOperationResult(this.AccountId, this.UserId, this.ExtensionId, this.Operation)
    {
      Result = this.Result,
      Message = this.Message
    };
  }
}
