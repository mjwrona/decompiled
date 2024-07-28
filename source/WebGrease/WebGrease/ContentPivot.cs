// Decompiled with JetBrains decompiler
// Type: WebGrease.ContentPivot
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease
{
  public class ContentPivot
  {
    public ContentPivot(params ResourcePivotKey[] pivotKeys) => this.PivotKeys = (IEnumerable<ResourcePivotKey>) pivotKeys;

    public IEnumerable<ResourcePivotKey> PivotKeys { get; private set; }

    public string this[string groupKey] => this.PivotKeys.Where<ResourcePivotKey>((Func<ResourcePivotKey, bool>) (pk => pk.GroupKey.Equals(groupKey))).Select<ResourcePivotKey, string>((Func<ResourcePivotKey, string>) (pk => pk.Key)).FirstOrDefault<string>();

    public override string ToString() => "{0}".InvariantFormat((object) string.Join("-", this.PivotKeys.Select<ResourcePivotKey, string>((Func<ResourcePivotKey, string>) (p => p.Key)).Where<string>((Func<string, bool>) (i => !i.IsNullOrWhitespace()))));
  }
}
