// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.FullRefreshStatusRecord
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking
{
  public class FullRefreshStatusRecord
  {
    public DateTime Timestamp { get; }

    public IReadOnlyList<UpstreamStatusCategory> Categories { get; }

    public FullRefreshStatusRecord(
      DateTime timestamp,
      IEnumerable<UpstreamStatusCategory> categories)
    {
      if (categories == null)
        throw new ArgumentNullException(nameof (categories));
      this.Timestamp = timestamp;
      this.Categories = (IReadOnlyList<UpstreamStatusCategory>) categories.ToImmutableList<UpstreamStatusCategory>();
    }
  }
}
