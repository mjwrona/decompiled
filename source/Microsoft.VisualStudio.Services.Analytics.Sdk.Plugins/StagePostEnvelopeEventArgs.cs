// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.StagePostEnvelopeEventArgs
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using System;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public class StagePostEnvelopeEventArgs : EventArgs
  {
    public StagePostEnvelopeEventArgs(
      string tableName,
      int shard,
      StageStreamInfo stream,
      StagePostEnvelope envelope,
      TimeSpan duration)
    {
      this.TableName = tableName;
      this.Shard = shard;
      this.Stream = stream;
      this.RecordCount = envelope.Records != null ? envelope.Records.Count : 0;
      this.Envelope = envelope;
      this.Duration = duration;
    }

    public string TableName { get; private set; }

    public int Shard { get; private set; }

    public StageStreamInfo Stream { get; private set; }

    public int RecordCount { get; private set; }

    public StagePostEnvelope Envelope { get; private set; }

    public TimeSpan Duration { get; private set; }
  }
}
