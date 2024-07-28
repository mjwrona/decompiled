// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent15
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent15 : ProjectComponent14
  {
    protected override ObjectBinder<ProjectInfo> CreateProjectHistoryColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectHistoryColumns4();

    protected override void BindProperties(ProjectInfo project)
    {
      List<PropertyValue> propertyValueList = new List<PropertyValue>();
      if (project.Properties != null)
      {
        foreach (ProjectProperty property in (IEnumerable<ProjectProperty>) project.Properties)
        {
          if (!TFStringComparer.TeamProjectPropertyName.Equals(property.Name, "TemplateName"))
            propertyValueList.Add(new PropertyValue(property.Name, property.Value));
        }
      }
      ArtifactSpec projectPropertySpec = TeamProjectUtil.GetProjectPropertySpec(project.Id);
      string typeName;
      IEnumerable<SqlDataRecord> sqlRecords = PropertyComponent9.ConvertToSqlRecords(new ArtifactPropertyValue(projectPropertySpec, (IEnumerable<PropertyValue>) propertyValueList), this.GetDataspaceId(projectPropertySpec.DataspaceIdentifier, "Default"), out typeName);
      this.BindTable("@properties", typeName, sqlRecords);
    }
  }
}
