// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ICheckinItemManager
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal interface ICheckinItemManager : IEnumerable<ItemPathPair>, IEnumerable
  {
    void Enqueue(ItemPathPair item);

    void Flush(bool isLastPage);

    int ParameterId { get; }

    int TotalCount { get; }

    List<ItemPathPair> FirstPage { get; }
  }
}
