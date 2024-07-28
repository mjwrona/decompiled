// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IdAndRev
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class IdAndRev : IIdAndRevBase
  {
    public IdAndRev()
    {
    }

    public IdAndRev(int id, int revision)
    {
      this.Id = id;
      this.Revision = revision;
    }

    [XmlAttribute]
    public int Id { get; set; }

    [XmlAttribute]
    public int Revision { get; set; }

    public override bool Equals(object obj) => IdAndRev.Equals(this, obj as IdAndRev);

    public override int GetHashCode()
    {
      int num = this.Id;
      int hashCode1 = num.GetHashCode();
      num = this.Revision;
      int hashCode2 = num.GetHashCode();
      return hashCode1 ^ hashCode2;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0} {1})", (object) this.Id, (object) this.Revision);

    public static bool Equals(IdAndRev left, IdAndRev right)
    {
      if (left == null && right == null)
        return true;
      return left != null && right != null && left.Id == right.Id && left.Revision == right.Revision;
    }
  }
}
