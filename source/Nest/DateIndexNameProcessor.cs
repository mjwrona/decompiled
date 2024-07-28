// Decompiled with JetBrains decompiler
// Type: Nest.DateIndexNameProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class DateIndexNameProcessor : ProcessorBase, IDateIndexNameProcessor, IProcessor
  {
    public IEnumerable<string> DateFormats { get; set; }

    public Nest.DateRounding? DateRounding { get; set; }

    public Field Field { get; set; }

    public string IndexNameFormat { get; set; }

    public string IndexNamePrefix { get; set; }

    public string Locale { get; set; }

    public string TimeZone { get; set; }

    protected override string Name => "date_index_name";
  }
}
