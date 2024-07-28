// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectHistoryColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectHistoryColumns : ProjectInfoColumns
  {
    private SqlColumnBinder deltaColumn = new SqlColumnBinder("ProjectPropertiesDelta");

    protected override ProjectInfo Bind()
    {
      ProjectInfo projectInfo = base.Bind();
      int ordinal = this.deltaColumn.GetOrdinal((IDataReader) this.Reader);
      if (ordinal >= 0 && !this.deltaColumn.IsNull((IDataReader) this.Reader))
        ProjectHistoryColumns.ReadProperties(projectInfo, this.Reader.GetSqlXml(ordinal));
      return projectInfo;
    }

    internal static void ReadProperties(ProjectInfo projectInfo, SqlXml sqlXml)
    {
      using (XmlReader reader = sqlXml.CreateReader())
      {
        foreach (XElement descendant in XDocument.Load(reader).Descendants((XName) "property"))
        {
          ProjectProperty projectProperty = new ProjectProperty(descendant.Element((XName) "name").Value, (object) descendant.Element((XName) "value")?.Value);
          projectInfo.Properties.Add(projectProperty);
        }
      }
    }
  }
}
