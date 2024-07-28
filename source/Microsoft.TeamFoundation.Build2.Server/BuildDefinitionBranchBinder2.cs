// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionBranchBinder2
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDefinitionBranchBinder2 : BuildObjectBinder<BuildDefinitionBranch>
  {
    private SqlColumnBinder m_branchName = new SqlColumnBinder("BranchName");
    private SqlColumnBinder m_sourceId = new SqlColumnBinder("SourceId");
    private SqlColumnBinder m_pendingSourceOwner = new SqlColumnBinder("PendingSourceOwner");
    private SqlColumnBinder m_pendingSourceVersion = new SqlColumnBinder("PendingSourceVersion");

    public BuildDefinitionBranchBinder2(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override BuildDefinitionBranch Bind()
    {
      BuildDefinitionBranch definitionBranch = new BuildDefinitionBranch();
      definitionBranch.BranchName = this.m_branchName.GetString((IDataReader) this.Reader, false);
      definitionBranch.SourceId = this.m_sourceId.GetInt64((IDataReader) this.Reader, 0L);
      Guid? nullableGuid = this.m_pendingSourceOwner.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid.HasValue)
        definitionBranch.PendingSourceOwner = nullableGuid.Value;
      definitionBranch.PendingSourceVersion = this.m_pendingSourceVersion.GetString((IDataReader) this.Reader, true);
      return definitionBranch;
    }
  }
}
