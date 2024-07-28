// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.ViewMetadataOperationApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class ViewMetadataOperationApplier<T, TU> : IMetadataOperationApplier
    where T : IPackageMetadataItemFactory<TU>, new()
    where TU : PackageMetadataItem
  {
    public PackageMetadataItem Apply(
      ICommitLogEntry commitLogEntry,
      PackageMetadataItem currentState)
    {
      if (currentState.PermanentDeletedDate.HasValue)
        return currentState;
      if (!(commitLogEntry.CommitOperationData is ViewOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      T obj1 = new T();
      ref T local = ref obj1;
      if ((object) default (T) == null)
      {
        T obj2 = local;
        local = ref obj2;
      }
      IItemData data = currentState.Data;
      TU metadataItem = local.CreateMetadataItem(data);
      metadataItem.ModifiedBy = commitLogEntry.UserId;
      metadataItem.ModifiedDate = commitLogEntry.CreatedDate;
      if (commitOperationData.MetadataSuboperation != MetadataSuboperation.Add)
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidViewSuboperation());
      metadataItem.Views = (IEnumerable<Guid>) new HashSet<Guid>(metadataItem.Views)
      {
        commitOperationData.ViewId
      };
      return (PackageMetadataItem) metadataItem;
    }
  }
}
