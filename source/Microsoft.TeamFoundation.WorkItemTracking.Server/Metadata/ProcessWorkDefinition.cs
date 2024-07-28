// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessWorkDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessWorkDefinition
  {
    internal ProcessWorkDefinition()
    {
    }

    public ProcessWorkDefinition(
      IReadOnlyCollection<ProcessWorkItemTypeDefinition> workItemTypeDefinitions,
      ProcessBacklogs processBacklogs,
      IReadOnlyDictionary<string, CategoryWorkItemTypes> workItemTypeCategories,
      IReadOnlyDictionary<string, OobLinkType> linkTypes)
    {
      this.WorkItemTypeDefinitions = workItemTypeDefinitions;
      this.ProcessBacklogs = processBacklogs;
      this.WorkItemTypeCategories = workItemTypeCategories;
      this.LinkTypes = linkTypes;
    }

    public IReadOnlyCollection<ProcessWorkItemTypeDefinition> WorkItemTypeDefinitions { get; internal set; }

    public IReadOnlyCollection<ProcessFieldDefinition> ProcessFields => this.WorkItemTypeDefinitions != null ? (IReadOnlyCollection<ProcessFieldDefinition>) this.WorkItemTypeDefinitions.SelectMany<ProcessWorkItemTypeDefinition, ProcessFieldDefinition>((Func<ProcessWorkItemTypeDefinition, IEnumerable<ProcessFieldDefinition>>) (wit => (IEnumerable<ProcessFieldDefinition>) wit.FieldDefinitions)).Distinct<ProcessFieldDefinition>().ToList<ProcessFieldDefinition>() : (IReadOnlyCollection<ProcessFieldDefinition>) new List<ProcessFieldDefinition>();

    public ProcessBacklogs ProcessBacklogs { get; internal set; }

    public IReadOnlyDictionary<string, CategoryWorkItemTypes> WorkItemTypeCategories { get; internal set; }

    public IReadOnlyDictionary<string, OobLinkType> LinkTypes { get; internal set; }
  }
}
