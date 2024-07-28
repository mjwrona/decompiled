// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.CommonBatchOperationTypes
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  internal static class CommonBatchOperationTypes
  {
    internal static IReadOnlyCollection<IBatchOperationType> Types => (IReadOnlyCollection<IBatchOperationType>) new List<IBatchOperationType>()
    {
      BatchOperationType.Promote,
      BatchOperationType.PermanentDelete,
      BatchOperationType.RestoreToFeed,
      BatchOperationType.Delete
    }.AsReadOnly();
  }
}
