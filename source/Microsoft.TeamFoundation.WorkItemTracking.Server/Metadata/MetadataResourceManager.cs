// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.MetadataResourceManager
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class MetadataResourceManager
  {
    public static IReadOnlyCollection<string> GetSystemProcessTypeletResources(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<string>) new List<string>()
      {
        MetadataResourceManager.GetSystemProcessMetadataResource(requestContext, ProcessMetadataResourceType.BehaviorMetadata, "System.OrderedBehavior"),
        MetadataResourceManager.GetSystemProcessMetadataResource(requestContext, ProcessMetadataResourceType.BehaviorMetadata, "System.TaskBacklogBehavior"),
        MetadataResourceManager.GetSystemProcessMetadataResource(requestContext, ProcessMetadataResourceType.BehaviorMetadata, "System.RequirementBacklogBehavior"),
        MetadataResourceManager.GetSystemProcessMetadataResource(requestContext, ProcessMetadataResourceType.BehaviorMetadata, "System.PortfolioBacklogBehavior"),
        MetadataResourceManager.GetSystemProcessMetadataResource(requestContext, ProcessMetadataResourceType.WorkItemTypeDefinitionMetadata, "System.WorkItemType")
      };
    }

    public static IReadOnlyCollection<string> GetAgileProcessTypeletResources(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      return (IReadOnlyCollection<string>) new List<string>()
      {
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "System.RequirementBacklogBehavior"),
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "Microsoft.VSTS.Agile.EpicBacklogBehavior"),
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "Microsoft.VSTS.Agile.FeatureBacklogBehavior")
      };
    }

    public static IReadOnlyCollection<string> GetCMMIProcessTypeletResources(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      return (IReadOnlyCollection<string>) new List<string>()
      {
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "System.TaskBacklogBehavior"),
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "System.RequirementBacklogBehavior"),
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "Microsoft.VSTS.CMMI.EpicBacklogBehavior"),
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "Microsoft.VSTS.CMMI.FeatureBacklogBehavior")
      };
    }

    public static IReadOnlyCollection<string> GetScrumProcessTypeletResources(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      return (IReadOnlyCollection<string>) new List<string>()
      {
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "System.OrderedBehavior"),
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "System.RequirementBacklogBehavior"),
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "Microsoft.VSTS.Scrum.EpicBacklogBehavior"),
        MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "Microsoft.VSTS.Scrum.FeatureBacklogBehavior")
      };
    }

    public static IReadOnlyCollection<string> GetHydroProcessTypeletResources(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      List<string> typeletResources = new List<string>();
      string resourceValuesXml1 = MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "Microsoft.VSTS.Basic.EpicBacklogBehavior");
      typeletResources.Add(resourceValuesXml1);
      requestContext.Trace(911531, TraceLevel.Info, "WorkItemType", "MetadataService", string.Format("Microsoft.VSTS.Basic.EpicBacklogBehavior : {0}", (object) resourceValuesXml1));
      string resourceValuesXml2 = MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, processTypeId, ProcessMetadataResourceType.BehaviorMetadata, "System.RequirementBacklogBehavior");
      requestContext.Trace(911531, TraceLevel.Info, "WorkItemType", "MetadataService", string.Format("System.RequirementBacklogBehavior : {0}", (object) resourceValuesXml2));
      typeletResources.Add(resourceValuesXml2);
      return (IReadOnlyCollection<string>) typeletResources;
    }

    public static string GetSystemProcessMetadataResource(
      IVssRequestContext requestContext,
      ProcessMetadataResourceType resourceType,
      string resourceName)
    {
      Guid systemProcessId = ProcessConstants.SystemProcessId;
      return MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, systemProcessId, resourceType, resourceName);
    }

    private static string GetProcessMetadataResourceValuesXML(
      IVssRequestContext requestContext,
      Guid processTypeId,
      ProcessMetadataResourceType resourceType,
      string resourceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext.GetService<ITeamFoundationProcessService>();
      using (Stream processMetadataFile = context.GetService<ITeamFoundationProcessMetadataFilesService>().GetProcessMetadataFile(requestContext, processTypeId, resourceType, resourceName))
      {
        if (processMetadataFile == null)
          throw new ArgumentException(string.Format("For process : {0} , Resource type : {1} and Resource name : {2} does not exist )", (object) processTypeId, (object) resourceType, (object) resourceName));
        processMetadataFile.Seek(0L, SeekOrigin.Begin);
        using (StreamReader streamReader = new StreamReader(processMetadataFile))
          return streamReader.ReadToEnd();
      }
    }

    public static string GetResourceXMLForProcessWorkItemType(
      IVssRequestContext requestContext,
      ProcessDescriptor systemDescriptor,
      string workItemTypeReferenceName,
      ProcessMetadataResourceType resourceType)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(systemDescriptor, nameof (systemDescriptor));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      string lowerInvariant = workItemTypeReferenceName.ToLowerInvariant();
      return MetadataResourceManager.GetProcessMetadataResourceValuesXML(requestContext, systemDescriptor.TypeId, resourceType, lowerInvariant);
    }
  }
}
