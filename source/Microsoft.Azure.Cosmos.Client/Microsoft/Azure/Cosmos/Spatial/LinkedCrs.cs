// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.LinkedCrs
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  public sealed class LinkedCrs : Crs, IEquatable<LinkedCrs>
  {
    internal LinkedCrs(string href, string hrefType = null)
      : base(CrsType.Linked)
    {
      this.Href = href != null ? href : throw new ArgumentNullException(nameof (href));
      this.HrefType = hrefType;
    }

    [DataMember(Name = "href")]
    public string Href { get; private set; }

    [DataMember(Name = "hrefType")]
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
