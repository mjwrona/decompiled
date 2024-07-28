// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxItemInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ClassVisibility(ClientVisibility.ProductInternal, ClientVisibility.Private)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Framework")]
  public class StrongBoxItemInfo
  {
    public StrongBoxItemInfo() => this.FileId = -1;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.ProductInternal, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public Guid DrawerId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.ProductInternal, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public StrongBoxItemKind ItemKind { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.ProductInternal, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string LookupKey { get; set; }

    [XmlIgnore]
    public Guid SigningKeyId { get; set; }

    [XmlIgnore]
    public DateTime? ExpirationDate { get; set; }

    [XmlIgnore]
    public DateTime? LastUpdateTime { get; set; }

    [XmlIgnore]
    public string CredentialName { get; set; }

    [XmlIgnore]
    internal byte[] EncryptedContent { get; set; }

    [XmlIgnore]
    public int SecureFileId { get; set; }

    [XmlIgnore]
    internal string Value { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.ProductInternal, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int FileId { get; set; }

    internal bool Equals(StrongBoxItemInfo other)
    {
      if (other == null || !(this.DrawerId == other.DrawerId) || this.ItemKind != other.ItemKind || !(this.LookupKey == other.LookupKey) || !(this.SigningKeyId == other.SigningKeyId))
        return false;
      DateTime? expirationDate1 = this.ExpirationDate;
      DateTime? expirationDate2 = other.ExpirationDate;
      return (expirationDate1.HasValue == expirationDate2.HasValue ? (expirationDate1.HasValue ? (expirationDate1.GetValueOrDefault() == expirationDate2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.CredentialName == other.CredentialName && (this.EncryptedContent == other.EncryptedContent || this.EncryptedContent != null && other.EncryptedContent != null && ((IEnumerable<byte>) this.EncryptedContent).SequenceEqual<byte>((IEnumerable<byte>) other.EncryptedContent)) && this.SecureFileId == other.SecureFileId && this.Value == other.Value && this.FileId == other.FileId;
    }
  }
}
