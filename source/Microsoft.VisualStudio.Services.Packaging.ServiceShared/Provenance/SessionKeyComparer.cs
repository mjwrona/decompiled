// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.SessionKeyComparer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class SessionKeyComparer : IEqualityComparer<SessionKey>
  {
    public static readonly SessionKeyComparer Instance = new SessionKeyComparer();

    public bool Equals(SessionKey x, SessionKey y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && string.Equals(y.Protocol, x.Protocol, StringComparison.OrdinalIgnoreCase) && y.SessionId.Equals(x.SessionId);
    }

    public int GetHashCode(SessionKey obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Protocol) ^ obj.SessionId.GetHashCode();
  }
}
