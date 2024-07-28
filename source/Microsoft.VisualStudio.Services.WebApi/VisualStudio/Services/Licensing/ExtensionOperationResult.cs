// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionOperationResult
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class ExtensionOperationResult
  {
    public ExtensionOperationResult(
      Guid accountId,
      Guid userId,
      string extensionId,
      ExtensionOperation operation)
    {
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionId, nameof (extensionId));
      this.AccountId = accountId;
      this.UserId = userId;
      this.ExtensionId = extensionId;
      this.Operation = operation;
    }

    public Guid AccountId { get; }

    public Guid UserId { get; set; }

    public string ExtensionId { get; }

    public ExtensionOperation Operation { get; }

    public OperationResult Result { get; set; }

    public string Message { get; set; }

    public override string ToString() => string.Format("{0}|{1}|{2}|{3}|{4}", (object) this.AccountId, (object) this.UserId, (object) this.ExtensionId, (object) this.Message, (object) this.Result.ToString());
  }
}
