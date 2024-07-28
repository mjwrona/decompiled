// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.StreamCountEventArgs
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using System;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public class StreamCountEventArgs : EventArgs
  {
    public StreamCountEventArgs(
      string tableName,
      int shard,
      int liveStreamsCount,
      int reoveryStreamsCount)
    {
      this.TableName = tableName;
      this.Shard = shard;
      this.LiveStreamsCount = liveStreamsCount;
      this.RecoveryStreamsCount = reoveryStreamsCount;
    }

    public string TableName { get; private set; }

    public int Shard { get; private set; }

    public int LiveStreamsCount { get; private set; }

    public int RecoveryStreamsCount { get; set; }
  }
}
