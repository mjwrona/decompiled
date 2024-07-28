// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.RetentionPolicy2010Binder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class RetentionPolicy2010Binder2 : BuildObjectBinder<RetentionPolicy2010>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder buildReason = new SqlColumnBinder("BuildReason");
    private SqlColumnBinder buildStatus = new SqlColumnBinder("BuildStatus");
    private SqlColumnBinder numberToKeep = new SqlColumnBinder("NumberToKeep");
    private SqlColumnBinder deleteOptions = new SqlColumnBinder("DeleteOptions");

    public RetentionPolicy2010Binder2(BuildSqlResourceComponent component)
      : base(component)
    {
    }

    protected override RetentionPolicy2010 Bind() => new RetentionPolicy2010()
    {
      DefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false),
      BuildReason = RosarioHelper.Convert(this.buildReason.GetBuildReason(this.Reader)),
      BuildStatus = RosarioHelper.Convert(this.buildStatus.GetBuildStatus(this.Reader)),
      NumberToKeep = this.numberToKeep.GetInt32((IDataReader) this.Reader, 0),
      DeleteOptions = RosarioHelper.Convert(this.deleteOptions.GetDeleteOptions(this.Reader)),
      ProjectId = this.Component.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader))
    };
  }
}
