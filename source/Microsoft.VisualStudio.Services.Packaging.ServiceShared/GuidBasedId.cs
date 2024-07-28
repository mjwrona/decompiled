// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.GuidBasedId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class GuidBasedId
  {
    public GuidBasedId(Guid guid)
    {
      ArgumentUtility.CheckForDefault<Guid>(guid, nameof (guid));
      this.Guid = guid;
    }

    public Guid Guid { get; }

    protected bool Equals(GuidBasedId other) => other != (GuidBasedId) null && this.Guid == other.Guid;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((GuidBasedId) obj);
    }

    public override int GetHashCode() => this.Guid.GetHashCode();

    public static bool operator ==(GuidBasedId a, GuidBasedId b)
    {
      if ((object) a != null)
        return a.Equals(b);
      return (object) b == null;
    }

    public static bool operator !=(GuidBasedId a, GuidBasedId b) => !(a == b);

    public override string ToString() => this.Guid.ToString();
  }
}
