// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceDefinitionDataColumns2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceDefinitionDataColumns2 : ServiceDefinitionDataColumns
  {
    private SqlColumnBinder parentServiceTypeColumn = new SqlColumnBinder("ParentServiceType");
    private SqlColumnBinder parentIdentifierColumn = new SqlColumnBinder("ParentIdentifier");
    private SqlColumnBinder statusColumn = new SqlColumnBinder("Status");

    protected override ServiceDefinition Bind()
    {
      ServiceDefinition serviceDefinition = base.Bind();
      serviceDefinition.ParentServiceType = this.parentServiceTypeColumn.GetString((IDataReader) this.Reader, true);
      serviceDefinition.ParentIdentifier = this.parentIdentifierColumn.GetGuid((IDataReader) this.Reader, true);
      serviceDefinition.Status = (ServiceStatus) this.statusColumn.GetByte((IDataReader) this.Reader);
      return serviceDefinition;
    }
  }
}
