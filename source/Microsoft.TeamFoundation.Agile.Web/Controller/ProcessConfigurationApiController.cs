// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.ProcessConfigurationApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Contracts;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "processconfiguration")]
  public class ProcessConfigurationApiController : TfsProjectApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (ProcessConfiguration), null, null)]
    [ClientExample("GET__work_processconfiguration.json", "Get process configuration", null, null)]
    public HttpResponseMessage GetProcessConfiguration()
    {
      ProcessConfiguration processConfiguration = this.Convert(this.GetProcessSettings());
      processConfiguration.Url = this.GetSelfUrl();
      return this.Request.CreateResponse<ProcessConfiguration>(HttpStatusCode.OK, processConfiguration);
    }

    private ProcessConfiguration Convert(
      ProjectProcessConfiguration projectProcessConfiguration)
    {
      ProcessConfiguration processConfiguration1 = new ProcessConfiguration();
      processConfiguration1.TypeFields = this.GetTypeFields(projectProcessConfiguration);
      processConfiguration1.TaskBacklog = this.Convert((Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CategoryConfiguration) projectProcessConfiguration.TaskBacklog);
      processConfiguration1.RequirementBacklog = this.Convert((Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CategoryConfiguration) projectProcessConfiguration.RequirementBacklog);
      ProcessConfiguration processConfiguration2 = processConfiguration1;
      BacklogCategoryConfiguration[] portfolioBacklogs = projectProcessConfiguration.PortfolioBacklogs;
      Microsoft.TeamFoundation.Work.WebApi.Contracts.CategoryConfiguration[] array = portfolioBacklogs != null ? ((IEnumerable<BacklogCategoryConfiguration>) portfolioBacklogs).Select<BacklogCategoryConfiguration, Microsoft.TeamFoundation.Work.WebApi.Contracts.CategoryConfiguration>((Func<BacklogCategoryConfiguration, Microsoft.TeamFoundation.Work.WebApi.Contracts.CategoryConfiguration>) (pb => this.Convert((Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CategoryConfiguration) pb))).ToArray<Microsoft.TeamFoundation.Work.WebApi.Contracts.CategoryConfiguration>() : (Microsoft.TeamFoundation.Work.WebApi.Contracts.CategoryConfiguration[]) null;
      processConfiguration2.PortfolioBacklogs = array;
      if (projectProcessConfiguration.BugWorkItems != null)
        processConfiguration1.BugWorkItems = this.Convert(projectProcessConfiguration.BugWorkItems);
      return processConfiguration1;
    }

    private Microsoft.TeamFoundation.Work.WebApi.Contracts.CategoryConfiguration Convert(
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CategoryConfiguration categoryConfiguration)
    {
      IEnumerable<IWorkItemType> typesForCategory = this.GetWorkItemTypesForCategory(categoryConfiguration.CategoryReferenceName);
      return new Microsoft.TeamFoundation.Work.WebApi.Contracts.CategoryConfiguration()
      {
        Name = categoryConfiguration.PluralName,
        ReferenceName = categoryConfiguration.CategoryReferenceName,
        WorkItemTypes = typesForCategory != null ? typesForCategory.Select<IWorkItemType, WorkItemTypeReference>((Func<IWorkItemType, WorkItemTypeReference>) (wit =>
        {
          return new WorkItemTypeReference()
          {
            Name = wit.Name,
            Url = this.GetWorkItemTypeUrl(wit.Name)
          };
        })).ToArray<WorkItemTypeReference>() : (WorkItemTypeReference[]) null
      };
    }

    private IDictionary<string, WorkItemFieldReference> GetTypeFields(
      ProjectProcessConfiguration projectProcessConfiguration)
    {
      return (IDictionary<string, WorkItemFieldReference>) this.Convert(new List<TypeField>()
      {
        projectProcessConfiguration.EffortField,
        projectProcessConfiguration.OrderByField,
        projectProcessConfiguration.RemainingWorkField,
        projectProcessConfiguration.TeamField,
        projectProcessConfiguration.ActivityField,
        projectProcessConfiguration.ClosedDateField
      });
    }

    private WorkItemFieldReference Create(
      string fieldReferenceName,
      WorkItemTrackingFieldService fieldService)
    {
      FieldEntry field = (FieldEntry) null;
      if (!fieldService.TryGetField(this.TfsRequestContext, fieldReferenceName, out field))
        return (WorkItemFieldReference) null;
      return new WorkItemFieldReference()
      {
        Name = field.Name,
        ReferenceName = field.ReferenceName,
        Url = this.GetFieldUrl(field.ReferenceName)
      };
    }

    protected virtual string GetWorkItemTypeUrl(string workItemTypeName) => WitUrlHelper.GetWorkItemTypeUrl(this.TfsRequestContext, this.ProjectId, workItemTypeName);

    protected virtual string GetFieldUrl(string fieldReferenceName) => WitUrlHelper.GetFieldUrl(this.TfsRequestContext, fieldReferenceName);

    protected virtual ProjectProcessConfiguration GetProcessSettings() => this.TfsRequestContext.GetService<IProjectConfigurationService>().GetProcessSettings(this.TfsRequestContext, this.ProjectInfo.Uri, true);

    protected virtual IEnumerable<IWorkItemType> GetWorkItemTypesForCategory(
      string categoryReferenceName)
    {
      WebAccessWorkItemService service = this.TfsRequestContext.GetService<WebAccessWorkItemService>();
      return service.GetWorkItemTypes(this.TfsRequestContext, this.ProjectId).SelectByName(service.GetWorkItemNamesForCategories(this.TfsRequestContext, this.ProjectInfo.Name, (IEnumerable<string>) new string[1]
      {
        categoryReferenceName
      }));
    }

    protected virtual string GetSelfUrl()
    {
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      routeValues["project"] = (object) this.ProjectId;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid empty = Guid.Empty;
      return service.GetLocationData(tfsRequestContext, empty).GetResourceUri(this.TfsRequestContext, "work", WorkWebConstants.ProcessConfigurationLocationId, (object) routeValues, true).ToString();
    }

    protected virtual Dictionary<string, WorkItemFieldReference> Convert(List<TypeField> typeFields)
    {
      WorkItemTrackingFieldService fieldDictionary = this.TfsRequestContext.GetService<WebAccessWorkItemService>().GetFieldDictionary(this.TfsRequestContext);
      return typeFields.Where<TypeField>((Func<TypeField, bool>) (tf => tf != null)).ToDictionary<TypeField, string, WorkItemFieldReference>((Func<TypeField, string>) (tf => tf.Type.ToString()), (Func<TypeField, WorkItemFieldReference>) (tf => this.Create(tf.Name, fieldDictionary)));
    }
  }
}
