// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.UnspecifiedCrs
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  internal class UnspecifiedCrs : Crs, IEquatable<UnspecifiedCrs>
  {
    public UnspecifiedCrs()
      : base(CrsType.Unspecified)
    {
    }

    public override bool Equals(object obj) => this.Equals(obj as UnspecifiedCrs);

    public override int GetHashCode() => 0;

    public bool Equals(UnspecifiedCrs other)
    {
      if (other == null)
        return false;
      UnspecifiedCrs unspecifiedCrs = this;
      return true;
    }
  }
}
