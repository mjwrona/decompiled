// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.NpmPackagesBatchRequestConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class NpmPackagesBatchRequestConverter : BasePackagesBatchRequestConverter
  {
    private Dictionary<NpmBatchOperationType, Func<BatchOperationData>> supportedTypes = new Dictionary<NpmBatchOperationType, Func<BatchOperationData>>()
    {
      {
        NpmBatchOperationType.Promote,
        (Func<BatchOperationData>) (() => (BatchOperationData) new BatchPromoteData())
      },
      {
        NpmBatchOperationType.Deprecate,
        (Func<BatchOperationData>) (() => (BatchOperationData) new BatchDeprecateData())
      },
      {
        NpmBatchOperationType.Unpublish,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        NpmBatchOperationType.Delete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        NpmBatchOperationType.RestoreToFeed,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        NpmBatchOperationType.PermanentDelete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        NpmBatchOperationType.UpgradeCachedPackages,
        (Func<BatchOperationData>) (() => (BatchOperationData) new NpmBatchUpgradeData())
      }
    };

    public override bool IsSupportedOperation(string operation) => Enum.TryParse<NpmBatchOperationType>(operation, true, out NpmBatchOperationType _);

    public override IPackagesBatchRequest Create(string operationType)
    {
      NpmBatchOperationType key = (NpmBatchOperationType) Enum.Parse(typeof (NpmBatchOperationType), operationType, true);
      BatchOperationData batchOperationData = this.supportedTypes[key]();
      return (IPackagesBatchRequest) new NpmPackagesBatchRequest()
      {
        Operation = key,
        Data = batchOperationData
      };
    }
  }
}
