// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDetailBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildDetailBinder : BuildObjectBinder<BuildDetail>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder buildUri = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder buildNumber = new SqlColumnBinder("BuildNumber");
    private SqlColumnBinder startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder buildStatus = new SqlColumnBinder("BuildStatus");
    private SqlColumnBinder quality = new SqlColumnBinder("Quality");
    private SqlColumnBinder compilationStatus = new SqlColumnBinder("CompilationStatus");
    private SqlColumnBinder testStatus = new SqlColumnBinder("TestStatus");
    private SqlColumnBinder dropLocation = new SqlColumnBinder("DropLocation");
    private SqlColumnBinder dropLocationRoot = new SqlColumnBinder("DropLocationRoot");
    private SqlColumnBinder logLocation = new SqlColumnBinder("LogLocation");
    private SqlColumnBinder sourceGetVersion = new SqlColumnBinder("SourceGetVersion");
    private SqlColumnBinder controllerId = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder lastChangedOn = new SqlColumnBinder("LastChangedOn");
    private SqlColumnBinder lastChangedBy = new SqlColumnBinder("LastChangedBy");
    private SqlColumnBinder labelName = new SqlColumnBinder("LabelName");
    private SqlColumnBinder reason = new SqlColumnBinder("Reason");
    private SqlColumnBinder processParameters = new SqlColumnBinder("ProcessParameters");
    private SqlColumnBinder keepForever = new SqlColumnBinder("KeepForever");
    private SqlColumnBinder isDeleted = new SqlColumnBinder("Deleted");
    private SqlColumnBinder queueIds = new SqlColumnBinder("QueueIds");

    public BuildDetailBinder(IVssRequestContext requestContext, BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override BuildDetail Bind()
    {
      string displayName = (string) null;
      TeamFoundationIdentity identity = (TeamFoundationIdentity) null;
      BuildDetail buildDetail = new BuildDetail();
      buildDetail.Uri = this.buildUri.GetArtifactUriFromString(this.Reader, "Build", false);
      buildDetail.BuildDefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false);
      buildDetail.BuildNumber = this.buildNumber.GetBuildItem(this.Reader, false);
      buildDetail.StartTime = this.startTime.GetDateTime((IDataReader) this.Reader);
      buildDetail.FinishTime = this.finishTime.GetDateTime((IDataReader) this.Reader);
      buildDetail.Status = this.buildStatus.GetBuildStatus(this.Reader);
      buildDetail.Quality = BuildQuality.TryConvertResIdToBuildQuality(this.quality.GetString((IDataReader) this.Reader, true));
      buildDetail.CompilationStatus = this.compilationStatus.GetBuildPhaseStatus(this.Reader);
      buildDetail.TestStatus = this.testStatus.GetBuildPhaseStatus(this.Reader);
      buildDetail.DropLocation = this.dropLocation.GetString((IDataReader) this.Reader, true);
      buildDetail.DropLocationRoot = this.dropLocationRoot.GetString((IDataReader) this.Reader, true);
      buildDetail.LogLocation = this.logLocation.GetString((IDataReader) this.Reader, true);
      buildDetail.SourceGetVersion = this.sourceGetVersion.GetString((IDataReader) this.Reader, true);
      buildDetail.BuildControllerUri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      buildDetail.LastChangedOn = this.lastChangedOn.GetDateTime((IDataReader) this.Reader);
      buildDetail.LastChangedBy = this.GetUniqueName(this.m_requestContext, this.lastChangedBy.GetString((IDataReader) this.Reader, false), out displayName, out identity);
      string g = identity != null ? identity.TeamFoundationId.ToString() : Guid.Empty.ToString();
      buildDetail.LastChangedByDisplayName = displayName;
      buildDetail.LastChangeByTFID = new Guid(g);
      buildDetail.LabelName = this.labelName.GetString((IDataReader) this.Reader, true);
      buildDetail.Reason = this.reason.GetBuildReason(this.Reader);
      buildDetail.ProcessParameters = this.processParameters.GetString((IDataReader) this.Reader, true);
      buildDetail.KeepForever = this.keepForever.GetBoolean((IDataReader) this.Reader);
      buildDetail.IsDeleted = this.isDeleted.GetBoolean((IDataReader) this.Reader);
      buildDetail.QueueIds.AddRange((IEnumerable<int>) this.queueIds.XmlToListOfInt32(this.Reader));
      return buildDetail;
    }
  }
}
