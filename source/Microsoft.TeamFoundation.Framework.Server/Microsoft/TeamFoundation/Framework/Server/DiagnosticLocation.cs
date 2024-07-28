// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticLocation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal struct DiagnosticLocation : IEquatable<DiagnosticLocation>
  {
    private readonly string m_area;
    private readonly string m_layer;
    private readonly string m_methodName;

    public DiagnosticLocation(string area, string layer, string methodName)
    {
      this.m_area = area;
      this.m_layer = layer;
      this.m_methodName = methodName;
    }

    public bool IsInitialized() => this.m_area != null && this.m_layer != null && this.m_methodName != null;

    public bool Equals(DiagnosticLocation other) => string.Equals(this.m_area, other.m_area, StringComparison.OrdinalIgnoreCase) && string.Equals(this.m_layer, other.m_layer, StringComparison.OrdinalIgnoreCase) && string.Equals(this.m_methodName, other.m_methodName, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object obj) => obj is DiagnosticLocation other && this.Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.m_area ?? string.Empty) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.m_layer ?? string.Empty) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.m_methodName ?? string.Empty);

    public override string ToString()
    {
      if (!this.IsInitialized())
        return (string) null;
      return this.m_area + "." + this.m_layer + "." + this.m_methodName;
    }
  }
}
