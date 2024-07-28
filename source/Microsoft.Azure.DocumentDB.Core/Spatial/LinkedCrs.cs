// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.LinkedCrs
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Spatial
{
  public sealed class LinkedCrs : Crs, IEquatable<LinkedCrs>
  {
    internal LinkedCrs(string href, string hrefType = null)
      : base(CrsType.Linked)
    {
      this.Href = href != null ? href : throw new ArgumentNullException(nameof (href));
      this.HrefType = hrefType;
    }

    public string Href { get; private set; }

    public string HrefType { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as LinkedCrs);

    public override int GetHashCode() => (this.Href != null ? this.Href.GetHashCode() : 0) * 397 ^ (this.HrefType != null ? this.HrefType.GetHashCode() : 0);

    public bool Equals(LinkedCrs other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return string.Equals(this.Href, other.Href) && string.Equals(this.HrefType, other.HrefType);
    }
  }
}
