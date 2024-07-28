// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GuidAndString
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public struct GuidAndString : IEquatable<GuidAndString>
  {
    private readonly string m_string;
    private readonly Guid m_guid;
    internal static readonly GuidAndString Empty;

    public GuidAndString(string str, Guid guid)
    {
      this.m_string = str;
      this.m_guid = guid;
    }

    public bool Equals(GuidAndString other) => this.GuidId.Equals(other.GuidId) && string.Equals(this.String, other.String, StringComparison.Ordinal);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0} {1})", (object) this.GuidId, (object) this.String);

    internal string String => this.m_string;

    internal Guid GuidId => this.m_guid;
  }
}
