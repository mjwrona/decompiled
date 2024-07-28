// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Dataspace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class Dataspace
  {
    public static Dataspace Clone(Dataspace dataspace) => new Dataspace()
    {
      DataspaceIdentifier = dataspace.DataspaceIdentifier,
      DatabaseId = dataspace.DatabaseId,
      DataspaceCategory = dataspace.DataspaceCategory,
      DataspaceId = dataspace.DataspaceId,
      State = dataspace.State
    };

    public override int GetHashCode() => (((17 * 23 + this.DataspaceIdentifier.GetHashCode()) * 23 + this.DataspaceCategory.GetHashCode()) * 23 + this.DatabaseId.GetHashCode()) * 23 + this.DataspaceId.GetHashCode();

    public override bool Equals(object obj) => obj != null && obj is Dataspace dataspace && dataspace.DataspaceIdentifier.Equals(this.DataspaceIdentifier) && dataspace.DataspaceCategory.Equals(this.DataspaceCategory, StringComparison.OrdinalIgnoreCase) && dataspace.DataspaceId == this.DataspaceId && dataspace.DatabaseId == this.DatabaseId;

    [XmlElement]
    public Guid DataspaceIdentifier { get; set; }

    [XmlElement]
    public string DataspaceCategory { get; set; }

    [XmlElement]
    public int DatabaseId { get; set; }

    [XmlElement]
    public int DataspaceId { get; set; }

    [XmlElement]
    public DataspaceState State { get; set; }
  }
}
