// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.CaseInsensitiveTupleComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class CaseInsensitiveTupleComparer : IEqualityComparer<Tuple<string, string>>
  {
    public bool Equals(Tuple<string, string> lhs, Tuple<string, string> rhs) => StringComparer.OrdinalIgnoreCase.Equals(lhs.Item1, rhs.Item1) && StringComparer.OrdinalIgnoreCase.Equals(lhs.Item2, rhs.Item2);

    public int GetHashCode(Tuple<string, string> tuple) => StringComparer.OrdinalIgnoreCase.GetHashCode(tuple.Item1) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(tuple.Item2);

    public static CaseInsensitiveTupleComparer Instance { get; } = new CaseInsensitiveTupleComparer();
  }
}
