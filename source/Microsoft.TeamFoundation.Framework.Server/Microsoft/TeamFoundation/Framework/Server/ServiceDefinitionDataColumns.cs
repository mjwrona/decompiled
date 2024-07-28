// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceDefinitionDataColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceDefinitionDataColumns : ObjectBinder<ServiceDefinition>
  {
    private SqlColumnBinder typeNameColumn = new SqlColumnBinder("ServiceType");
    private SqlColumnBinder identifierColumn = new SqlColumnBinder("Identifier");
    private SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder relativeToSettingColumn = new SqlColumnBinder("RelativeToSetting");
    private SqlColumnBinder relativePathColumn = new SqlColumnBinder("RelativePath");
    private SqlColumnBinder descriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder toolIdColumn = new SqlColumnBinder("ToolType");

    protected override ServiceDefinition Bind()
    {
      ServiceDefinition serviceDefinition = new ServiceDefinition()
      {
        ServiceOwner = Guid.Empty,
        ServiceType = this.typeNameColumn.GetString((IDataReader) this.Reader, false),
        Identifier = this.identifierColumn.GetGuid((IDataReader) this.Reader),
        DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false),
        RelativePath = this.relativePathColumn.GetString((IDataReader) this.Reader, true),
        RelativeToSetting = (RelativeToSetting) this.relativeToSettingColumn.GetInt32((IDataReader) this.Reader),
        Description = this.descriptionColumn.GetString((IDataReader) this.Reader, true),
        ToolId = this.toolIdColumn.GetString((IDataReader) this.Reader, true)
      };
      if (serviceDefinition.Identifier == ServiceInstanceTypes.SPS)
        serviceDefinition.DisplayName = string.Intern(serviceDefinition.DisplayName);
      if (serviceDefinition.ToolId == null)
        serviceDefinition.ToolId = "Framework";
      return serviceDefinition;
    }
  }
}
