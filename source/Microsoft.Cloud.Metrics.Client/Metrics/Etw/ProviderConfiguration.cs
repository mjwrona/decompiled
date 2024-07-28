// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.ProviderConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal sealed class ProviderConfiguration
  {
    public ProviderConfiguration(Guid id, EtwTraceLevel level, long keywordsAny, long keywordsAll)
    {
      this.Id = id;
      this.Level = level;
      this.KeywordsAny = keywordsAny;
      this.KeywordsAll = keywordsAll;
    }

    private ProviderConfiguration()
    {
    }

    public Guid Id { get; private set; }

    public EtwTraceLevel Level { get; private set; }

    public long KeywordsAny { get; private set; }

    public long KeywordsAll { get; private set; }

    public override string ToString() => string.Format("Id: {0} Level: {1} KeywordsAny: 0x{2} KeywordsAll: 0x{3}", (object) this.Id.ToString("B"), (object) this.Level.ToString(), (object) this.KeywordsAny.ToString("X16"), (object) this.KeywordsAll.ToString("X16"));

    public ProviderConfiguration Clone() => new ProviderConfiguration()
    {
      Id = this.Id,
      Level = this.Level,
      KeywordsAny = this.KeywordsAny,
      KeywordsAll = this.KeywordsAll
    };

    public void MergeForEnable(ProviderConfiguration otherConfiguration)
    {
      if (!(this.Id == otherConfiguration.Id))
        return;
      this.Level = this.Level > otherConfiguration.Level ? this.Level : otherConfiguration.Level;
      this.KeywordsAny |= otherConfiguration.KeywordsAny;
      this.KeywordsAll &= otherConfiguration.KeywordsAll;
    }
  }
}
