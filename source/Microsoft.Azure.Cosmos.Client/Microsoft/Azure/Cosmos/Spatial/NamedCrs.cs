// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.NamedCrs
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  public sealed class NamedCrs : Crs, IEquatable<NamedCrs>
  {
    internal NamedCrs(string name)
      : base(CrsType.Named)
    {
      this.Name = name != null ? name : throw new ArgumentNullException(nameof (name));
    }

    [DataMember(Name = "name")]
    public string Name { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as NamedCrs);

    public override int GetHashCode() => this.Name.GetHashCode();

    public bool Equals(NamedCrs other)
    {
      if (other == null)
        return false;
      return this == other || string.Equals(this.Name, other.Name);
    }
  }
}
