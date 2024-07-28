// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.PyPiPackagesBatchRequestConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17E1C323-94FE-4FF1-903A-ED51BA3159D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API
{
  public class PyPiPackagesBatchRequestConverter : BasePackagesBatchRequestConverter
  {
    private Dictionary<PyPiBatchOperationType, Func<BatchOperationData>> supportedTypes = new Dictionary<PyPiBatchOperationType, Func<BatchOperationData>>()
    {
      {
        PyPiBatchOperationType.Promote,
        (Func<BatchOperationData>) (() => (BatchOperationData) new BatchPromoteData())
      },
      {
        PyPiBatchOperationType.Delete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        PyPiBatchOperationType.RestoreToFeed,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        PyPiBatchOperationType.PermanentDelete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      }
    };

    public override bool IsSupportedOperation(string operationType) => Enum.TryParse<PyPiBatchOperationType>(operationType, true, out PyPiBatchOperationType _);

    public override IPackagesBatchRequest Create(string operationType)
    {
      PyPiBatchOperationType key = (PyPiBatchOperationType) Enum.Parse(typeof (PyPiBatchOperationType), operationType, true);
      BatchOperationData batchOperationData = this.supportedTypes[key]();
      return (IPackagesBatchRequest) new PyPiPackagesBatchRequest()
      {
        Operation = key,
        Data = batchOperationData
      };
    }
  }
}
