// Decompiled with JetBrains decompiler
// Type: Nest.GeoIpProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class GeoIpProcessor : ProcessorBase, IGeoIpProcessor, IProcessor
  {
    public string DatabaseFile { get; set; }

    public Field Field { get; set; }

    public bool? IgnoreMissing { get; set; }

    public IEnumerable<string> Properties { get; set; }

    public Field TargetField { get; set; }

    public bool? FirstOnly { get; set; }

    protected override string Name => "geoip";
  }
}
