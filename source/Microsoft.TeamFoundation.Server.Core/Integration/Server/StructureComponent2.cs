// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.StructureComponent2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class StructureComponent2 : StructureComponent
  {
    internal override List<CommonStructureNodeInfo> GetNodesFromIds(List<string> ids)
    {
      List<CommonStructureNodeInfo> nodesFromIds = new List<CommonStructureNodeInfo>();
      this.PrepareStoredProcedure("prc_css_get_nodes_from_ids");
      this.BindStringTable("@nodeIds", (IEnumerable<string>) ids);
      SqlDataReader reader = this.ExecuteReader();
      StructureComponent.NodeInfoColumns nodeInfoColumns = new StructureComponent.NodeInfoColumns();
      while (reader.Read())
        nodesFromIds.Add(nodeInfoColumns.bind(reader));
      return nodesFromIds;
    }
  }
}
