// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildInformationMerger2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class BuildInformationMerger2010 : IDisposable
  {
    private bool m_hasMoreData;
    private bool m_initialized;
    private int m_groupId = int.MinValue;
    private IList<string> m_informationTypes;
    private BuildInformation2010Component m_db;
    private ResultCollection m_resultCollection;
    private IVssRequestContext m_requestContext;
    private ObjectBinder<BuildInformationRow> m_buildInformationBinder;
    private ObjectBinder<BuildInformationNode2010> m_buildInformationBinder2;
    private List<BuildInfo> m_buildInfos = new List<BuildInfo>();
    private List<StreamingCollection<BuildInformationNode2010>> m_commandCollections = new List<StreamingCollection<BuildInformationNode2010>>();

    public BuildInformationMerger2010(
      IVssRequestContext requestContext,
      IList<string> informationTypes)
    {
      this.m_informationTypes = informationTypes;
      this.m_requestContext = requestContext;
      this.m_db = requestContext.CreateComponent<BuildInformation2010Component>("Build");
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "BuildInformationMerger2010 constructed");
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
      this.m_db = (BuildInformation2010Component) null;
    }

    public void Enqueue(
      Guid projectId,
      string buildUri,
      int queueId,
      StreamingCollection<BuildInformationNode2010> buildInformation)
    {
      this.m_requestContext.TraceEnter(0, "Build", "Command", nameof (Enqueue));
      buildInformation.IsComplete = false;
      this.m_buildInfos.Add(new BuildInfo(projectId, buildUri, queueId));
      this.m_commandCollections.Add(buildInformation);
      this.m_requestContext.TraceLeave(0, "Build", "Command", nameof (Enqueue));
    }

    public bool TryMergeNext()
    {
      this.m_requestContext.TraceEnter(0, "Build", "Command", nameof (TryMergeNext));
      BuildInformationNode2010 node = (BuildInformationNode2010) null;
      if (this.m_buildInfos.Count > 0)
      {
        if (!this.m_initialized)
        {
          this.m_initialized = true;
          this.m_requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Querying build information");
          this.m_resultCollection = this.m_db.QueryBuildInformation((IEnumerable<BuildInfo>) this.m_buildInfos, this.m_informationTypes);
          if (this.m_db.ServiceVersion <= ServiceVersion.V2)
          {
            this.m_buildInformationBinder = this.m_resultCollection.GetCurrent<BuildInformationRow>();
            this.m_hasMoreData = this.m_buildInformationBinder.MoveNext();
          }
          else
            this.m_buildInformationBinder2 = this.m_resultCollection.GetCurrent<BuildInformationNode2010>();
        }
        if (this.m_db.ServiceVersion > ServiceVersion.V2)
          this.m_hasMoreData = this.m_buildInformationBinder2.MoveNext();
        if (this.m_hasMoreData)
        {
          this.m_requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Having more data to merge");
          node = this.m_db.ServiceVersion > ServiceVersion.V2 ? this.m_buildInformationBinder2.Current : this.ReadNode(this.m_buildInformationBinder, ref this.m_hasMoreData);
          if (this.m_groupId == int.MinValue)
          {
            this.m_groupId = node.GroupId;
            this.m_requestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Set group Id to '{0}'", (object) this.m_groupId);
          }
          do
          {
            VssStringComparer url = VssStringComparer.Url;
            BuildInfo buildInfo = this.m_buildInfos[0];
            string uri1 = buildInfo.Uri;
            string buildUri = node.BuildUri;
            if (url.Equals(uri1, buildUri) && this.m_groupId == node.GroupId)
            {
              IVssRequestContext requestContext = this.m_requestContext;
              buildInfo = this.m_buildInfos[0];
              string uri2 = buildInfo.Uri;
              // ISSUE: variable of a boxed type
              __Boxed<int> groupId = (ValueType) this.m_groupId;
              requestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Stopped at build information node '<{0},{1}>'", (object) uri2, (object) groupId);
              break;
            }
            this.m_buildInfos.RemoveAt(0);
            this.m_commandCollections[0].IsComplete = true;
            this.m_commandCollections.RemoveAt(0);
            this.m_groupId = node.GroupId;
          }
          while (this.m_buildInfos.Count > 0);
          if (this.m_buildInfos.Count > 0)
          {
            this.FilterAndEnqueue(node);
            this.m_requestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Filtered and enqueued build information node '<{0},{1}>'", (object) node.BuildUri, (object) node.GroupId);
          }
        }
        else
        {
          this.m_requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Having no more data to merge");
          while (this.m_buildInfos.Count > 0)
          {
            this.m_requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Skipping build '{0}'", (object) this.m_buildInfos[0].Uri);
            this.m_buildInfos.RemoveAt(0);
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
        this.m_buildInformationBinder2 = (ObjectBinder<BuildInformationNode2010>) null;
      }
      this.m_requestContext.TraceLeave(0, "Build", "Command", nameof (TryMergeNext));
      return node != null;
    }

    private BuildInformationNode2010 ReadNode(
      ObjectBinder<BuildInformationRow> rowBinder,
      ref bool hasMoreData)
    {
      BuildInformationNode2010 informationNode2010 = new BuildInformationNode2010()
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
        informationNode2010.Fields.Add(new InformationField2010(rowBinder.Current.FieldName, rowBinder.Current.FieldValue));
      while (hasMoreData = rowBinder.MoveNext())
      {
        BuildInformationRow current = rowBinder.Current;
        if (current.GroupId == informationNode2010.GroupId && current.NodeId == informationNode2010.NodeId)
          informationNode2010.Fields.Add(new InformationField2010(current.FieldName, current.FieldValue));
        else
          break;
      }
      return informationNode2010;
    }

    private void FilterAndEnqueue(BuildInformationNode2010 node)
    {
      if (TFStringComparer.InformationType.Equals(InformationTypes.CheckInOutcome, node.Type) || TFStringComparer.InformationType.Equals(InformationTypes.ReshelvedShelveset, node.Type))
      {
        foreach (InformationField2010 field in node.Fields)
        {
          int result;
          if (field.Name == InformationFields.RequestId && (!int.TryParse(field.Value, out result) || result != this.m_buildInfos[0].QueueId))
            return;
        }
      }
      this.m_commandCollections[0].Enqueue(node);
    }

    private static void ConvertNode(BuildInformationNode2010 current)
    {
      if (TFStringComparer.InformationType.StartsWith(current.Type, "BuildStep#ActivityTracking"))
      {
        if (TFStringComparer.InformationType.EndsWith(current.Type, "TestConfiguration") || TFStringComparer.InformationType.EndsWith(current.Type, "CompileConfiguration"))
        {
          int num1 = -1;
          int index1 = -1;
          int num2 = -1;
          for (int index2 = 0; index2 < current.Fields.Count; ++index2)
          {
            if (current.Fields[index2].Name == InformationFields.Platform)
              num2 = index2;
            else if (current.Fields[index2].Name == InformationFields.Flavor)
              num1 = index2;
            else if (current.Fields[index2].Name == InformationFields.Message)
              index1 = index2;
          }
          if (num2 >= 0 && num1 >= 0)
          {
            InformationField2010 informationField2010;
            if (index1 >= 0)
            {
              informationField2010 = current.Fields[index1];
            }
            else
            {
              informationField2010 = new InformationField2010()
              {
                Name = InformationFields.Message
              };
              current.Fields.Add(informationField2010);
            }
            informationField2010.Value = !TFStringComparer.InformationType.EndsWith(current.Type, "TestConfiguration") ? ResourceStrings.BuildStepTopLevelCompilationMessage((object) current.Fields[num2].Value, (object) current.Fields[num1].Value) : ResourceStrings.BuildStepTopLevelTestMessage((object) current.Fields[num2].Value, (object) current.Fields[num1].Value);
            current.Fields.RemoveAt(Math.Max(num2, num1));
            current.Fields.RemoveAt(Math.Min(num2, num1));
          }
        }
        current.Type = InformationTypes.BuildStep;
      }
      else
      {
        if (!TFStringComparer.InformationType.Equals(current.Type, "BuildStep#BuildProject"))
          return;
        current.Type = InformationTypes.BuildStep;
        for (int index = 0; index < current.Fields.Count; ++index)
        {
          if (current.Fields[index].Name == InformationFields.Name)
          {
            string str = !VersionControlPath.IsServerItem(current.Fields[index].Value) ? Path.GetFileName(current.Fields[index].Value) : VersionControlPath.GetFileName(current.Fields[index].Value);
            current.Fields.Add(new InformationField2010()
            {
              Name = InformationFields.Message,
              Value = ResourceStrings.BuildStepProjectCompilationMessage((object) str)
            });
            break;
          }
        }
      }
    }

    private class TestInformation2008
    {
      private bool IncludeTestSummaries { get; set; }

      private bool IncludeCodeCoverageSummaries { get; set; }

      private Dictionary<string, BuildInformationNode2010> ConfigurationNodes { get; set; }

      private int MaxNodeId { get; set; }

      public TestInformation2008(IList<string> informationTypes)
      {
        bool flag = informationTypes.Count == 1 && TFStringComparer.InformationType.Equals(informationTypes[0], BuildConstants.Star);
        this.IncludeTestSummaries = flag || informationTypes.Contains<string>(InformationTypes.TestSummary, (IEqualityComparer<string>) TFStringComparer.InformationType);
        this.IncludeCodeCoverageSummaries = flag || informationTypes.Contains<string>(InformationTypes.CodeCoverageSummary, (IEqualityComparer<string>) TFStringComparer.InformationType);
      }

      public void AddIfConfiguration(BuildInformationNode2010 node)
      {
        if (this.MaxNodeId < node.NodeId)
          this.MaxNodeId = node.NodeId;
        string platformFlavorKey = this.GetPlatformFlavorKey(node);
        if (string.IsNullOrEmpty(platformFlavorKey))
          return;
        if (this.ConfigurationNodes == null)
          this.ConfigurationNodes = new Dictionary<string, BuildInformationNode2010>();
        this.ConfigurationNodes.Add(platformFlavorKey, node);
      }

      public void EnqueueTestNodes(
        IVssRequestContext context,
        StreamingCollection<BuildInformationNode2010> commandCollection,
        string buildUri)
      {
        this.ConfigurationNodes = (Dictionary<string, BuildInformationNode2010>) null;
      }

      private void EnqueueConfigurationChildren(
        StreamingCollection<BuildInformationNode2010> commandCollection,
        Dictionary<string, List<BuildInformationNode2010>> configurationChildren)
      {
        if (this.ConfigurationNodes == null)
          return;
        foreach (KeyValuePair<string, BuildInformationNode2010> configurationNode in this.ConfigurationNodes)
        {
          if (configurationChildren.ContainsKey(configurationNode.Key))
          {
            foreach (BuildInformationNode2010 informationNode2010 in configurationChildren[configurationNode.Key])
            {
              informationNode2010.ParentId = configurationNode.Value.NodeId;
              commandCollection.Enqueue(informationNode2010);
            }
          }
        }
      }

      private string GetPlatformFlavorKey(BuildInformationNode2010 parentNode)
      {
        if (TFStringComparer.InformationType.Equals(parentNode.Type, InformationTypes.ConfigurationSummary))
        {
          int index1 = -1;
          int index2 = -1;
          for (int index3 = 0; index3 < parentNode.Fields.Count; ++index3)
          {
            if (parentNode.Fields[index3].Name == InformationFields.Platform)
              index2 = index3;
            else if (parentNode.Fields[index3].Name == InformationFields.Flavor)
              index1 = index3;
          }
          if (index2 >= 0 && index1 >= 0)
            return parentNode.Fields[index2].Value + "|" + parentNode.Fields[index1].Value;
        }
        return (string) null;
      }
    }
  }
}
