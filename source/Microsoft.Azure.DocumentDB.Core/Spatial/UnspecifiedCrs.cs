// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.UnspecifiedCrs
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Spatial
{
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
