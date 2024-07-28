// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableMigrationTimeTracker
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class TableMigrationTimeTracker
  {
    private ConcurrentDictionary<ITrackingKey, DateTime?> startTimes;
    private TimeTrackingKeyType type;

    public TableMigrationTimeTracker(TimeTrackingKeyType type)
    {
      this.type = type != TimeTrackingKeyType.Uncategorized ? type : throw new ArgumentException("Cannot create a TableMigrationTimeTracker with " + type.ToString() + " key type.");
      this.startTimes = new ConcurrentDictionary<ITrackingKey, DateTime?>();
    }

    public void Put(ITrackingKey key, DateTime? dt)
    {
      if (key.Type != this.type)
        throw new InvalidOperationException("The time tracking key's type (" + key.Type.ToString() + ") is incompitable with the tracker's configured type (" + this.type.ToString() + ").");
      this.startTimes[key] = dt;
    }

    public DateTime? Get(ITrackingKey key)
    {
      if (key.Type != this.type)
        throw new InvalidOperationException("The time tracking key's type (" + key.Type.ToString() + ") is incompitable with the tracker's configured type (" + this.type.ToString() + ").");
      DateTime? nullable;
      return this.startTimes.TryGetValue(key, out nullable) ? nullable : new DateTime?();
    }
  }
}
