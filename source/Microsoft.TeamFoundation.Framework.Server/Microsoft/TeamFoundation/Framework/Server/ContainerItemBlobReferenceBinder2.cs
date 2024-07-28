// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContainerItemBlobReferenceBinder2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContainerItemBlobReferenceBinder2 : ContainerItemBlobReferenceBinder
  {
    private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    private TeamFoundationSqlResourceComponent containerComponent;

    public ContainerItemBlobReferenceBinder2(TeamFoundationSqlResourceComponent component) => this.containerComponent = component;

    protected override ContainerItemBlobReference Bind()
    {
      ContainerItemBlobReference itemBlobReference = base.Bind();
      itemBlobReference.ScopeIdentifier = this.containerComponent.GetDataspaceIdentifier(this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader));
      return itemBlobReference;
    }
  }
}
