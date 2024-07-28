// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProjectWorkItemTypes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ProjectWorkItemTypes
  {
    private Lazy<Dictionary<string, WorkItemType>> m_lazyTypesByName;
    private Lazy<Dictionary<string, WorkItemType>> m_lazyTypesByRefName;
    private Lazy<Dictionary<int, WorkItemType>> m_lazyTypesByLegacyId;

    internal ProjectWorkItemTypes(
      IEnumerable<WorkItemTypeEntry> workItemTypeEntries,
      Guid projectId,
      MetadataDBStamps stamps)
      : this(workItemTypeEntries != null ? workItemTypeEntries.Select<WorkItemTypeEntry, WorkItemType>((Func<WorkItemTypeEntry, WorkItemType>) (entry => new WorkItemType(entry, projectId))) : (IEnumerable<WorkItemType>) null, projectId, stamps)
    {
    }

    private ProjectWorkItemTypes(ProjectWorkItemTypes source)
    {
      IReadOnlyCollection<WorkItemType> workItemTypes = source.WorkItemTypes;
      // ISSUE: explicit constructor call
      this.\u002Ector(workItemTypes != null ? workItemTypes.Select<WorkItemType, WorkItemType>((Func<WorkItemType, WorkItemType>) (t => t.Clone())) : (IEnumerable<WorkItemType>) null, source.ProjectId, source.DbStamps);
    }

    internal ProjectWorkItemTypes(
      ProjectWorkItemTypes source,
      IReadOnlyCollection<WorkItemType> newWorkItemTypes)
    {
      IReadOnlyCollection<WorkItemType> workItemTypes = source.WorkItemTypes;
      // ISSUE: explicit constructor call
      this.\u002Ector(workItemTypes != null ? workItemTypes.Select<WorkItemType, WorkItemType>((Func<WorkItemType, WorkItemType>) (t => t.Clone())).Concat<WorkItemType>((IEnumerable<WorkItemType>) newWorkItemTypes) : (IEnumerable<WorkItemType>) null, source.ProjectId, source.DbStamps);
    }

    public ProjectWorkItemTypes(
      IEnumerable<WorkItemType> workItemTypes,
      Guid projectId,
      MetadataDBStamps stamps)
    {
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemType>>(workItemTypes, nameof (workItemTypes));
      this.WorkItemTypes = (IReadOnlyCollection<WorkItemType>) workItemTypes.ToArray<WorkItemType>();
      this.ProjectId = projectId;
      this.DbStamps = stamps;
      this.m_lazyTypesByName = new Lazy<Dictionary<string, WorkItemType>>((Func<Dictionary<string, WorkItemType>>) (() => this.WorkItemTypes.Where<WorkItemType>((Func<WorkItemType, bool>) (wit => !wit.Id.HasValue || wit.Id.Value > 0)).ToDictionary<WorkItemType, string>((Func<WorkItemType, string>) (wit => wit.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
      this.m_lazyTypesByRefName = new Lazy<Dictionary<string, WorkItemType>>((Func<Dictionary<string, WorkItemType>>) (() => this.WorkItemTypes.Where<WorkItemType>((Func<WorkItemType, bool>) (wit => !wit.Id.HasValue || wit.Id.Value > 0)).ToDictionary<WorkItemType, string>((Func<WorkItemType, string>) (wit => wit.ReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName)));
      this.m_lazyTypesByLegacyId = new Lazy<Dictionary<int, WorkItemType>>((Func<Dictionary<int, WorkItemType>>) (() => this.WorkItemTypes.Where<WorkItemType>((Func<WorkItemType, bool>) (wit => wit.Id.HasValue)).GroupBy<WorkItemType, int>((Func<WorkItemType, int>) (wit => wit.Id.Value)).ToDictionary<IGrouping<int, WorkItemType>, int, WorkItemType>((Func<IGrouping<int, WorkItemType>, int>) (g => g.Key), (Func<IGrouping<int, WorkItemType>, WorkItemType>) (g => g.First<WorkItemType>()))));
    }

    public MetadataDBStamps DbStamps { get; private set; }

    public Guid ProjectId { get; private set; }

    public IReadOnlyCollection<WorkItemType> WorkItemTypes { get; private set; }

    public IReadOnlyDictionary<string, WorkItemType> WorkItemTypesByName => (IReadOnlyDictionary<string, WorkItemType>) this.m_lazyTypesByName.Value;

    public IReadOnlyDictionary<string, WorkItemType> WorkItemTypesByReferenceName => (IReadOnlyDictionary<string, WorkItemType>) this.m_lazyTypesByRefName.Value;

    public IReadOnlyDictionary<int, WorkItemType> WorkItemTypesById => (IReadOnlyDictionary<int, WorkItemType>) this.m_lazyTypesByLegacyId.Value;

    private void AssociateTypelets(IEnumerable<ProcessWorkItemType> derivedTypes)
    {
      IReadOnlyDictionary<string, WorkItemType> typesByReferenceName = this.WorkItemTypesByReferenceName;
      foreach (ProcessWorkItemType derivedType in derivedTypes)
      {
        WorkItemType workItemType;
        if (typesByReferenceName.TryGetValue(derivedType.ParentTypeRefName, out workItemType))
          workItemType.InheritedWorkItemType = derivedType;
      }
    }

    public ProjectWorkItemTypes Clone(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<ProcessWorkItemType> typelets,
      Dictionary<string, int> customTypeProvisionedIdsByName)
    {
      ProjectWorkItemTypes projectWorkItemTypes;
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(witRequestContext.RequestContext))
      {
        IEnumerable<int> source1 = this.WorkItemTypes.Where<WorkItemType>((Func<WorkItemType, bool>) (t => t.Id.HasValue)).Select<WorkItemType, int>((Func<WorkItemType, int>) (t => t.Id.Value)).Where<int>((Func<int, bool>) (i => i > 0));
        IEnumerable<ProcessWorkItemType> source2 = typelets.Where<ProcessWorkItemType>((Func<ProcessWorkItemType, bool>) (t => !t.IsDerived));
        int num;
        List<WorkItemType> newWorkItemTypes = !source1.Any<int>() ? source2.Select<ProcessWorkItemType, WorkItemType>((Func<ProcessWorkItemType, WorkItemType>) (t => WorkItemType.Create(witRequestContext, t, this.ProjectId))).ToList<WorkItemType>() : (customTypeProvisionedIdsByName == null ? source2.Select<ProcessWorkItemType, WorkItemType>((Func<ProcessWorkItemType, WorkItemType>) (t => WorkItemType.Create(witRequestContext, t, this.ProjectId))).ToList<WorkItemType>() : source2.Select<ProcessWorkItemType, WorkItemType>((Func<ProcessWorkItemType, WorkItemType>) (t => customTypeProvisionedIdsByName.TryGetValue(t.Name, out num) ? WorkItemType.Create(witRequestContext, t, this.ProjectId, new int?(num)) : WorkItemType.Create(witRequestContext, t, this.ProjectId))).ToList<WorkItemType>());
        List<ProcessWorkItemType> list = typelets.Where<ProcessWorkItemType>((Func<ProcessWorkItemType, bool>) (t => t.IsDerived)).ToList<ProcessWorkItemType>();
        projectWorkItemTypes = new ProjectWorkItemTypes(this, (IReadOnlyCollection<WorkItemType>) newWorkItemTypes);
        projectWorkItemTypes.AssociateTypelets((IEnumerable<ProcessWorkItemType>) list);
      }
      else
      {
        projectWorkItemTypes = new ProjectWorkItemTypes(this);
        projectWorkItemTypes.AssociateTypelets(typelets);
      }
      return projectWorkItemTypes;
    }
  }
}
