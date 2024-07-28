// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ServiceMetadataBinder
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ServiceMetadataBinder : ObjectBinder<ConnectedServiceMetadata>
  {
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder KindColumn = new SqlColumnBinder("Kind");
    private SqlColumnBinder FriendlyNameColumn = new SqlColumnBinder("FriendlyName");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder ServiceUriColumn = new SqlColumnBinder("ServiceUri");
    private SqlColumnBinder AuthenticatedBy = new SqlColumnBinder(nameof (AuthenticatedBy));
    private string m_teamProject;

    internal ServiceMetadataBinder(string teamProject) => this.m_teamProject = teamProject;

    protected override ConnectedServiceMetadata Bind() => new ConnectedServiceMetadata(this.NameColumn.GetString((IDataReader) this.Reader, false), this.m_teamProject, (ConnectedServiceKind) this.KindColumn.GetByte((IDataReader) this.Reader), this.FriendlyNameColumn.GetString((IDataReader) this.Reader, true), this.DescriptionColumn.GetString((IDataReader) this.Reader, true), this.ServiceUriColumn.GetString((IDataReader) this.Reader, true))
    {
      AuthenticatedBy = this.AuthenticatedBy.GetGuid((IDataReader) this.Reader, true)
    };
  }
}
