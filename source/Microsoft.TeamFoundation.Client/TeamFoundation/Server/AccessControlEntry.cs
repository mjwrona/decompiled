// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AccessControlEntry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03")]
  [Serializable]
  public class AccessControlEntry : ICloneable
  {
    public string ActionId = string.Empty;
    public string Sid = string.Empty;
    public bool Deny;

    public AccessControlEntry(string actionId, string sid, bool deny)
    {
      this.ActionId = Aux.NormalizeString(actionId, false);
      this.Sid = Aux.NormalizeString(sid, false);
      this.Deny = deny;
    }

    public override bool Equals(object obj) => obj is AccessControlEntry accessControlEntry && this.ActionId == accessControlEntry.ActionId && this.Sid == accessControlEntry.Sid && this.Deny == accessControlEntry.Deny;

    public override int GetHashCode() => this.ActionId.GetHashCode() ^ this.Sid.GetHashCode();

    public object Clone() => this.MemberwiseClone();

    public AccessControlEntry()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AccessControlEntry FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      AccessControlEntry accessControlEntry = new AccessControlEntry();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "ActionId":
              accessControlEntry.ActionId = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Deny":
              accessControlEntry.Deny = XmlUtility.BooleanFromXmlElement(reader);
              continue;
            case "Sid":
              accessControlEntry.Sid = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return accessControlEntry;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("AccessControlEntry instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ActionId: " + this.ActionId);
      stringBuilder.AppendLine("  Deny: " + this.Deny.ToString());
      stringBuilder.AppendLine("  Sid: " + this.Sid);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.ActionId != null)
        XmlUtility.ToXmlElement(writer, "ActionId", this.ActionId);
      if (this.Deny)
        XmlUtility.ToXmlElement(writer, "Deny", this.Deny);
      if (this.Sid != null)
        XmlUtility.ToXmlElement(writer, "Sid", this.Sid);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, AccessControlEntry obj) => obj.ToXml(writer, element);
  }
}
