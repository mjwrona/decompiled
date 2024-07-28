// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.RetentionPolicyBinder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class RetentionPolicyBinder2 : BuildObjectBinder<RetentionPolicy>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder buildReason = new SqlColumnBinder("BuildReason");
    private SqlColumnBinder buildStatus = new SqlColumnBinder("BuildStatus");
    private SqlColumnBinder numberToKeep = new SqlColumnBinder("NumberToKeep");
    private SqlColumnBinder deleteOptions = new SqlColumnBinder("DeleteOptions");

    internal RetentionPolicyBinder2(BuildSqlResourceComponent component)
      : base(component)
    {
    }

    protected override RetentionPolicy Bind() => new RetentionPolicy()
    {
      DefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false),
      BuildReason = this.buildReason.GetBuildReason(this.Reader),
      BuildStatus = this.buildStatus.GetBuildStatus(this.Reader),
      NumberToKeep = this.numberToKeep.GetInt32((IDataReader) this.Reader, 0),
      DeleteOptions = this.deleteOptions.GetDeleteOptions(this.Reader),
      ProjectId = this.Component.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader))
    };
  }
}
