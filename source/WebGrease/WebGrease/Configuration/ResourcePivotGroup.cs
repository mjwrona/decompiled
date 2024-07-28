// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.ResourcePivotGroup
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;

namespace WebGrease.Configuration
{
  public class ResourcePivotGroup
  {
    public ResourcePivotGroup(
      string key,
      ResourcePivotApplyMode applyMode,
      IEnumerable<string> keys)
    {
      this.Key = key;
      this.ApplyMode = applyMode;
      this.Keys = new HashSet<string>(keys);
    }

    public ResourcePivotApplyMode ApplyMode { get; private set; }

    public string Key { get; private set; }

    public HashSet<string> Keys { get; private set; }
  }
}
