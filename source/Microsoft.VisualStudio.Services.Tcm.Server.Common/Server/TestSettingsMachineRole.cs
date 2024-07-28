// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestSettingsMachineRole
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue(false)]
    public bool IsExecution { get; set; }

    internal static TestSettingsMachineRole[] FromXml(IVssRequestContext context, string xml)
    {
      if (!string.IsNullOrEmpty(xml))
      {
        try
        {
          List<TestSettingsMachineRole> settingsMachineRoleList = new List<TestSettingsMachineRole>();
          XmlReader safeReader = XmlUtility.CreateSafeReader((TextReader) new StringReader(xml));
          for (bool flag = safeReader.ReadToFollowing("role"); flag; flag = safeReader.ReadToNextSibling("role"))
          {
            TestSettingsMachineRole settingsMachineRole = new TestSettingsMachineRole();
            if (safeReader.MoveToAttribute("name"))
              settingsMachineRole.Name = safeReader.ReadContentAsString();
            if (safeReader.MoveToAttribute("execution"))
              settingsMachineRole.IsExecution = safeReader.ReadContentAsBoolean();
            settingsMachineRoleList.Add(settingsMachineRole);
          }
          return settingsMachineRoleList.ToArray();
        }
        catch (XmlException ex)
        {
          context.TraceException("Database", (Exception) ex);
        }
      }
      return (TestSettingsMachineRole[]) null;
    }

    internal static string ToXml(TestSettingsMachineRole[] roles)
    {
      string xml = (string) null;
      if (roles != null)
      {
        StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
        XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w);
        xmlTextWriter.WriteStartDocument();
        xmlTextWriter.WriteStartElement(nameof (roles));
        foreach (TestSettingsMachineRole role in roles)
        {
          xmlTextWriter.WriteStartElement("role");
          xmlTextWriter.WriteAttributeString("name", role.Name);
          xmlTextWriter.WriteStartAttribute("execution");
          xmlTextWriter.WriteValue(role.IsExecution);
          xmlTextWriter.WriteEndAttribute();
          xmlTextWriter.WriteEndElement();
        }
        xmlTextWriter.WriteEndDocument();
        xmlTextWriter.Flush();
        xml = w.ToString();
      }
      return xml;
    }
  }
}
