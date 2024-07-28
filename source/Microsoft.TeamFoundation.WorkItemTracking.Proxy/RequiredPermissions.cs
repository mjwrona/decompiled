// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RequiredPermissions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public sealed class RequiredPermissions
  {
    private string m_permissionName;
    private int m_requiredPermissionBits;

    private RequiredPermissions()
    {
    }

    public string PermissionName
    {
      get => this.m_permissionName;
      set => this.m_permissionName = value;
    }

    public int RequiredPermissionBits
    {
      get => this.m_requiredPermissionBits;
      set => this.m_requiredPermissionBits = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static RequiredPermissions FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      RequiredPermissions requiredPermissions = new RequiredPermissions();
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
            case "PermissionName":
              requiredPermissions.m_permissionName = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "RequiredPermissionBits":
              requiredPermissions.m_requiredPermissionBits = XmlUtility.Int32FromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return requiredPermissions;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RequiredPermissions instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  PermissionName: " + this.m_permissionName);
      stringBuilder.AppendLine("  RequiredPermissionBits: " + this.m_requiredPermissionBits.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_permissionName != null)
        XmlUtility.ToXmlElement(writer, "PermissionName", this.m_permissionName);
      if (this.m_requiredPermissionBits != 0)
        XmlUtility.ToXmlElement(writer, "RequiredPermissionBits", this.m_requiredPermissionBits);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, RequiredPermissions obj) => obj.ToXml(writer, element);
  }
}
