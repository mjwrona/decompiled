// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.StructureComponent4
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class StructureComponent4 : StructureComponent3
  {
    internal override List<CommonStructureNodeInfo> GetAllNodes()
    {
      List<CommonStructureNodeInfo> allNodes = new List<CommonStructureNodeInfo>();
      StructureComponent.NodeInfoColumns nodeInfoColumns = new StructureComponent.NodeInfoColumns();
      this.PrepareStoredProcedure("prc_css_get_all_nodes");
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
        allNodes.Add(nodeInfoColumns.bind(reader));
      return allNodes;
    }
  }
}
