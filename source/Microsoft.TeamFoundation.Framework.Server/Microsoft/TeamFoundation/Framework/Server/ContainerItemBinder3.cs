// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContainerItemBinder3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContainerItemBinder3 : ContainerItemBinder2
  {
    private SqlColumnBinder ArtifactIdColumn = new SqlColumnBinder("ArtifactId");

    internal ContainerItemBinder3(TeamFoundationSqlResourceComponent component)
      : base(component)
    {
    }

    protected override FileContainerItem Bind()
    {
      FileContainerItem fileContainerItem = base.Bind();
      fileContainerItem.ArtifactId = this.ArtifactIdColumn.GetNullableInt64((IDataReader) this.Reader);
      return fileContainerItem;
    }
  }
}
