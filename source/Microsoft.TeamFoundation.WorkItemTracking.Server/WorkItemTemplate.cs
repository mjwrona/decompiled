// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTemplate : WorkItemTemplateDescriptor
  {
    public IDictionary<string, string> Fields { get; }

    public WorkItemTemplate(
      Guid id,
      string name,
      IDictionary<string, string> fields,
      string description,
      string workItemTypeName,
      Guid ownerId,
      Guid projectId)
      : base(id, name, description, workItemTypeName, ownerId, projectId)
    {
      if (fields == null)
        throw new InvalidWorkItemTemplateFieldsException(ServerResources.WorkItemTemplateFieldsNotSpecified());
      this.Fields = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      foreach (KeyValuePair<string, string> field in (IEnumerable<KeyValuePair<string, string>>) fields)
      {
        string key = field.Key.Trim();
        if (this.Fields.ContainsKey(key))
          throw new InvalidWorkItemTemplateFieldRefNameException(ServerResources.WorkItemTemplateDuplicateFieldRefName((object) key));
        this.Fields[key] = field.Value;
      }
    }

    public WorkItemTemplate CloneWithId(Guid templateId) => new WorkItemTemplate(templateId, this.Name, this.Fields, this.Description, this.WorkItemTypeName, this.OwnerId, this.ProjectId);

    public WorkItemTemplate Clone() => new WorkItemTemplate(this.Id, this.Name, this.Fields, this.Description, this.WorkItemTypeName, this.OwnerId, this.ProjectId);

    public void SetWorkItemTypeName(string workItemTypeName) => this.WorkItemTypeName = workItemTypeName;
  }
}
