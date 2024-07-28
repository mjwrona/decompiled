// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContainerBinder2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContainerBinder2 : ContainerBinder
  {
    private TeamFoundationSqlResourceComponent containerComponent;
    private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");

    internal ContainerBinder2(TeamFoundationSqlResourceComponent component) => this.containerComponent = component;

    protected override Microsoft.VisualStudio.Services.FileContainer.FileContainer Bind()
    {
      Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer = base.Bind();
      fileContainer.ScopeIdentifier = this.containerComponent.GetDataspaceIdentifier(this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader));
      return fileContainer;
    }
  }
}
