// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IdAndString
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public struct IdAndString : IEquatable<IdAndString>
  {
    private readonly string m_string;
    private readonly int m_id;
    internal static readonly IdAndString Empty;

    internal IdAndString(string str, int id)
    {
      this.m_string = str;
      this.m_id = id;
    }

    public bool Equals(IdAndString other) => this.Id == other.Id && string.Equals(this.String, other.String, StringComparison.Ordinal);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0} {1})", (object) this.Id, (object) this.String);

    internal string String => this.m_string;

    internal int Id => this.m_id;
  }
}
