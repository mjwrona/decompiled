// Decompiled with JetBrains decompiler
// Type: Nest.BulkResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Nest
{
  [DataContract]
  public class BulkResponse : ResponseBase
  {
    [DataMember(Name = "errors")]
    public bool Errors { get; internal set; }

    public override bool IsValid => base.IsValid && !this.Errors && !this.ItemsWithErrors.HasAny<BulkResponseItemBase>();

    [DataMember(Name = "items")]
    public IReadOnlyList<BulkResponseItemBase> Items { get; internal set; } = EmptyReadOnly<BulkResponseItemBase>.List;

    [IgnoreDataMember]
    public IEnumerable<BulkResponseItemBase> ItemsWithErrors => this.Items.HasAny<BulkResponseItemBase>() ? this.Items.Where<BulkResponseItemBase>((Func<BulkResponseItemBase, bool>) (i => !i.IsValid)) : Enumerable.Empty<BulkResponseItemBase>();

    [DataMember(Name = "took")]
    public long Took { get; internal set; }

    protected override void DebugIsValid(StringBuilder sb)
    {
      if (this.Items == null)
        return;
      sb.AppendLine("# Invalid Bulk items:");
      foreach (var data in this.Items.Select((item, i) => new
      {
        item = item,
        i = i
      }).Where(i => !i.item.IsValid))
        sb.AppendLine(string.Format("  operation[{0}]: {1}", (object) data.i, (object) data.item));
    }
  }
}
