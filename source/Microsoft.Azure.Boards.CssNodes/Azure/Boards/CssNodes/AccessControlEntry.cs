// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.AccessControlEntry
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Azure.Boards.CssNodes
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [ClassNotSealed]
  [Serializable]
  public class AccessControlEntry : ICloneable
  {
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string ActionId;
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Sid;
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public bool Deny;

    public AccessControlEntry()
    {
      this.ActionId = string.Empty;
      this.Sid = string.Empty;
      this.Deny = false;
    }

    public AccessControlEntry(string actionId, string sid, bool deny)
    {
      this.ActionId = Aux.NormalizeString(actionId, false);
      this.Sid = Aux.NormalizeString(sid, false);
      this.Deny = deny;
    }

    public override bool Equals(object obj) => obj is AccessControlEntry accessControlEntry && this.ActionId == accessControlEntry.ActionId && this.Sid == accessControlEntry.Sid && this.Deny == accessControlEntry.Deny;

    public override int GetHashCode() => this.ActionId.GetHashCode() ^ this.Sid.GetHashCode();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.ActionId.Length + this.Sid.Length + 50);
      stringBuilder.Append("[ACE ActionId:");
      stringBuilder.Append(this.ActionId);
      stringBuilder.Append(" Sid:");
      stringBuilder.Append(this.Sid);
      stringBuilder.Append(" Deny:");
      stringBuilder.Append(this.Deny);
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }

    public object Clone() => this.MemberwiseClone();
  }
}
