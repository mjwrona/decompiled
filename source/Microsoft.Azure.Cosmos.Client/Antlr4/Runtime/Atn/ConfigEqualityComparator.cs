// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ConfigEqualityComparator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class ConfigEqualityComparator : IEqualityComparer<ATNConfig>
  {
    public int GetHashCode(ATNConfig o) => 31 * (31 * (31 * 7 + o.state.stateNumber) + o.alt) + o.semanticContext.GetHashCode();

    public bool Equals(ATNConfig a, ATNConfig b)
    {
      if (a == b)
        return true;
      return a != null && b != null && a.state.stateNumber == b.state.stateNumber && a.alt == b.alt && a.semanticContext.Equals((object) b.semanticContext);
    }
  }
}
