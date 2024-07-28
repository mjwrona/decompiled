// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.Query
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model
{
  [DataContract]
  public class Query : QueryReference
  {
    private string wiql;

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ParentId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember]
    public NodeType? Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Count { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<Query> Value { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TeamProjectReference Project { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<string> Columns { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public QueryType QueryType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<SortField> SortOptions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string WebUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Wiql
    {
      get => this.wiql;
      set
      {
        this.wiql = value;
        if (string.IsNullOrWhiteSpace(this.wiql))
          return;
        this.Type = new NodeType?(NodeType.Query);
      }
    }
  }
}
