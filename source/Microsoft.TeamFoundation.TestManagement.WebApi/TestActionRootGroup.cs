// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestActionRootGroup
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  internal class TestActionRootGroup : TestActionGroup
  {
    private int m_lastUsedId;
    private string invalidXmlAttribute = "Invalid attribute in XML, {0}={1}";

    internal TestActionRootGroup(ITestActionOwner owner)
      : base(owner, 0)
    {
    }

    protected override void ReadAttributes(XmlReader reader)
    {
      if (reader != null)
      {
        string s = reader["last"];
        if (!int.TryParse(s, out this.m_lastUsedId))
          throw new XmlException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.invalidXmlAttribute, (object) "last", (object) s));
      }
      else
        this.m_lastUsedId = 0;
    }

    protected override void WriteAttributes(XmlWriter writer)
    {
      this.WriteBaseAttributes(writer);
      writer.WriteStartAttribute("last");
      writer.WriteValue(this.m_lastUsedId);
    }

    internal int GetNextAvailableId() => ++this.m_lastUsedId;
  }
}
