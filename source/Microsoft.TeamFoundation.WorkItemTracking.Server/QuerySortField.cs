// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QuerySortField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QuerySortField : IEquatable<QuerySortField>
  {
    public QueryTableAlias TableAlias { get; set; }

    public FieldEntry Field { get; set; }

    public bool Descending { get; set; }

    public bool? NullsFirst { get; set; }

    public bool Equals(QuerySortField other) => other != null && this.TableAlias == other.TableAlias && this.Field.FieldId == other.Field.FieldId;

    public override bool Equals(object obj) => this.Equals((QuerySortField) obj);

    public override int GetHashCode() => this.Field.FieldId | (int) this.TableAlias << 24;
  }
}
