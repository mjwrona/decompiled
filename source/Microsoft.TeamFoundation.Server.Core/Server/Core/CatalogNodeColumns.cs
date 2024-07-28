// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogNodeColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogNodeColumns : ObjectBinder<CatalogNode>
  {
    private SqlColumnBinder parentPathColumn = new SqlColumnBinder("ParentPath");
    private SqlColumnBinder childItemColumn = new SqlColumnBinder("ChildItem");
    private SqlColumnBinder resourceIdentifierColumn = new SqlColumnBinder("ResourceIdentifier");
    private SqlColumnBinder isDefaultColumn = new SqlColumnBinder("IsDefault");
    private SqlColumnBinder matchedQueryColumn = new SqlColumnBinder("MatchedQuery");
    private string m_internalPath;
    private bool m_expandDependencies;

    public CatalogNodeColumns(bool expandDependencies, string internalPath)
    {
      this.m_expandDependencies = expandDependencies;
      this.m_internalPath = internalPath;
    }

    protected override CatalogNode Bind()
    {
      string str = CatalogComponent.TranslateNodePath(this.parentPathColumn.GetString((IDataReader) this.Reader, false) + this.childItemColumn.GetString((IDataReader) this.Reader, false), this.m_internalPath, CatalogRoots.OrganizationalPath);
      return new CatalogNode()
      {
        FullPath = str,
        ParentPath = str.Substring(0, str.Length - 24),
        ChildItem = str.Substring(str.Length - 24, 24),
        ResourceIdentifier = this.resourceIdentifierColumn.GetGuid((IDataReader) this.Reader),
        IsDefault = this.isDefaultColumn.GetBoolean((IDataReader) this.Reader),
        MatchedQuery = this.matchedQueryColumn.GetBoolean((IDataReader) this.Reader),
        NodeDependenciesIncluded = this.m_expandDependencies
      };
    }
  }
}
