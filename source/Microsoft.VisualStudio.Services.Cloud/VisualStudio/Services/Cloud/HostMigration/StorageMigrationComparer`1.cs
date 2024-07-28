// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.StorageMigrationComparer`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  internal class StorageMigrationComparer<T> : IComparer<T> where T : IStorageMigration
  {
    internal StorageMigrationComparer()
    {
    }

    public int Compare(T x, T y)
    {
      int num1 = x.StorageType - y.StorageType;
      if (num1 != 0)
        return num1;
      if (!x.IsSharded && !y.IsSharded)
        return x.Id.CompareTo(y.Id);
      if (!x.IsSharded && y.IsSharded)
        return -1;
      if (x.IsSharded && !y.IsSharded)
        return 1;
      int num2 = x.ShardIndex.GetValueOrDefault() - y.ShardIndex.GetValueOrDefault();
      if (num2 != 0)
        return num2;
      if (x.VsoArea != null && y.VsoArea != null)
      {
        int num3 = x.VsoArea.CompareTo(y.VsoArea);
        if (num3 != 0)
          return num3;
      }
      if ((object) x == (object) y)
        return 0;
      throw new InvalidOperationException("The storage migration items are either equivalent or not comparable. This will result in undefined order of subjob during parallel migration.");
    }
  }
}
