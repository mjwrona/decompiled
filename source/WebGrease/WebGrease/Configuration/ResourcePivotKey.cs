// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.ResourcePivotKey
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  public class ResourcePivotKey
  {
    public ResourcePivotKey(string groupKey, string key)
    {
      this.GroupKey = groupKey;
      this.Key = key;
    }

    public string GroupKey { get; private set; }

    public string Key { get; private set; }

    public override string ToString() => "[{0}:{1}]".InvariantFormat((object) this.GroupKey, (object) this.Key);

    internal string ToString(string format) => format.InvariantFormat((object) this.GroupKey, (object) this.Key);
  }
}
