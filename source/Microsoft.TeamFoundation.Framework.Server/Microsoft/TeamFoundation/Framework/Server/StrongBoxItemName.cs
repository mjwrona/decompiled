// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxItemName
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct StrongBoxItemName : IEquatable<StrongBoxItemName>
  {
    [XmlElement]
    public Guid DrawerId { get; set; }

    [XmlElement]
    public string LookupKey { get; set; }

    public bool Equals(StrongBoxItemName other) => this.DrawerId == other.DrawerId && StringComparer.OrdinalIgnoreCase.Equals(this.LookupKey, other.LookupKey);

    public override int GetHashCode() => this.DrawerId.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.LookupKey);
  }
}
