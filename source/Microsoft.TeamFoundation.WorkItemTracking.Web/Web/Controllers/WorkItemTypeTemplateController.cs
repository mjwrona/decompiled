// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeTemplateController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemTypeTemplate")]
  [ClientInternalUseOnly(true)]
  public class WorkItemTypeTemplateController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5909000;

    public override string TraceArea => "workItemTypeTemplate";

    [TraceFilter(5909000, 5909010)]
    [HttpPost]
    [ClientResponseType(typeof (ProvisioningResult), null, null)]
    public HttpResponseMessage UpdateWorkItemTypeDefinition(
      WorkItemTypeTemplateUpdateModel updateModel)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<WorkItemTypeTemplateUpdateModel>(updateModel, nameof (updateModel));
      ProvisioningImportEventsCallback importEventCallback = new ProvisioningImportEventsCallback();
      try
      {
        ProvisioningResult provisioningResult = new ProvisioningResult();
        IProvisioningService service = this.TfsRequestContext.GetService<IProvisioningService>();
        int projectId = this.ProjectId == Guid.Empty ? 0 : this.TfsRequestContext.WitContext().TreeService.GetTreeNode(this.ProjectId, this.ProjectId).Id;
        if (updateModel.TemplateType == TemplateType.WorkItemType)
        {
          if (updateModel.ActionType == Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ProvisioningActionType.Validate && projectId == 0)
            projectId = -1;
          service.ImportWorkItemType(this.TfsRequestContext, projectId, updateModel.Methodology, updateModel.Template, (Microsoft.TeamFoundation.WorkItemTracking.Server.ProvisioningActionType) updateModel.ActionType, importEventCallback: importEventCallback);
        }
        else if (updateModel.TemplateType == TemplateType.GlobalWorkflow)
          service.ImportGlobalWorkflow(this.TfsRequestContext, projectId, updateModel.Template, (Microsoft.TeamFoundation.WorkItemTracking.Server.ProvisioningActionType) updateModel.ActionType, importEventCallback: importEventCallback);
        provisioningResult.ProvisioningImportEvents = (IEnumerable<string>) importEventCallback.ProvisioningImportEvents;
        return this.Request.CreateResponse<ProvisioningResult>(HttpStatusCode.OK, provisioningResult);
      }
      catch (LegacyDeniedOrNotExist ex)
      {
        throw new LegacyDeniedOrNotExist((Exception) ex);
      }
      catch (LegacyServerException ex)
      {
        List<string> provisioningImportEvents = importEventCallback.ProvisioningImportEvents;
        throw new WorkItemTrackingTypeTemplateException((Exception) ex, (IEnumerable<string>) provisioningImportEvents);
      }
      catch (XmlSchemaValidationException ex)
      {
        List<string> provisioningImportEvents = importEventCallback.ProvisioningImportEvents;
        throw new WorkItemTrackingTypeTemplateException((Exception) ex, (IEnumerable<string>) provisioningImportEvents);
      }
      catch (XmlException ex)
      {
        List<string> provisioningImportEvents = importEventCallback.ProvisioningImportEvents;
        throw new WorkItemTrackingTypeTemplateException((Exception) ex, (IEnumerable<string>) provisioningImportEvents);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(900113, TraceLevel.Error, "Rest", nameof (WorkItemTypeTemplateController), ex);
        throw;
      }
    }

    [TraceFilter(5909011, 5909020)]
    [HttpGet]
    [ClientResponseType(typeof (WorkItemTypeTemplate), null, null)]
    public HttpResponseMessage ExportWorkItemTypeDefinition(string type = null, bool exportGlobalLists = false)
    {
      try
      {
        WorkItemTypeTemplate itemTypeTemplate = new WorkItemTypeTemplate();
        IProvisioningService service = this.TfsRequestContext.GetService<IProvisioningService>();
        XmlDocument xml;
        if (type == null)
        {
          xml = service.ExportGlobalWorkflow(this.TfsRequestContext, this.ProjectId, exportGlobalLists ? ExportMask.ExportGlobalLists : ExportMask.None);
        }
        else
        {
          if (this.ProjectId == Guid.Empty)
            throw new WorkItemTrackingTypeTemplateNotFoundException(ResourceStrings.WorkItemTrackingTypeTemplateNotFound());
          ProcessDescriptor processDescriptor;
          if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") && this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(this.TfsRequestContext, this.ProjectId, out processDescriptor) && (processDescriptor.IsSystem || processDescriptor.IsDerived))
            throw new FeatureDisabledException(string.Format("{0} Process Id: {1}, Project Id: {2}", (object) FrameworkResources.FeatureDisabledError(), (object) processDescriptor.RowId, (object) this.ProjectId));
          xml = service.ExportWorkItemType(this.TfsRequestContext, this.ProjectId, type, exportGlobalLists ? ExportMask.ExportGlobalLists : ExportMask.None);
        }
        itemTypeTemplate.Template = WorkItemTypeTemplateController.ConvertToString(xml);
        return this.Request.CreateResponse<WorkItemTypeTemplate>(HttpStatusCode.OK, itemTypeTemplate);
      }
      catch (LegacyServerException ex)
      {
        throw new WorkItemTrackingTypeTemplateException((Exception) ex);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(900112, TraceLevel.Error, "Rest", nameof (WorkItemTypeTemplateController), ex);
        throw;
      }
    }

    private static string ConvertToString(XmlDocument xml)
    {
      using (StringWriter stringWriter = new StringWriter())
      {
        StringWriter output = stringWriter;
        using (XmlWriter w = XmlWriter.Create((TextWriter) output, new XmlWriterSettings()
        {
          Indent = true
        }))
        {
          xml.WriteTo(w);
          w.Flush();
          return stringWriter.GetStringBuilder().ToString();
        }
      }
    }
  }
}
