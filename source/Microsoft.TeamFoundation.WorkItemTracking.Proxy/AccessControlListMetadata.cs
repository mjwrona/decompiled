// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.AccessControlListMetadata
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
  public sealed class AccessControlListMetadata
  {
    private string m_fullSelectionPermission;
    internal string[] m_irrevocableAdminPermissions = Helper.ZeroLengthArrayOfString;
    private string m_objectClassId;
    internal string[] m_permissionDescriptions = Helper.ZeroLengthArrayOfString;
    internal string[] m_permissionDisplayStrings = Helper.ZeroLengthArrayOfString;
    internal string[] m_permissionNames = Helper.ZeroLengthArrayOfString;
    internal RequiredPermissions[] m_permissionRequirements = Helper.ZeroLengthArrayOfRequiredPermissions;

    private AccessControlListMetadata()
    {
    }

    public string FullSelectionPermission
    {
      get => this.m_fullSelectionPermission;
      set => this.m_fullSelectionPermission = value;
    }

    public string[] IrrevocableAdminPermissions
    {
      get => (string[]) this.m_irrevocableAdminPermissions.Clone();
      set => this.m_irrevocableAdminPermissions = value;
    }

    public string ObjectClassId
    {
      get => this.m_objectClassId;
      set => this.m_objectClassId = value;
    }

    public string[] PermissionDescriptions
    {
      get => (string[]) this.m_permissionDescriptions.Clone();
      set => this.m_permissionDescriptions = value;
    }

    public string[] PermissionDisplayStrings
    {
      get => (string[]) this.m_permissionDisplayStrings.Clone();
      set => this.m_permissionDisplayStrings = value;
    }

    public string[] PermissionNames
    {
      get => (string[]) this.m_permissionNames.Clone();
      set => this.m_permissionNames = value;
    }

    public RequiredPermissions[] PermissionRequirements
    {
      get => (RequiredPermissions[]) this.m_permissionRequirements.Clone();
      set => this.m_permissionRequirements = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AccessControlListMetadata FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      AccessControlListMetadata controlListMetadata = new AccessControlListMetadata();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name1 = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name2 = reader.Name;
          if (name2 != null)
          {
            switch (name2.Length)
            {
              case 13:
                if (name2 == "ObjectClassId")
                {
                  controlListMetadata.m_objectClassId = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 15:
                if (name2 == "PermissionNames")
                {
                  controlListMetadata.m_permissionNames = Helper.ArrayOfStringFromXml(reader, false);
                  continue;
                }
                break;
              case 22:
                switch (name2[10])
                {
                  case 'D':
                    if (name2 == "PermissionDescriptions")
                    {
                      controlListMetadata.m_permissionDescriptions = Helper.ArrayOfStringFromXml(reader, false);
                      continue;
                    }
                    break;
                  case 'R':
                    if (name2 == "PermissionRequirements")
                    {
                      controlListMetadata.m_permissionRequirements = Helper.ArrayOfRequiredPermissionsFromXml(serviceProvider, reader, false);
                      continue;
                    }
                    break;
                }
                break;
              case 23:
                if (name2 == "FullSelectionPermission")
                {
                  controlListMetadata.m_fullSelectionPermission = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 24:
                if (name2 == "PermissionDisplayStrings")
                {
                  controlListMetadata.m_permissionDisplayStrings = Helper.ArrayOfStringFromXml(reader, false);
                  continue;
                }
                break;
              case 27:
                if (name2 == "IrrevocableAdminPermissions")
                {
                  controlListMetadata.m_irrevocableAdminPermissions = Helper.ArrayOfStringFromXml(reader, false);
                  continue;
                }
                break;
            }
          }
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return controlListMetadata;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("AccessControlListMetadata instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  FullSelectionPermission: " + this.m_fullSelectionPermission);
      stringBuilder.AppendLine("  IrrevocableAdminPermissions: " + Helper.ArrayToString<string>(this.m_irrevocableAdminPermissions));
      stringBuilder.AppendLine("  ObjectClassId: " + this.m_objectClassId);
      stringBuilder.AppendLine("  PermissionDescriptions: " + Helper.ArrayToString<string>(this.m_permissionDescriptions));
      stringBuilder.AppendLine("  PermissionDisplayStrings: " + Helper.ArrayToString<string>(this.m_permissionDisplayStrings));
      stringBuilder.AppendLine("  PermissionNames: " + Helper.ArrayToString<string>(this.m_permissionNames));
      stringBuilder.AppendLine("  PermissionRequirements: " + Helper.ArrayToString<RequiredPermissions>(this.m_permissionRequirements));
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_fullSelectionPermission != null)
        XmlUtility.ToXmlElement(writer, "FullSelectionPermission", this.m_fullSelectionPermission);
      Helper.ToXml(writer, "IrrevocableAdminPermissions", this.m_irrevocableAdminPermissions, false, false);
      if (this.m_objectClassId != null)
        XmlUtility.ToXmlElement(writer, "ObjectClassId", this.m_objectClassId);
      Helper.ToXml(writer, "PermissionDescriptions", this.m_permissionDescriptions, false, false);
      Helper.ToXml(writer, "PermissionDisplayStrings", this.m_permissionDisplayStrings, false, false);
      Helper.ToXml(writer, "PermissionNames", this.m_permissionNames, false, false);
      Helper.ToXml(writer, "PermissionRequirements", this.m_permissionRequirements, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, AccessControlListMetadata obj) => obj.ToXml(writer, element);
  }
}
