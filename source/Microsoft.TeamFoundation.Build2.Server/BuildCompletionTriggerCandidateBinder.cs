// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildCompletionTriggerCandidateBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildCompletionTriggerCandidateBinder : 
    BuildObjectBinder<BuildCompletionTriggerCandidate>
  {
    private SqlColumnBinder m_targetDataspaceId = new SqlColumnBinder("TargetDataspaceId");
    private SqlColumnBinder m_targetDefinitionId = new SqlColumnBinder("TargetDefinitionId");
    private SqlColumnBinder m_branchFilters = new SqlColumnBinder("BranchFilters");

    internal BuildCompletionTriggerCandidateBinder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override BuildCompletionTriggerCandidate Bind() => new BuildCompletionTriggerCandidate()
    {
      ProjectId = this.ResourceComponent.GetDataspaceIdentifier(this.m_targetDataspaceId.GetInt32((IDataReader) this.Reader)),
      DefinitionId = this.m_targetDefinitionId.GetInt32((IDataReader) this.Reader),
      BranchFilters = this.m_branchFilters.GetString((IDataReader) this.Reader, false)
    };
  }
}
