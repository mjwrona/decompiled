// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemType
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class WorkItemType : IWorkItemType
  {
    private HashSet<int> m_fieldIds;
    private AdditionalWorkItemTypeProperties m_extendedProperties;
    private FieldDefinitionCollection m_fields;

    internal WorkItemType(
      int? id,
      int projectId,
      Guid projectGuid,
      string name,
      string referenceName,
      string description,
      string color)
    {
      this.Id = id;
      this.ProjectId = projectId;
      this.ProjectGuid = projectGuid;
      this.Name = name;
      this.ReferenceName = referenceName;
      this.Description = description;
      this.Color = color;
    }

    public int? Id { get; private set; }

    public int ProjectId { get; private set; }

    public Guid ProjectGuid { get; private set; }

    public string Name { get; private set; }

    public string ReferenceName { get; private set; }

    public string Description { get; private set; }

    public string Color { get; private set; }

    public IReadOnlyCollection<int> Fields => (IReadOnlyCollection<int>) this.InternalFields;

    internal HashSet<int> InternalFields
    {
      get
      {
        if (this.m_fieldIds == null)
          this.m_fieldIds = new HashSet<int>();
        return this.m_fieldIds;
      }
    }

    public AdditionalWorkItemTypeProperties GetExtendedProperties(IVssRequestContext requestContext)
    {
      if (this.m_extendedProperties == null)
      {
        WorkItemTypeService service = requestContext.GetService<WorkItemTypeService>();
        requestContext.WitContext();
        this.m_extendedProperties = service.GetWorkItemTypeByReferenceName(requestContext, this.ProjectGuid, this.Name).GetAdditionalProperties(requestContext);
      }
      return this.m_extendedProperties;
    }

    public List<string> GetOrderedStates(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(910500, "WorkItemStateDefinition", "WorkItemStateDefinitionService", nameof (GetOrderedStates));
      try
      {
        IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
        ProcessDescriptor processDescriptor;
        if (!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) || !service.TryGetProjectProcessDescriptor(requestContext, this.ProjectGuid, out processDescriptor) || processDescriptor.IsCustom)
          return (List<string>) null;
        List<WorkItemStateDefinition> list = requestContext.GetService<IWorkItemStateDefinitionService>().GetCombinedStateDefinitions(requestContext, processDescriptor.TypeId, this.ReferenceName).ToList<WorkItemStateDefinition>();
        list.Sort();
        return list.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).ToList<string>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(910500, "Services", "WorkItemStateDefinitionService", ex);
        return (List<string>) null;
      }
      finally
      {
        requestContext.TraceLeave(910500, "WorkItemStateDefinition", "WorkItemStateDefinitionService", nameof (GetOrderedStates));
      }
    }

    public FieldDefinitionCollection GetFields(IVssRequestContext requestContext)
    {
      if (this.m_fields == null)
      {
        IEnumerable<int> fieldIdsCopy = Enumerable.Empty<int>();
        if (requestContext.IsTracing(919751, TraceLevel.Warning, nameof (WorkItemType), "MissingFieldIdsInGetFields"))
          fieldIdsCopy = (IEnumerable<int>) new HashSet<int>((IEnumerable<int>) this.InternalFields);
        this.m_fields = new FieldDefinitionCollection(this.InternalFields.Join<int, FieldEntry, int, FieldDefinition>(requestContext.GetService<WorkItemTrackingFieldService>().GetAllFields(requestContext), (Func<int, int>) (workItemFieldId => workItemFieldId), (Func<FieldEntry, int>) (existingField => existingField.FieldId), (Func<int, FieldEntry, FieldDefinition>) ((workItemFieldId, existingField) => new FieldDefinition(existingField))));
        this.m_fieldIds = new HashSet<int>(this.m_fields.Select<FieldDefinition, int>((Func<FieldDefinition, int>) (field => field.Id)));
        requestContext.TraceConditionally(919751, TraceLevel.Warning, nameof (WorkItemType), "MissingFieldIdsInGetFields", (Func<string>) (() =>
        {
          IEnumerable<int> source = fieldIdsCopy.Except<int>((IEnumerable<int>) this.m_fieldIds);
          return source.Any<int>() ? new
          {
            Name = this.Name,
            ReferenceName = this.ReferenceName,
            ProjectGuid = this.ProjectGuid,
            FieldIds = source.ToArray<int>()
          }.Serialize() : string.Empty;
        }));
      }
      return this.m_fields;
    }
  }
}
