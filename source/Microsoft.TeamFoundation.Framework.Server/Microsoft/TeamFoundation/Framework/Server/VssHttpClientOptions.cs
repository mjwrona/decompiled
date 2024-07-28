// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssHttpClientOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssHttpClientOptions : IEquatable<VssHttpClientOptions>
  {
    public VssReadConsistencyLevel? ReadConsistencyLevel { get; set; }

    public bool Equals(VssHttpClientOptions other)
    {
      if (other == null)
        return false;
      VssReadConsistencyLevel? consistencyLevel1 = this.ReadConsistencyLevel;
      VssReadConsistencyLevel? consistencyLevel2 = other.ReadConsistencyLevel;
      return consistencyLevel1.GetValueOrDefault() == consistencyLevel2.GetValueOrDefault() & consistencyLevel1.HasValue == consistencyLevel2.HasValue;
    }

    public override int GetHashCode() => this.ReadConsistencyLevel.GetHashCode();
  }
}
