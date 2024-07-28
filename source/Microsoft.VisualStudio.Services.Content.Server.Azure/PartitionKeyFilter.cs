// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.PartitionKeyFilter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class PartitionKeyFilter : KnownBoundsFilter
  {
    public PartitionKeyFilter(NullFilter<PartitionKeyColumn> filter)
      : base((IFilter<INonUserColumn>) filter, (string) null, (string) null)
    {
    }

    public PartitionKeyFilter(EqualFilter<PartitionKeyColumn> filter)
      : base((IFilter<INonUserColumn>) filter, ((StringValue) filter.ColumnValue.Value).Value, ((StringValue) filter.ColumnValue.Value).Value)
    {
    }

    public PartitionKeyFilter(RangeFilter<PartitionKeyColumn> filter)
      : base((IFilter<INonUserColumn>) filter, filter.Minimum == null ? (string) null : ((StringValue) filter.Minimum.BoundaryValue.Value).Value, filter.Maximum == null ? (string) null : ((StringValue) filter.Maximum.BoundaryValue.Value).Value)
    {
    }
  }
}
