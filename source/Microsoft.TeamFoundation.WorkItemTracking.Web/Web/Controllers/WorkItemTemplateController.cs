// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTemplateController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "templates", ResourceVersion = 1)]
  [ResolveTfsProjectAndTeamFilter(RequireExplicitTeam = true)]
  public class WorkItemTemplateController : TfsTeamApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InvalidWorkItemTemplateNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidWorkItemTypeNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidWorkItemTemplateDescriptionException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidWorkItemTemplateFieldsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidWorkItemTemplateFieldRefNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidWorkItemTemplateFieldValueException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidWorkItemTemplateIdException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTemplateNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTemplateLimitPerTypeExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Devops.Teams.Service.TeamSecurityException>(HttpStatusCode.Forbidden);
      exceptionMap.AddTranslation<InvalidWorkItemTemplateNameException, InvalidWorkItemTemplateNameException>();
      exceptionMap.AddTranslation<InvalidWorkItemTypeNameException, InvalidWorkItemTypeNameException>();
      exceptionMap.AddTranslation<InvalidWorkItemTemplateDescriptionException, InvalidWorkItemTemplateDescriptionException>();
      exceptionMap.AddTranslation<InvalidWorkItemTemplateFieldsException, InvalidWorkItemTemplateFieldsException>();
      exceptionMap.AddTranslation<InvalidWorkItemTemplateFieldRefNameException, InvalidWorkItemTemplateFieldRefNameException>();
      exceptionMap.AddTranslation<InvalidWorkItemTemplateFieldValueException, InvalidWorkItemTemplateFieldValueException>();
      exceptionMap.AddTranslation<InvalidWorkItemTemplateIdException, InvalidWorkItemTemplateIdException>();
      exceptionMap.AddTranslation<WorkItemTemplateNotFoundException, WorkItemTemplateNotFoundException>();
      exceptionMap.AddTranslation<Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException, Microsoft.TeamFoundation.Core.WebApi.TeamNotFoundException>();
      exceptionMap.AddTranslation<Microsoft.Azure.Devops.Teams.Service.TeamSecurityException, Microsoft.TeamFoundation.Core.WebApi.TeamSecurityException>();
      exceptionMap.AddTranslation<WorkItemTemplateLimitPerTypeExceededException, WorkItemTemplateLimitExceededException>();
    }

    [HttpPost]
    [ClientExample("Create_template.json", "Creates a template", null, null)]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate), null, null)]
    [ClientLocationId("6A90345F-A676-4969-AFCE-8E163E1D5642")]
    public HttpResponseMessage CreateTemplate([FromBody] Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate template)
    {
      this.ThrowIfNull((object) this.ProjectId, "ProjectId");
      this.ThrowIfNull((object) template, nameof (template));
      this.ValidateTeam();
      IWorkItemTemplateService service = this.TfsRequestContext.GetService<IWorkItemTemplateService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplate dataModel = this.TransformToDataModel(template);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplate template1 = dataModel;
      return this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate>(HttpStatusCode.Created, this.TransformToWebApi(service.CreateTemplate(tfsRequestContext, template1), new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
    }

    [HttpGet]
    [ClientExample("GET_templates.json", "Gets templates", null, null)]
    [ClientResponseType(typeof (IList<WorkItemTemplateReference>), null, null)]
    [ClientLocationId("6A90345F-A676-4969-AFCE-8E163E1D5642")]
    public HttpResponseMessage GetTemplates([FromUri(Name = "workitemtypename")] string workItemTypeName = null)
    {
      this.ThrowIfNull((object) this.ProjectId, "ProjectId");
      this.ValidateTeam();
      IWorkItemTemplateService service = this.TfsRequestContext.GetService<IWorkItemTemplateService>();
      return this.Request.CreateResponse<IEnumerable<WorkItemTemplateReference>>(HttpStatusCode.OK, this.TransformToWebApiReference(!string.IsNullOrEmpty(workItemTypeName) ? service.GetTemplates(this.TfsRequestContext, this.ProjectId, this.TeamId, workItemTypeName) : service.GetTemplates(this.TfsRequestContext, this.ProjectId, this.TeamId), new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
    }

    [HttpGet]
    [ClientExample("GET_template.json", "Gets the template with specified id", null, null)]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate), null, null)]
    [ClientLocationId("FB10264A-8836-48A0-8033-1B0CCD2748D5")]
    public HttpResponseMessage GetTemplate(Guid templateId)
    {
      this.ThrowIfNull((object) this.ProjectId, "ProjectId");
      this.ThrowIfNull((object) templateId, nameof (templateId));
      this.ValidateTeam(true);
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplate template = this.TfsRequestContext.GetService<IWorkItemTemplateService>().GetTemplate(this.TfsRequestContext, this.ProjectId, templateId);
      if (template == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      Dictionary<string, string> workItemTypeUrlCache = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      return this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate>(HttpStatusCode.OK, this.TransformToWebApi(template, workItemTypeUrlCache));
    }

    [HttpPut]
    [ClientExample("PUT_template.json", "Replace template contents", null, null)]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate), null, null)]
    [ClientLocationId("FB10264A-8836-48A0-8033-1B0CCD2748D5")]
    public HttpResponseMessage ReplaceTemplate(Guid templateId, [FromBody] Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate templateContent)
    {
      this.ThrowIfNull((object) this.ProjectId, "ProjectId");
      this.ThrowIfNull((object) templateId, nameof (templateId));
      this.ThrowIfNull((object) templateContent, nameof (templateContent));
      this.ValidateTeam(true);
      IWorkItemTemplateService service = this.TfsRequestContext.GetService<IWorkItemTemplateService>();
      templateContent.Id = templateId;
      service.UpdateTemplate(this.TfsRequestContext, this.TransformToDataModel(templateContent));
      return this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate>(HttpStatusCode.OK, this.TransformToWebApi(service.GetTemplate(this.TfsRequestContext, this.ProjectId, templateId), new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("FB10264A-8836-48A0-8033-1B0CCD2748D5")]
    public HttpResponseMessage DeleteTemplate(Guid templateId)
    {
      this.ThrowIfNull((object) this.ProjectId, "ProjectId");
      this.ThrowIfNull((object) templateId, nameof (templateId));
      this.ValidateTeam();
      this.TfsRequestContext.GetService<IWorkItemTemplateService>().DeleteTemplate(this.TfsRequestContext, this.ProjectId, templateId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    private Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate TransformToWebApi(
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplate template,
      Dictionary<string, string> workItemTypeUrlCache)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("workItemType", this.GetWorkItemTypeUrl((WorkItemTemplateDescriptor) template, workItemTypeUrlCache));
      string templateUrl = this.GetTemplateUrl(new Guid?(template.Id));
      referenceLinks.AddLink("self", templateUrl);
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate webApi = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate();
      webApi.Id = template.Id;
      webApi.Description = template.Description;
      webApi.Fields = template.Fields;
      webApi.Name = template.Name;
      webApi.WorkItemTypeName = template.WorkItemTypeName;
      webApi.Url = this.GetTemplateUrl(new Guid?(template.Id));
      webApi.Links = referenceLinks;
      return webApi;
    }

    private string GetWorkItemTypeUrl(
      WorkItemTemplateDescriptor descriptor,
      Dictionary<string, string> workItemTypeUrlCache)
    {
      string workItemTypeUrl = string.Empty;
      if (!workItemTypeUrlCache.TryGetValue(descriptor.WorkItemTypeName, out workItemTypeUrl))
      {
        workItemTypeUrl = WitUrlHelper.GetWorkItemTypeUrl(this.TfsRequestContext.WitContext(), this.ProjectId, descriptor.WorkItemTypeName);
        workItemTypeUrlCache[descriptor.WorkItemTypeName] = workItemTypeUrl;
      }
      return workItemTypeUrl;
    }

    private IEnumerable<WorkItemTemplateReference> TransformToWebApiReference(
      IEnumerable<WorkItemTemplateDescriptor> descriptors,
      Dictionary<string, string> workItemTypeUrlCache)
    {
      return descriptors.Select<WorkItemTemplateDescriptor, WorkItemTemplateReference>((Func<WorkItemTemplateDescriptor, WorkItemTemplateReference>) (t => this.TransformToWebApiReference(t, workItemTypeUrlCache)));
    }

    private WorkItemTemplateReference TransformToWebApiReference(
      WorkItemTemplateDescriptor descriptor,
      Dictionary<string, string> workItemTypeUrlCache)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("workItemType", this.GetWorkItemTypeUrl(descriptor, workItemTypeUrlCache));
      string templateUrl = this.GetTemplateUrl(new Guid?(descriptor.Id));
      referenceLinks.AddLink("self", templateUrl);
      WorkItemTemplateReference webApiReference = new WorkItemTemplateReference();
      webApiReference.Id = descriptor.Id;
      webApiReference.Name = descriptor.Name;
      webApiReference.WorkItemTypeName = descriptor.WorkItemTypeName;
      webApiReference.Description = descriptor.Description;
      webApiReference.Url = templateUrl;
      webApiReference.Links = referenceLinks;
      return webApiReference;
    }

    private Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplate TransformToDataModel(
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTemplate template)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplate(template.Id, template.Name, template.Fields, template.Description, template.WorkItemTypeName, this.TeamId, this.ProjectId);
    }

    private void ThrowIfNull(object value, string paramName)
    {
      if (value == null)
        throw new VssPropertyValidationException(paramName, Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.Templates_ArgumentNull((object) paramName));
    }

    private void ValidateTeam(bool hasTemplateId = false)
    {
      if (this.TeamId == Guid.Empty)
        throw new VssResourceNotFoundException(hasTemplateId ? WorkItemTrackingLocationIds.TemplatesLocationWithTemplateIdGuid : WorkItemTrackingLocationIds.TemplatesLocationGuid);
    }

    private string GetTemplateUrl(Guid? templateId)
    {
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ILocationService service = tfsRequestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(tfsRequestContext, "wit", WorkItemTrackingLocationIds.TemplatesLocationWithTemplateIdGuid, templateId.HasValue ? (object) new
        {
          project = this.ProjectInfo.Name,
          team = this.TeamId,
          templateId = templateId.Value.ToString()
        } : (object) new
        {
          project = this.ProjectInfo.Name,
          team = this.TeamId,
          templateId = ""
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException("TODO: add to resourcestrings.resx", ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }
  }
}
