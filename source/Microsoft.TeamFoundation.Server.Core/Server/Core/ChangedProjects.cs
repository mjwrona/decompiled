// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ChangedProjects
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class ChangedProjects
  {
    private IList<ProjectInfo> m_modified;
    private IList<ProjectInfo> m_deleted;
    private ProjectRevision m_initialRevision;
    private string m_revision;

    internal ChangedProjects(
      IList<ProjectInfo> modified,
      IList<ProjectInfo> deleted,
      ProjectRevision revision)
    {
      this.m_modified = modified;
      this.m_deleted = deleted;
      this.m_initialRevision = revision;
    }

    public IList<ProjectInfo> Modified => this.m_modified;

    public IList<ProjectInfo> Deleted => this.m_deleted;

    public string Revision
    {
      get
      {
        if (this.m_revision == null)
          this.m_revision = this.GetRevision();
        return this.m_revision;
      }
    }

    private string GetRevision() => new ProjectRevision(this.m_modified == null || this.m_modified.Count <= 0 ? this.m_initialRevision.Modified : this.m_modified.Last<ProjectInfo>().Revision + 1L, this.m_deleted == null || this.m_deleted.Count <= 0 ? this.m_initialRevision.Deleted : this.m_deleted.Last<ProjectInfo>().Revision + 1L).ToString();
  }
}
