// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildInformationMerger
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class BuildInformationMerger : IDisposable
  {
    private bool m_hasMoreData;
    private bool m_initialized;
    private BuildInformationComponent m_db;
    private int m_groupId = int.MinValue;
    private IList<string> m_informationTypes;
    private ResultCollection m_resultCollection;
    private IVssRequestContext m_requestContext;
    private List<BuildDetail> m_builds = new List<BuildDetail>();
    private ObjectBinder<BuildInformationRow> m_buildInformationBinder;
    private ObjectBinder<BuildInformationNode> m_buildInformationBinder2;
    private List<StreamingCollection<BuildInformationNode>> m_commandCollections = new List<StreamingCollection<BuildInformationNode>>();

    public BuildInformationMerger(IVssRequestContext requestContext, IList<string> informationTypes)
    {
      this.m_requestContext = requestContext;
      this.m_informationTypes = informationTypes;
      this.m_db = requestContext.CreateComponent<BuildInformationComponent>("Build");
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "BuildInformationMerger constructed");
    }

    public void Dispose()
    {
      if (this.m_resultCollection != null)
      {
        this.m_resultCollection.Dispose();
        this.m_resultCollection = (ResultCollection) null;
      }
      if (this.m_db == null)
        return;
      this.m_db.Dispose();
      this.m_db = (BuildInformationComponent) null;
    }

    public void Enqueue(
      BuildDetail build,
      StreamingCollection<BuildInformationNode> buildInformation)
    {
      this.m_requestContext.TraceEnter(0, "Build", "Command", nameof (Enqueue));
      buildInformation.IsComplete = false;
      this.m_builds.Add(build);
      this.m_commandCollections.Add(buildInformation);
      this.m_requestContext.TraceLeave(0, "Build", "Command", nameof (Enqueue));
    }

    public bool TryMergeNext()
    {
      this.m_requestContext.TraceEnter(0, "Build", "Command", nameof (TryMergeNext));
      BuildInformationNode buildInformationNode = (BuildInformationNode) null;
      if (this.m_builds.Count > 0)
      {
        if (!this.m_initialized)
        {
          this.m_initialized = true;
          this.m_requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Querying build information");
          this.m_resultCollection = this.m_db.QueryBuildInformation((ICollection<BuildDetail>) this.m_builds, this.m_informationTypes);
          if (this.m_db.ServiceVersion <= ServiceVersion.V2)
          {
            this.m_buildInformationBinder = this.m_resultCollection.GetCurrent<BuildInformationRow>();
            this.m_hasMoreData = this.m_buildInformationBinder.MoveNext();
          }
          else
            this.m_buildInformationBinder2 = this.m_resultCollection.GetCurrent<BuildInformationNode>();
        }
        if (this.m_db.ServiceVersion > ServiceVersion.V2)
          this.m_hasMoreData = this.m_buildInformationBinder2.MoveNext();
        if (this.m_hasMoreData)
        {
          this.m_requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Having more data to merge");
          buildInformationNode = this.m_db.ServiceVersion > ServiceVersion.V2 ? this.m_buildInformationBinder2.Current : BuildInformationMerger.ReadNode(this.m_buildInformationBinder, ref this.m_hasMoreData);
          if (this.m_groupId == int.MinValue)
          {
            this.m_groupId = buildInformationNode.GroupId;
            this.m_requestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Set group Id to '{0}'", (object) this.m_groupId);
          }
          while (!VssStringComparer.Url.Equals(this.m_builds[0].Uri, buildInformationNode.BuildUri) || this.m_groupId != buildInformationNode.GroupId)
          {
            this.m_builds.RemoveAt(0);
            this.m_commandCollections[0].IsComplete = true;
            this.m_commandCollections.RemoveAt(0);
            this.m_groupId = buildInformationNode.GroupId;
            if (this.m_builds.Count <= 0)
              goto label_13;
          }
          this.m_requestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Stopped at build information node '<{0},{1}>'", (object) this.m_builds[0].Uri, (object) this.m_groupId);
label_13:
          if (this.m_builds.Count > 0)
          {
            this.m_commandCollections[0].Enqueue(buildInformationNode);
            this.m_requestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read build information node '<{0},{1}>'", (object) buildInformationNode.BuildUri, (object) buildInformationNode.GroupId);
          }
        }
        else
        {
          this.m_requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Having no more data to merge");
          while (this.m_builds.Count > 0)
          {
            this.m_requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Skipping build '{0}'", (object) this.m_builds[0].Uri);
            this.m_builds.RemoveAt(0);
            this.m_commandCollections[0].IsComplete = true;
            this.m_commandCollections.RemoveAt(0);
          }
        }
      }
      if (!this.m_hasMoreData)
      {
        if (this.m_resultCollection != null)
        {
          this.m_resultCollection.Dispose();
          this.m_resultCollection = (ResultCollection) null;
        }
        this.m_initialized = false;
        this.m_buildInformationBinder = (ObjectBinder<BuildInformationRow>) null;
        this.m_buildInformationBinder2 = (ObjectBinder<BuildInformationNode>) null;
      }
      this.m_requestContext.TraceLeave(0, "Build", "Command", nameof (TryMergeNext));
      return buildInformationNode != null;
    }

    internal static BuildInformationNode ReadNode(
      ObjectBinder<BuildInformationRow> rowBinder,
      ref bool hasMoreData)
    {
      BuildInformationNode buildInformationNode = new BuildInformationNode()
      {
        BuildUri = rowBinder.Current.BuildUri,
        GroupId = rowBinder.Current.GroupId,
        LastModifiedBy = rowBinder.Current.LastModifiedBy,
        LastModifiedDate = rowBinder.Current.LastModifiedDate,
        NodeId = rowBinder.Current.NodeId,
        ParentId = rowBinder.Current.ParentId,
        Type = rowBinder.Current.Type
      };
      if (!string.IsNullOrEmpty(rowBinder.Current.FieldName))
        buildInformationNode.Fields.Add(new InformationField(rowBinder.Current.FieldName, rowBinder.Current.FieldValue));
      while (hasMoreData = rowBinder.MoveNext())
      {
        BuildInformationRow current = rowBinder.Current;
        if (current.GroupId == buildInformationNode.GroupId && current.NodeId == buildInformationNode.NodeId)
          buildInformationNode.Fields.Add(new InformationField(current.FieldName, current.FieldValue));
        else
          break;
      }
      return buildInformationNode;
    }
  }
}
