// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.MinifyCssPivot
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WebGrease.Configuration;

namespace WebGrease.Activities
{
  internal class MinifyCssPivot
  {
    private readonly string stringValue;

    public MinifyCssPivot(
      IEnumerable<IDictionary<string, string>> mergedResource,
      ResourcePivotKey[] newContentResourcePivotKeys,
      float dpi)
    {
      this.MergedResource = mergedResource;
      this.NewContentResourcePivotKeys = newContentResourcePivotKeys;
      this.Dpi = dpi;
      this.stringValue = string.Join("-", ((IEnumerable<ResourcePivotKey>) this.NewContentResourcePivotKeys).Select<ResourcePivotKey, string>((Func<ResourcePivotKey, string>) (p => p.Key)));
    }

    public IEnumerable<IDictionary<string, string>> MergedResource { get; private set; }

    public ResourcePivotKey[] NewContentResourcePivotKeys { get; private set; }

    public float Dpi { get; private set; }

    public override string ToString() => this.stringValue;

    public override int GetHashCode() => this.stringValue.GetHashCode();
  }
}
