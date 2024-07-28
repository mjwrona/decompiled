// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.QueuedBuildBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class QueuedBuildBinder : BuildObjectBinder<QueuedBuild>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder controllerId = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder queuePosition = new SqlColumnBinder("QueuePosition");
    private SqlColumnBinder priority = new SqlColumnBinder("Priority");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder batchId = new SqlColumnBinder("BatchId");
    private SqlColumnBinder getOption = new SqlColumnBinder("GetOption");
    private SqlColumnBinder customGetVersion = new SqlColumnBinder("CustomGetVersion");
    private SqlColumnBinder dropLocation = new SqlColumnBinder("DropLocation");
    private SqlColumnBinder requestedFor = new SqlColumnBinder("RequestedFor");
    private SqlColumnBinder requestedBy = new SqlColumnBinder("RequestedBy");
    private SqlColumnBinder shelvesetName = new SqlColumnBinder("ShelvesetName");
    private SqlColumnBinder reason = new SqlColumnBinder("Reason");
    private SqlColumnBinder processParameters = new SqlColumnBinder("ProcessParameters");
    private SqlColumnBinder buildUris = new SqlColumnBinder("BuildUris");

    public QueuedBuildBinder(IVssRequestContext requestContext, BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override QueuedBuild Bind()
    {
      QueuedBuild queuedBuild = new QueuedBuild();
      string displayName1 = (string) null;
      string displayName2 = (string) null;
      TeamFoundationIdentity identity1 = (TeamFoundationIdentity) null;
      TeamFoundationIdentity identity2 = (TeamFoundationIdentity) null;
      queuedBuild.Id = this.queueId.GetInt32((IDataReader) this.Reader);
      queuedBuild.BuildControllerUri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      queuedBuild.BuildDefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false);
      queuedBuild.QueueTime = this.queueTime.GetDateTime((IDataReader) this.Reader);
      queuedBuild.QueuePosition = this.queuePosition.GetInt32((IDataReader) this.Reader);
      queuedBuild.Priority = (QueuePriority) this.priority.GetByte((IDataReader) this.Reader, (byte) 3);
      queuedBuild.Status = (QueueStatus) this.status.GetByte((IDataReader) this.Reader, (byte) 0);
      queuedBuild.BatchId = this.batchId.GetGuid((IDataReader) this.Reader, false);
      queuedBuild.GetOption = (GetOption) this.getOption.GetByte((IDataReader) this.Reader, (byte) 1);
      queuedBuild.CustomGetVersion = this.customGetVersion.GetString((IDataReader) this.Reader, true);
      queuedBuild.DropLocation = this.dropLocation.GetString((IDataReader) this.Reader, true);
      queuedBuild.RequestedFor = this.GetUniqueName(this.m_requestContext, this.requestedFor.GetString((IDataReader) this.Reader, true), out displayName2, out identity1);
      queuedBuild.RequestedForDisplayName = displayName2;
      queuedBuild.RequestedForIdentity = identity1;
      queuedBuild.RequestedBy = this.GetUniqueName(this.m_requestContext, this.requestedBy.GetString((IDataReader) this.Reader, false), out displayName1, out identity2);
      queuedBuild.RequestedByDisplayName = displayName1;
      queuedBuild.RequestedByIdentity = identity2;
      queuedBuild.ShelvesetName = this.shelvesetName.GetString((IDataReader) this.Reader, true);
      queuedBuild.Reason = this.reason.GetBuildReason(this.Reader);
      queuedBuild.ProcessParameters = this.processParameters.GetString((IDataReader) this.Reader, true);
      int int32 = this.buildId.GetInt32((IDataReader) this.Reader, 0, 0);
      if (int32 > 0)
        queuedBuild.BuildId = new int?(int32);
      IList<string> listOfString = this.buildUris.XmlToListOfString(this.Reader);
      queuedBuild.BuildUris.AddRange(listOfString.Select<string, string>((System.Func<string, string>) (x => DBHelper.CreateArtifactUri("Build", x))));
      return queuedBuild;
    }
  }
}
