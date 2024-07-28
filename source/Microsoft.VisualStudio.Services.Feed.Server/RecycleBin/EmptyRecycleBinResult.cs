// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.RecycleBin.EmptyRecycleBinResult
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server.RecycleBin
{
  public class EmptyRecycleBinResult
  {
    public EmptyRecycleBinResult(Guid feedId) => this.FeedId = feedId;

    public Guid FeedId { get; }

    public int SuccessfulDeletes { get; set; }

    public int AttemptedDeletes { get; set; }

    public int FailedDeletes { get; set; }

    public ISet<string> FailedProtocols { get; set; } = (ISet<string>) new HashSet<string>();
  }
}
