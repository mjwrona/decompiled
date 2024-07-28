// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemTypeFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public class WorkItemTypeFactory
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType Create(
      WorkItemTrackingRequestContext witRequestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType,
      bool includeLinks,
      bool includeCalculatedFields,
      string color = null,
      string icon = null)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      AdditionalWorkItemTypeProperties workItemTypeDetails = workItemType.GetAdditionalProperties(requestContext);
      string workItemTypeUrl = WitUrlHelper.GetWorkItemTypeUrl(witRequestContext, workItemType);
      ReferenceLinks referenceLinks = (ReferenceLinks) null;
      if (includeLinks)
      {
        referenceLinks = new ReferenceLinks();
        referenceLinks.AddLink("self", workItemTypeUrl, (ISecuredObject) workItemType);
      }
      IDictionary<string, WorkItemStateTransition[]> dictionary = (IDictionary<string, WorkItemStateTransition[]>) null;
      if (workItemTypeDetails.Transitions != null && workItemTypeDetails.Transitions.Any<KeyValuePair<string, HashSet<string>>>())
        dictionary = (IDictionary<string, WorkItemStateTransition[]>) workItemTypeDetails.Transitions.ToDictionary<KeyValuePair<string, HashSet<string>>, string, WorkItemStateTransition[]>((Func<KeyValuePair<string, HashSet<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, HashSet<string>>, WorkItemStateTransition[]>) (kvp =>
        {
          IEnumerable<WorkItemStateTransition> source = kvp.Value.Select<string, WorkItemStateTransition>((Func<string, WorkItemStateTransition>) (to => WorkItemStateTransitionFactory.Create(workItemType, kvp.Key, to, workItemTypeDetails.Actions)));
          return source == null ? (WorkItemStateTransition[]) null : source.ToArray<WorkItemStateTransition>();
        }));
      string str = workItemTypeDetails.Form;
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
      {
        Layout formLayout = workItemTypeDetails.GetFormLayout(requestContext);
        if (formLayout != null)
          str = new FormTransformer().ConvertToNewFormXml(formLayout);
      }
      string color1 = color ?? workItemType.Color ?? CommonWITUtils.GetDefaultWorkItemTypeColor();
      string icon1 = icon ?? workItemType.Icon ?? WorkItemTypeIconUtils.GetDefaultIcon();
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor projectProcess = (ProcessDescriptor) null;
      if (workItemType.ProjectId != Guid.Empty)
        service.TryGetProjectProcessDescriptor(requestContext, workItemType.ProjectId, out projectProcess);
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor> workItemStateColors = requestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStateColors(requestContext, workItemType.ProjectId, workItemType.Name).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>) (c => new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor((ISecuredObject) c)
      {
        Name = c.Name,
        Color = c.Color,
        Category = c.Category
      }));
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType workItemType1 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType((ISecuredObject) workItemType);
      workItemType1.Name = workItemType.Name;
      workItemType1.ReferenceName = workItemType.ReferenceName;
      workItemType1.Color = color1;
      workItemType1.Icon = WorkItemIconFactory.Create(requestContext, workItemType, icon1, color1);
      workItemType1.IsDisabled = workItemType.IsDisabled;
      workItemType1.Description = workItemType.Description;
      workItemType1.XmlForm = str;
      workItemType1.Url = workItemTypeUrl;
      workItemType1.FieldInstances = workItemType.GetFieldIds(requestContext, includeCalculatedFields).Select<int, WorkItemTypeFieldInstance>((Func<int, WorkItemTypeFieldInstance>) (fid => WorkItemTypeFieldInstanceFactory.Create(witRequestContext, witRequestContext.FieldDictionary.GetField(fid), workItemType, workItemTypeDetails, projectProcess: projectProcess, projectId: new Guid?(workItemType.ProjectId))));
      workItemType1.Transitions = dictionary;
      workItemType1.Links = referenceLinks;
      workItemType1.States = workItemStateColors;
      return workItemType1;
    }
  }
}
