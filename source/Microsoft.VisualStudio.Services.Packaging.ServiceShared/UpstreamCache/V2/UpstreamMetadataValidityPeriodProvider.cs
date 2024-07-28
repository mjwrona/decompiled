// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamMetadataValidityPeriodProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class UpstreamMetadataValidityPeriodProvider : IUpstreamMetadataValidityPeriodProvider
  {
    public UpstreamMetadataValidityPeriodProvider(
      IFrotocolLevelPackagingSetting<TimeSpan> validityPeriodWithEntriesSetting,
      IFrotocolLevelPackagingSetting<TimeSpan> validityPeriodWithNoEntriesSetting,
      IOrgLevelPackagingSetting<bool> differentValidityPeriodIfNoEntriesSetting)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CvalidityPeriodWithEntriesSetting\u003EP = validityPeriodWithEntriesSetting;
      // ISSUE: reference to a compiler-generated field
      this.\u003CvalidityPeriodWithNoEntriesSetting\u003EP = validityPeriodWithNoEntriesSetting;
      // ISSUE: reference to a compiler-generated field
      this.\u003CdifferentValidityPeriodIfNoEntriesSetting\u003EP = differentValidityPeriodIfNoEntriesSetting;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public TimeSpan GetValidityPeriod(IFeedRequest feedRequest, IMetadataDocument metadataDocument) => !metadataDocument.Entries.Any<IMetadataEntry>() && this.\u003CdifferentValidityPeriodIfNoEntriesSetting\u003EP.Get() ? this.\u003CvalidityPeriodWithNoEntriesSetting\u003EP.Get(feedRequest).AbsoluteValue() : this.\u003CvalidityPeriodWithEntriesSetting\u003EP.Get(feedRequest).AbsoluteValue();
  }
}
