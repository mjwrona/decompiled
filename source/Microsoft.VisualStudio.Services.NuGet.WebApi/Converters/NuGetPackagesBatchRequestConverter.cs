// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Converters.NuGetPackagesBatchRequestConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.NuGet.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Converters
{
  public class NuGetPackagesBatchRequestConverter : BasePackagesBatchRequestConverter
  {
    private Dictionary<NuGetBatchOperationType, Func<BatchOperationData>> supportedTypes = new Dictionary<NuGetBatchOperationType, Func<BatchOperationData>>()
    {
      {
        NuGetBatchOperationType.Promote,
        (Func<BatchOperationData>) (() => (BatchOperationData) new BatchPromoteData())
      },
      {
        NuGetBatchOperationType.List,
        (Func<BatchOperationData>) (() => (BatchOperationData) new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types.BatchListData())
      },
      {
        NuGetBatchOperationType.Delete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        NuGetBatchOperationType.RestoreToFeed,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        NuGetBatchOperationType.PermanentDelete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      }
    };

    public override bool IsSupportedOperation(string operationType) => Enum.TryParse<NuGetBatchOperationType>(operationType, true, out NuGetBatchOperationType _);

    public override IPackagesBatchRequest Create(string operationType)
    {
      NuGetBatchOperationType key = (NuGetBatchOperationType) Enum.Parse(typeof (NuGetBatchOperationType), operationType, true);
      BatchOperationData batchOperationData = this.supportedTypes[key]();
      return (IPackagesBatchRequest) new NuGetPackagesBatchRequest()
      {
        Operation = key,
        Data = batchOperationData
      };
    }
  }
}
