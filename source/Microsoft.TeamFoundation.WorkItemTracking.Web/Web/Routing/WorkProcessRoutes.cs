// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Routing.WorkProcessRoutes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Routing
{
  public class WorkProcessRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionFormLayout, "processDefinitions", "layout", "work/{area}/{processId}/workItemTypes/{witRefName}/{resource}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processDefinitionslayout");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionFormLayoutControls, "processDefinitions", "Controls", "work/{area}/{processId}/workItemTypes/{witRefName}/layout/groups/{groupId}/{resource}/{controlId}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        controlId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processDefinitionsControls");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionFormLayoutGroups, "processDefinitions", "Groups", "work/{area}/{processId}/workItemTypes/{witRefName}/layout/pages/{pageId}/sections/{sectionId}/{resource}/{groupId}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        groupId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processDefinitionsGroups");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionFormLayoutPages, "processDefinitions", "Pages", "work/{area}/{processId}/workItemTypes/{witRefName}/layout/{resource}/{pageId}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        pageId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processDefinitionsPages");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessWorkItemTypesRules, "processes", "rules", "work/{area}/{processId}/workItemTypes/{witRefName}/{resource}/{ruleId}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, 2, (object) new
      {
        ruleId = RouteParameter.Optional
      }, (object) new
      {
        processId = new GuidRouteConstraint()
      }, "processesrules");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessWorkItemTypeFields, "processes", "fields", "work/{area}/{processId}/workItemTypes/{witRefName}/{resource}/{fieldRefName}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, 2, (object) new
      {
        fieldRefName = RouteParameter.Optional
      }, (object) new
      {
        processId = new GuidRouteConstraint()
      }, "processesworkItemTypesfields");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessFields, "processes", "fields", "work/{area}/{processId}/{resource}/{field}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        field = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processesfields");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionFields, "processDefinitions", "fields", "work/{area}/{processId}/{resource}/{field}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        field = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processDefinitionsfields");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionWorkItemTypeFields, "processDefinitions", "workItemTypesFields", "work/{area}/{processId}/workItemTypes/{witRefNameForFields}/fields/{fieldRefName}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        resource = "workItemTypesFields",
        fieldRefName = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processDefinitionsworkItemTypesFields");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionWorkItemTypes, "processDefinitions", "workItemTypes", "work/{area}/{processId}/{resource}/{witRefName}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        witRefName = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processDefinitionsworkItemTypes");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionWorkItemTypeStates, "processDefinitions", "states", "work/{area}/{processId}/workItemTypes/{witRefName}/{resource}/{stateId}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        stateId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "ProcessDefinitions.WorkItemTypesstates");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessesStatesGuid, "processes", "states", "work/{area}/{processId}/workItemTypes/{witRefName}/{resource}/{stateId}", VssRestApiVersion.v3_2, VssRestApiReleaseState.Released, defaults: (object) new
      {
        stateId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "Processes.WorkItemTypesstates");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessBehaviors, "processes", "behaviors", "work/{area}/{processId}/{resource}/{behaviorRefName}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, 2, (object) new
      {
        behaviorRefName = RouteParameter.Optional
      }, (object) new
      {
        processId = new GuidRouteConstraint()
      }, "processesbehaviors");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessWorkItemTypes, "processes", "workItemTypes", "work/{area}/{processId}/{resource}/{witRefName}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, 2, (object) new
      {
        witRefName = RouteParameter.Optional
      }, (object) new
      {
        processId = new GuidRouteConstraint()
      }, "processesworkItemTypes");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessWorkItemTypesBehaviors, "processes", "workItemTypesBehaviors", "work/{area}/{processId}/{resource}/{witRefNameForBehaviors}/behaviors/{behaviorRefName}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released, defaults: (object) new
      {
        behaviorRefName = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "Process.workItemTypesBehaviors");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionWorkItemTypeBehaviors, "processDefinitions", "workItemTypes", "work/{area}/{processId}/{resource}/{witRefNameForBehaviors}/behaviors/{behaviorRefName}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        behaviorRefName = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "ProcessDefinitions.WorkItemTypesbehaviors");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionBehavior, "processDefinitions", "behaviors", "work/{area}/{processId}/{resource}/{behaviorId}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, defaults: (object) new
      {
        behaviorId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processDefinitionsbehaviors");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessesLocationId, "processes", "processes", "work/{resource}/{processTypeId}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, 2, (object) new
      {
        processTypeId = RouteParameter.Optional
      }, routeName: "processesprocesses");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessFormLayout, "processes", "layout", "work/{area}/{processId}/workItemTypes/{witRefName}/{resource}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processeslayout");
      HttpRouteCollection routes1 = routes;
      Guid layoutSystemControls = WorkItemTrackingLocationIds.ProcessesFormLayoutSystemControls;
      object obj = (object) new
      {
        processId = new GuidRouteConstraint()
      };
      var defaults = new
      {
        controlId = RouteParameter.Optional
      };
      object constraints = obj;
      VssRestApiVersion? deprecatedAtVersion = new VssRestApiVersion?();
      routes1.MapResourceRoute(TfsApiResourceScope.Collection, layoutSystemControls, "processes", "SystemControls", "work/{area}/{processId}/workItemTypes/{witRefName}/layout/systemcontrols/{controlId}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, defaults: (object) defaults, constraints: constraints, routeName: "processesSystemControls", deprecatedAtVersion: deprecatedAtVersion);
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessesFormLayoutControls, "processes", "Controls", "work/{area}/{processId}/workItemTypes/{witRefName}/layout/groups/{groupId}/{resource}/{controlId}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released, defaults: (object) new
      {
        controlId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processesControls");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessFormLayoutPages, "processes", "Pages", "work/{area}/{processId}/workItemTypes/{witRefName}/layout/{resource}/{pageId}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released, defaults: (object) new
      {
        pageId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processesPages");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessFormLayoutGroups, "processes", "Groups", "work/{area}/{processId}/workItemTypes/{witRefName}/layout/pages/{pageId}/sections/{sectionId}/{resource}/{groupId}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released, defaults: (object) new
      {
        groupId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processesGroups");
    }
  }
}
