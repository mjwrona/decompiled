// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PatchSpec
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal readonly struct PatchSpec
  {
    public PatchSpec(
      IReadOnlyList<PatchOperation> patchOperations,
      Either<PatchItemRequestOptions, TransactionalBatchPatchItemRequestOptions> requestOptions)
    {
      List<PatchOperation> patchOperationList = patchOperations != null ? new List<PatchOperation>(patchOperations.Count) : throw new ArgumentOutOfRangeException("Patch Operations cannot be null.");
      foreach (PatchOperation patchOperation in (IEnumerable<PatchOperation>) patchOperations)
        patchOperationList.Add(patchOperation);
      this.PatchOperations = (IReadOnlyList<PatchOperation>) patchOperationList;
      this.RequestOptions = requestOptions;
    }

    public IReadOnlyList<PatchOperation> PatchOperations { get; }

    public Either<PatchItemRequestOptions, TransactionalBatchPatchItemRequestOptions> RequestOptions { get; }
  }
}
