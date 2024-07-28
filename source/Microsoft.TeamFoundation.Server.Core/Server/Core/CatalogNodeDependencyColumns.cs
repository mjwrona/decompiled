// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogNodeDependencyColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogNodeDependencyColumns : ObjectBinder<CatalogNodeDependency>
  {
    private SqlColumnBinder pathColumn = new SqlColumnBinder("Path");
    private SqlColumnBinder associationKeyColumn = new SqlColumnBinder("AssociationKey");
    private SqlColumnBinder requiredPathColumn = new SqlColumnBinder("RequiredPath");
    private SqlColumnBinder isSingletonColumn = new SqlColumnBinder("IsSingleton");
    private string m_internalPath;

    public CatalogNodeDependencyColumns(string internalPath) => this.m_internalPath = internalPath;

    protected override CatalogNodeDependency Bind() => new CatalogNodeDependency()
    {
      FullPath = CatalogComponent.TranslateNodePath(this.pathColumn.GetString((IDataReader) this.Reader, false), this.m_internalPath, CatalogRoots.OrganizationalPath),
      AssociationKey = this.associationKeyColumn.GetString((IDataReader) this.Reader, false),
      RequiredNodeFullPath = CatalogComponent.TranslateNodePath(this.requiredPathColumn.GetString((IDataReader) this.Reader, false), this.m_internalPath, CatalogRoots.OrganizationalPath),
      IsSingleton = this.isSingletonColumn.GetBoolean((IDataReader) this.Reader, false)
    };
  }
}
