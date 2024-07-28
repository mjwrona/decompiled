// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API.CargoPackagesBatchRequestConverter
// Assembly: Microsoft.VisualStudio.Services.Cargo.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79D1C655-766F-4F71-AAEA-7C02E794C2F8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API
{
  public class CargoPackagesBatchRequestConverter : BasePackagesBatchRequestConverter
  {
    private Dictionary<CargoBatchOperationType, Func<BatchOperationData>> supportedTypes = new Dictionary<CargoBatchOperationType, Func<BatchOperationData>>()
    {
      {
        CargoBatchOperationType.Promote,
        (Func<BatchOperationData>) (() => (BatchOperationData) new BatchPromoteData())
      },
      {
        CargoBatchOperationType.Delete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        CargoBatchOperationType.RestoreToFeed,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        CargoBatchOperationType.PermanentDelete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        CargoBatchOperationType.Yank,
        (Func<BatchOperationData>) (() => (BatchOperationData) new BatchListData())
      }
    };

    public override bool IsSupportedOperation(string operationType) => Enum.TryParse<CargoBatchOperationType>(operationType, true, out CargoBatchOperationType _);

    public override IPackagesBatchRequest Create(string operationType)
    {
      CargoBatchOperationType key = (CargoBatchOperationType) Enum.Parse(typeof (CargoBatchOperationType), operationType, true);
      BatchOperationData batchOperationData = this.supportedTypes[key]();
      return (IPackagesBatchRequest) new CargoPackagesBatchRequest()
      {
        Operation = key,
        Data = batchOperationData
      };
    }
  }
}
