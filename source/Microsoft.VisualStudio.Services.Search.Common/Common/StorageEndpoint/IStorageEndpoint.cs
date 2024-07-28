// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint.IStorageEndpoint
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint
{
  public interface IStorageEndpoint : IEnumerable<IObjectStoreItem>, IEnumerable
  {
    bool CanAdd { get; }

    bool CanGet { get; }

    void Add(ContentId contentId, byte[] blob);

    bool Contains(ContentId contentId);

    bool TryGet(ContentId contentId, out IObjectStoreItem item);
  }
}
