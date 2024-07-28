// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.KnownBoundsFilter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class KnownBoundsFilter : IFilter<INonUserColumn>
  {
    protected KnownBoundsFilter(IFilter<INonUserColumn> filter, string min, string max)
    {
      this.Filter = filter;
      if (min == string.Empty)
        min = (string) null;
      if (max == string.Empty)
        max = (string) null;
      this.Min = min == null || max == null || string.Compare(min, max, StringComparison.Ordinal) <= 0 ? min : throw new ArgumentException("KnownBoundsFilter is created with a max value (" + max + ") less than the specified min value (" + min + ").");
      this.Max = max;
    }

    public bool IsNull => this.Filter.IsNull;

    public string Max { get; private set; }

    public string Min { get; private set; }

    protected IFilter<INonUserColumn> Filter { get; set; }

    public StringBuilder CreateFilter(StringBuilder builder) => this.Filter.CreateFilter(builder);

    public bool IsMatch(ITableEntityWithColumns entity) => this.Filter.IsMatch(entity);
  }
}
