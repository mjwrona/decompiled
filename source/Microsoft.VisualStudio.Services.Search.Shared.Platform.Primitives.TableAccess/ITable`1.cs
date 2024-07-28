// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ITable`1
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public interface ITable<T> : IDisposable
  {
    T Insert(T entity);

    void Delete(T entity);

    T Update(T entity);

    int CheckAndUpdate(T oldEntity, T newEntity);

    T RetriveTableEntity(TableEntityFilterList filterList);

    List<T> AddTableEntityBatch(List<T> azurePlatformEntityList, bool merge);

    List<T> RetriveTableEntityList(int count, TableEntityFilterList filterList);
  }
}
