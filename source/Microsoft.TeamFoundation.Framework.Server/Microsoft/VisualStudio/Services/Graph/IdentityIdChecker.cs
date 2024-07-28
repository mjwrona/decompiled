// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.IdentityIdChecker
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class IdentityIdChecker
  {
    private static readonly HashSet<int> storageKeyVersions = new HashSet<int>()
    {
      4,
      6,
      10,
      11,
      12
    };

    internal static bool IsCuid(Guid guid) => (int) guid.ToByteArray()[7] >> 4 == 7;

    internal static bool IsStorageKey(Guid guid) => IdentityIdChecker.storageKeyVersions.Contains((int) guid.ToByteArray()[7] >> 4);
  }
}
