// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildArtifactProvider
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server
{
  public static class BuildArtifactProvider
  {
    public static void WriteBuildReport(HttpContext context)
    {
      IVssRequestContext vssRequestContext = ((VisualStudioServicesApplication) context.ApplicationInstance).VssRequestContext;
      vssRequestContext.ValidateIdentity();
      BuildArtifactProvider.WriteV2Report(vssRequestContext, context);
    }

    private static void WriteV2Report(IVssRequestContext requestContext, HttpContext context)
    {
      context.Response.ContentType = "text/xml";
      string str1 = requestContext.ServiceHost.StaticContentDirectory.TrimEnd('/') + "/Build";
      string text = "type=\"text/xsl\" href=\"" + str1 + "/v2.0/transforms/" + requestContext.ServiceHost.GetCulture(requestContext).LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "/Build.xsl\"";
      using (XmlTextWriter writer = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8))
      {
        writer.Formatting = Formatting.Indented;
        writer.WriteStartDocument();
        writer.WriteProcessingInstruction("xml-stylesheet", text);
        TeamFoundationDataReader foundationDataReader = (TeamFoundationDataReader) null;
        try
        {
          BuildController controller = (BuildController) null;
          BuildDetail build = (BuildDetail) null;
          BuildDefinition definition = (BuildDefinition) null;
          List<QueuedBuild> queuedBuilds = new List<QueuedBuild>();
          string str2 = context.Request.QueryString["artifactMoniker"];
          string buildPath = (string) null;
          string buildNumber = (string) null;
          TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
          if (string.IsNullOrEmpty(str2))
          {
            using (requestContext.CreateComponent<BuildComponent>("Build"))
            {
              string rootPath = context.Request.QueryString["teamProject"];
              buildNumber = context.Request.QueryString["buildNumber"];
              buildPath = context.Request.QueryString["definitionPath"];
              if (!string.IsNullOrEmpty(buildNumber))
              {
                if (!string.IsNullOrEmpty(rootPath) && string.IsNullOrEmpty(buildPath))
                  buildPath = BuildPath.Root(rootPath, BuildConstants.Star);
                if (!string.IsNullOrEmpty(buildPath))
                {
                  foundationDataReader = service.QueryBuilds(requestContext, (IList<BuildDetailSpec>) new BuildDetailSpec[1]
                  {
                    new BuildDetailSpec(BuildPath.GetFullPath(buildPath), buildNumber)
                    {
                      QueryOptions = QueryOptions.Workspaces | QueryOptions.Controllers | QueryOptions.BatchedRequests,
                      InformationTypes = {
                        BuildConstants.Star
                      }
                    }
                  }, new Guid());
                  using (IEnumerator<BuildQueryResult> enumerator = foundationDataReader.CurrentEnumerable<BuildQueryResult>().GetEnumerator())
                  {
                    if (enumerator.MoveNext())
                    {
                      if (enumerator.Current.Builds.MoveNext())
                      {
                        build = enumerator.Current.Builds.Current;
                        definition = enumerator.Current.Definitions[0];
                        controller = enumerator.Current.Controllers.Count > 0 ? enumerator.Current.Controllers[0] : (BuildController) null;
                        queuedBuilds.AddRange((IEnumerable<QueuedBuild>) enumerator.Current.QueuedBuilds);
                      }
                    }
                  }
                }
              }
            }
          }
          else
          {
            foundationDataReader = service.QueryBuildsByUri(requestContext, (IList<string>) new string[1]
            {
              DBHelper.CreateArtifactUri("Build", str2)
            }, (IList<string>) BuildConstants.AllInformationTypes, QueryOptions.Workspaces | QueryOptions.Controllers | QueryOptions.BatchedRequests, QueryDeletedOption.ExcludeDeleted, new Guid(), false);
            BuildQueryResult buildQueryResult = foundationDataReader.Current<BuildQueryResult>();
            if (buildQueryResult.Builds.MoveNext())
            {
              build = buildQueryResult.Builds.Current;
              definition = buildQueryResult.Definitions.Count > 0 ? buildQueryResult.Definitions[0] : (BuildDefinition) null;
              controller = buildQueryResult.Controllers.Count > 0 ? buildQueryResult.Controllers[0] : (BuildController) null;
              queuedBuilds.AddRange((IEnumerable<QueuedBuild>) buildQueryResult.QueuedBuilds);
            }
          }
          if (build == null)
          {
            if (!string.IsNullOrEmpty(str2))
              throw new InvalidBuildUriException(str2);
            if (!string.IsNullOrEmpty(buildPath) && !string.IsNullOrEmpty(buildNumber))
              throw new BuildServerException(ResourceStrings.BuildDoesNotExistForSpec((object) buildPath, (object) buildNumber));
            throw new BuildServerException(ResourceStrings.ArtifactUrlHelp());
          }
          writer.WriteStartElement("Report");
          DateTime now = DateTime.Now;
          string empty = string.Empty;
          string str3 = !TimeZone.CurrentTimeZone.IsDaylightSavingTime(now) ? TimeZone.CurrentTimeZone.StandardName : TimeZone.CurrentTimeZone.DaylightName;
          writer.WriteElementString("ReportTime", now.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          writer.WriteElementString("TimeZoneName", str3);
          writer.WriteElementString("TimeZoneOffset", TFCommonUtil.GetLocalTimeZoneOffset(now));
          writer.WriteElementString("BaseUrl", str1);
          BuildArtifactProvider.WriteBuildControllerXml(writer, controller);
          BuildArtifactProvider.WriteBuildDefinitionXml(writer, definition);
          BuildArtifactProvider.WriteBuildXml(requestContext, writer, build, (IList<QueuedBuild>) queuedBuilds);
          writer.WriteEndElement();
        }
        catch (Exception ex)
        {
          writer.WriteStartElement("Exception");
          writer.WriteElementString("ExceptionType", ex.GetType().Name);
          writer.WriteElementString("Message", ex.Message);
          writer.WriteEndElement();
        }
        finally
        {
          foundationDataReader?.Dispose();
        }
        writer.WriteEndDocument();
      }
    }

    private static void WriteBuildControllerXml(XmlTextWriter writer, BuildController controller)
    {
      writer.WriteStartElement("BuildController");
      if (controller != null)
      {
        writer.WriteAttributeString("Uri", controller.Uri);
        writer.WriteAttributeString("Name", controller.Name);
        writer.WriteAttributeString("Status", CommonInformationHelper.EnumToString<ControllerStatus>(controller.Status));
        writer.WriteAttributeString("Enabled", controller.Enabled ? "1" : "0");
        writer.WriteAttributeString("CustomAssemblyPath", controller.CustomAssemblyPath);
        writer.WriteAttributeString("DateCreated", controller.DateCreated.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteAttributeString("DateUpdated", controller.DateUpdated.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteAttributeString("MaxConcurrentBuilds", controller.MaxConcurrentBuilds.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteAttributeString("Url", controller.Url);
        writer.WriteElementString("Description", controller.Description);
        writer.WriteElementString("StatusMessage", controller.StatusMessage);
      }
      writer.WriteEndElement();
    }

    private static void WriteBuildDefinitionXml(XmlTextWriter writer, BuildDefinition definition)
    {
      writer.WriteStartElement("BuildDefinition");
      if (definition != null)
      {
        writer.WriteAttributeString("Uri", definition.Uri);
        writer.WriteAttributeString("TeamProject", definition.TeamProject.Name);
        writer.WriteAttributeString("Name", definition.Name);
        writer.WriteAttributeString("FullPath", definition.FullPath);
        writer.WriteAttributeString("DefaultDropLocation", definition.DefaultDropLocation);
        writer.WriteAttributeString("ConfigurationFolderUri", string.Empty);
        writer.WriteAttributeString("ContinuousIntegrationType", CommonInformationHelper.EnumToString<DefinitionTriggerType>(definition.TriggerType));
        writer.WriteAttributeString("ContinuousIntegrationQuietPeriod", definition.ContinuousIntegrationQuietPeriod.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteAttributeString("QueueStatus", definition.QueueStatus.ToString());
        writer.WriteAttributeString("LastBuildUri", definition.LastBuildUri);
        writer.WriteAttributeString("LastGoodBuildUri", definition.LastGoodBuildUri);
        writer.WriteAttributeString("LastGoodBuildLabel", definition.LastGoodBuildLabel);
        writer.WriteElementString("Description", definition.Description);
        writer.WriteStartElement("RetentionPolicies");
        foreach (RetentionPolicy retentionPolicy in definition.RetentionPolicies)
        {
          writer.WriteStartElement("RetentionPolicy");
          writer.WriteAttributeString("BuildReason", CommonInformationHelper.EnumToString<BuildReason>(retentionPolicy.BuildReason));
          writer.WriteAttributeString("BuildStatus", CommonInformationHelper.EnumToString<BuildStatus>(retentionPolicy.BuildStatus));
          writer.WriteAttributeString("NumberToKeep", retentionPolicy.NumberToKeep.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          writer.WriteAttributeString("DeleteOptions", CommonInformationHelper.EnumToString<DeleteOptions>(retentionPolicy.DeleteOptions));
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteStartElement("Schedules");
        foreach (Schedule schedule in definition.Schedules)
        {
          writer.WriteStartElement("Schedule");
          writer.WriteAttributeString("UtcStartTime", schedule.UtcStartTime.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          writer.WriteAttributeString("UtcDaysToBuild", CommonInformationHelper.EnumToString<ScheduleDays>(schedule.UtcDaysToBuild));
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteStartElement("WorkspaceTemplate");
        writer.WriteAttributeString("LastModifiedDate", BuildCommonUtil.DateTimeToString(definition.WorkspaceTemplate.LastModifiedDate.ToLocalTime()));
        writer.WriteAttributeString("LastModifiedBy", definition.WorkspaceTemplate.LastModifiedBy);
        writer.WriteStartElement("Mappings");
        foreach (WorkspaceMapping mapping in definition.WorkspaceTemplate.Mappings)
        {
          writer.WriteStartElement("WorkspaceMapping");
          writer.WriteAttributeString("ServerItem", mapping.ServerItem);
          writer.WriteAttributeString("LocalItem", mapping.LocalItem);
          writer.WriteAttributeString("MappingType", CommonInformationHelper.EnumToString<WorkspaceMappingType>(mapping.MappingType));
          writer.WriteAttributeString("Depth", mapping.Depth.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    private static string GetDisplayText(BuildStatus status)
    {
      switch (status)
      {
        case BuildStatus.InProgress:
          return BuildTypeResource.Status_InProgress();
        case BuildStatus.Succeeded:
          return BuildTypeResource.Status_Succeeded();
        case BuildStatus.PartiallySucceeded:
          return BuildTypeResource.Status_PartiallySucceeded();
        case BuildStatus.Failed:
          return BuildTypeResource.Status_Failed();
        case BuildStatus.Stopped:
          return BuildTypeResource.Status_Stopped();
        case BuildStatus.NotStarted:
          return BuildTypeResource.Status_NotStarted();
        default:
          return string.Empty;
      }
    }

    private static void WriteBuildXml(
      IVssRequestContext requestContext,
      XmlTextWriter writer,
      BuildDetail build,
      IList<QueuedBuild> queuedBuilds)
    {
      writer.WriteStartElement("BuildDetail");
      writer.WriteAttributeString("Uri", build.Uri);
      writer.WriteAttributeString("BuildNumber", build.BuildNumber);
      writer.WriteAttributeString("BuildControllerUri", build.BuildControllerUri);
      writer.WriteAttributeString("BuildDefinitionUri", build.BuildDefinitionUri);
      writer.WriteAttributeString("StartTime", BuildCommonUtil.DateTimeToString(build.StartTime.ToLocalTime()));
      writer.WriteAttributeString("FinishTime", BuildCommonUtil.DateTimeToString(build.FinishTime.ToLocalTime()));
      writer.WriteAttributeString("Status", CommonInformationHelper.EnumToString<BuildStatus>(build.Status));
      writer.WriteAttributeString("DisplayStatus", BuildArtifactProvider.GetDisplayText(build.Status));
      writer.WriteAttributeString("CompilationStatus", CommonInformationHelper.EnumToString<BuildPhaseStatus>(build.CompilationStatus));
      writer.WriteAttributeString("TestStatus", CommonInformationHelper.EnumToString<BuildPhaseStatus>(build.TestStatus));
      writer.WriteAttributeString("DropLocation", build.DropLocation);
      writer.WriteAttributeString("LogLocation", build.LogLocation);
      writer.WriteAttributeString("SourceGetVersion", build.SourceGetVersion);
      writer.WriteAttributeString("LastChangedOn", BuildCommonUtil.DateTimeToString(build.LastChangedOn.ToLocalTime()));
      writer.WriteAttributeString("LastChangedBy", build.LastChangedBy);
      writer.WriteAttributeString("LabelName", build.LabelName);
      writer.WriteAttributeString("KeepForever", build.KeepForever.ToString());
      writer.WriteAttributeString("Quality", build.Quality);
      writer.WriteStartElement("ProcessParameters");
      writer.WriteRaw(build.ProcessParameters);
      writer.WriteEndElement();
      BuildArtifactProvider.WriteBuildRequests(requestContext, writer, queuedBuilds);
      BuildArtifactProvider.WriteBuildInformationXml(requestContext, writer, build.Information);
      writer.WriteEndElement();
    }

    private static void WriteBuildRequests(
      IVssRequestContext requestContext,
      XmlTextWriter writer,
      IList<QueuedBuild> queuedBuilds)
    {
      writer.WriteStartElement("Requests");
      foreach (QueuedBuild queuedBuild in (IEnumerable<QueuedBuild>) queuedBuilds)
      {
        writer.WriteStartElement("QueuedBuild");
        writer.WriteAttributeString("Id", queuedBuild.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteAttributeString("BatchId", queuedBuild.BatchId.ToString("D"));
        writer.WriteAttributeString("BuildControllerUri", queuedBuild.BuildControllerUri);
        writer.WriteAttributeString("BuildDefinitionUri", queuedBuild.BuildDefinitionUri);
        writer.WriteAttributeString("CustomGetVersion", queuedBuild.CustomGetVersion);
        writer.WriteAttributeString("DropLocation", queuedBuild.DropLocation);
        writer.WriteAttributeString("GetOption", queuedBuild.GetOption.ToString());
        writer.WriteAttributeString("Priority", queuedBuild.Priority.ToString());
        writer.WriteAttributeString("QueuePosition", queuedBuild.QueuePosition.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteAttributeString("QueueTime", BuildCommonUtil.DateTimeToString(queuedBuild.QueueTime));
        writer.WriteAttributeString("Reason", queuedBuild.Reason.ToString());
        writer.WriteAttributeString("RequestedBy", queuedBuild.RequestedBy);
        writer.WriteAttributeString("RequestedFor", queuedBuild.RequestedFor);
        writer.WriteAttributeString("ShelvesetName", queuedBuild.ShelvesetName);
        writer.WriteAttributeString("Status", queuedBuild.Status.ToString());
        writer.WriteAttributeString("TeamProject", queuedBuild.TeamProject);
        writer.WriteStartElement("ProcessParameters");
        writer.WriteRaw(queuedBuild.ProcessParameters);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    private static void WriteBuildInformationXml(
      IVssRequestContext requestContext,
      XmlTextWriter writer,
      StreamingCollection<BuildInformationNode> buildInformation)
    {
      TswaServerHyperlinkService service = requestContext.GetService<TswaServerHyperlinkService>();
      Dictionary<int, List<BuildInformationNode>> nodeParentDict = new Dictionary<int, List<BuildInformationNode>>();
      List<BuildInformationNode> buildInformationNodeList;
      foreach (BuildInformationNode buildInformationNode in buildInformation)
      {
        if (!nodeParentDict.TryGetValue(buildInformationNode.ParentId, out buildInformationNodeList))
        {
          buildInformationNodeList = new List<BuildInformationNode>();
          nodeParentDict.Add(buildInformationNode.ParentId, buildInformationNodeList);
        }
        buildInformationNodeList.Add(buildInformationNode);
      }
      writer.WriteStartElement("Information");
      if (nodeParentDict.TryGetValue(0, out buildInformationNodeList))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        buildInformationNodeList.Sort(BuildArtifactProvider.\u003C\u003EO.\u003C0\u003E__SortBuildInformationNodes ?? (BuildArtifactProvider.\u003C\u003EO.\u003C0\u003E__SortBuildInformationNodes = new Comparison<BuildInformationNode>(BuildArtifactProvider.SortBuildInformationNodes)));
        foreach (BuildInformationNode node in buildInformationNodeList)
          BuildArtifactProvider.WriteBuildInformationNodeXml(writer, requestContext, service, node, nodeParentDict);
      }
      writer.WriteEndElement();
    }

    private static void WriteBuildInformationNodeXml(
      XmlTextWriter writer,
      IVssRequestContext requestContext,
      TswaServerHyperlinkService linkingService,
      BuildInformationNode node,
      Dictionary<int, List<BuildInformationNode>> nodeParentDict)
    {
      List<BuildInformationNode> buildInformationNodeList = (List<BuildInformationNode>) null;
      writer.WriteStartElement("BuildInformationNode");
      writer.WriteAttributeString("Type", node.Type);
      writer.WriteAttributeString("NodeId", node.NodeId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      writer.WriteAttributeString("ParentId", node.ParentId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      foreach (InformationField field in node.Fields)
      {
        string str1 = (string) null;
        writer.WriteStartElement("InformationField");
        writer.WriteElementString("Name", field.Name);
        if (field.Name.Equals(InformationFields.StartTime, StringComparison.Ordinal) || field.Name.Equals(InformationFields.FinishTime, StringComparison.Ordinal))
        {
          DateTime dateTime = CommonInformationHelper.ToDateTime(field.Value);
          writer.WriteElementString("Value", BuildCommonUtil.DateTimeToString(dateTime.ToLocalTime()));
        }
        else
        {
          if (field.Name.Equals(InformationFields.ChangesetUri, StringComparison.Ordinal) || field.Name.Equals(InformationFields.WorkItemUri, StringComparison.Ordinal))
            str1 = field.Value;
          writer.WriteElementString("Value", field.Value);
        }
        writer.WriteEndElement();
        if (!string.IsNullOrEmpty(str1) && Microsoft.TeamFoundation.Build.Common.Validation.IsValidUri(str1, out string _) && LinkingUtilities.IsUriWellFormed(str1))
        {
          string str2 = (string) null;
          ArtifactId artifactId = LinkingUtilities.DecodeUri(str1);
          int result;
          if (int.TryParse(artifactId.ToolSpecificId, out result))
            str2 = !VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "Changeset") ? linkingService.GetWorkItemEditorUrl(requestContext, result).AbsoluteUri : linkingService.GetChangesetDetailsUrl(result).AbsoluteUri;
          if (!string.IsNullOrEmpty(str2))
          {
            writer.WriteStartElement("InformationField");
            writer.WriteElementString("Name", "ExternalUrl");
            writer.WriteElementString("Value", str2);
            writer.WriteEndElement();
          }
        }
      }
      if (nodeParentDict.TryGetValue(node.NodeId, out buildInformationNodeList))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        buildInformationNodeList.Sort(BuildArtifactProvider.\u003C\u003EO.\u003C0\u003E__SortBuildInformationNodes ?? (BuildArtifactProvider.\u003C\u003EO.\u003C0\u003E__SortBuildInformationNodes = new Comparison<BuildInformationNode>(BuildArtifactProvider.SortBuildInformationNodes)));
        foreach (BuildInformationNode node1 in buildInformationNodeList)
          BuildArtifactProvider.WriteBuildInformationNodeXml(writer, requestContext, linkingService, node1, nodeParentDict);
      }
      writer.WriteEndElement();
    }

    private static int SortBuildInformationNodes(BuildInformationNode x, BuildInformationNode y)
    {
      int num = 0;
      InformationField informationField1 = x.Fields.Find((Predicate<InformationField>) (f => f.Name.Equals(InformationFields.StartTime, StringComparison.Ordinal)));
      InformationField informationField2 = y.Fields.Find((Predicate<InformationField>) (f => f.Name.Equals(InformationFields.StartTime, StringComparison.Ordinal)));
      if (informationField1 != null && informationField2 != null)
        num = Comparer<DateTime>.Default.Compare(CommonInformationHelper.ToDateTime(informationField1.Value), CommonInformationHelper.ToDateTime(informationField2.Value));
      if (num == 0)
        num = Comparer<int>.Default.Compare(x.NodeId, y.NodeId);
      return num;
    }
  }
}
