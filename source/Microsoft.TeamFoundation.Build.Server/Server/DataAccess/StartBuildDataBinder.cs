// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.StartBuildDataBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class StartBuildDataBinder : BuildObjectBinder<StartBuildData>
  {
    private SqlColumnBinder buildUriColumn = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder messageQueueUrlColumn = new SqlColumnBinder("MessageQueueUrl");
    private SqlColumnBinder endpointUrlColumn = new SqlColumnBinder("EndpointUrl");
    private SqlColumnBinder startOrderColumn = new SqlColumnBinder("StartOrder");
    private SqlColumnBinder definitionIdColumn = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder controllerIdColumn = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder dropLocationColumn = new SqlColumnBinder("DropLocation");
    private SqlColumnBinder queueIdColumn = new SqlColumnBinder("QueueId");

    protected override StartBuildData Bind() => new StartBuildData()
    {
      BuildUri = this.buildUriColumn.GetArtifactUriFromString(this.Reader, "Build", false),
      MessageQueueUrl = DBHelper.DBUrlToServerUrl(this.messageQueueUrlColumn.GetString((IDataReader) this.Reader, false)),
      EndpointUrl = this.endpointUrlColumn.ColumnExists((IDataReader) this.Reader) ? DBHelper.DBUrlToServerUrl(this.endpointUrlColumn.GetString((IDataReader) this.Reader, false)) : (string) null,
      StartOrder = this.startOrderColumn.ColumnExists((IDataReader) this.Reader) ? this.startOrderColumn.GetInt32((IDataReader) this.Reader) : 0,
      DefinitionUri = this.definitionIdColumn.ColumnExists((IDataReader) this.Reader) ? this.definitionIdColumn.GetArtifactUriFromInt32(this.Reader, "Definition", false) : (string) null,
      ControllerUri = this.controllerIdColumn.ColumnExists((IDataReader) this.Reader) ? this.controllerIdColumn.GetArtifactUriFromInt32(this.Reader, "Controller", false) : (string) null,
      DropLocation = this.dropLocationColumn.ColumnExists((IDataReader) this.Reader) ? this.dropLocationColumn.GetString((IDataReader) this.Reader, true) : (string) null,
      QueueId = this.queueIdColumn.ColumnExists((IDataReader) this.Reader) ? this.queueIdColumn.GetInt32((IDataReader) this.Reader) : 0
    };
  }
}
