// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MachineRole
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class MachineRole
  {
    internal MachineRole()
    {
    }

    internal MachineRole(Guid id, string name)
    {
      this.Id = id;
      this.Name = name;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Name { get; set; }

    internal long LabEnvironmentId { get; private set; }

    [XmlIgnore]
    internal long LabSystemId { get; set; }

    internal void Internal_SetLabEnvironmentId(long environmentId) => this.LabEnvironmentId = environmentId;

    internal static string ToXml(List<MachineRole> roles)
    {
      StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      using (XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w))
      {
        xmlTextWriter.WriteStartDocument();
        xmlTextWriter.WriteStartElement("Roles");
        if (roles != null)
        {
          foreach (MachineRole role in roles)
          {
            xmlTextWriter.WriteStartElement("Role");
            xmlTextWriter.WriteAttributeString("Id", role.Id.ToString());
            xmlTextWriter.WriteAttributeString("Name", role.Name);
            xmlTextWriter.WriteEndElement();
          }
        }
        xmlTextWriter.WriteEndDocument();
        xmlTextWriter.Close();
      }
      return w.ToString();
    }

    internal static List<MachineRole> FromXml(string xml)
    {
      List<MachineRole> machineRoleList = new List<MachineRole>();
      if (!string.IsNullOrEmpty(xml))
      {
        using (XmlTextReader safeXmlTextReader = XmlUtility.CreateSafeXmlTextReader((TextReader) new StringReader(xml)))
        {
          if (safeXmlTextReader.IsStartElement("Roles"))
          {
            while (safeXmlTextReader.Read())
            {
              if (safeXmlTextReader.IsStartElement("Role"))
              {
                Guid guid = XmlConvert.ToGuid(safeXmlTextReader["Id"]);
                string name = safeXmlTextReader["Name"];
                if (guid != Guid.Empty && !string.IsNullOrEmpty(name))
                  machineRoleList.Add(new MachineRole(guid, name));
              }
            }
          }
        }
      }
      return machineRoleList;
    }

    internal static void UpdateExistingRoles(
      List<MachineRole> existingRoles,
      List<MachineRole> newRoles)
    {
      foreach (MachineRole newRole1 in newRoles)
      {
        MachineRole newRole = newRole1;
        MachineRole machineRole = newRole.Id != Guid.Empty ? existingRoles.Find((Predicate<MachineRole>) (n => n.Id == newRole.Id)) : (MachineRole) null;
        if (machineRole != null)
        {
          machineRole.Name = newRole.Name;
        }
        else
        {
          newRole.Id = Guid.NewGuid();
          existingRoles.Add(newRole);
        }
      }
      for (int i = 0; i < existingRoles.Count; i++)
      {
        if (!newRoles.Exists((Predicate<MachineRole>) (n => n.Id == existingRoles[i].Id)))
        {
          existingRoles.RemoveAt(i);
          i--;
        }
      }
    }
  }
}
