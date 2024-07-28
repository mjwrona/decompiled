// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.DfpjChangeFilter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.Exceptions;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class DfpjChangeFilter
  {
    private static bool IsDeletedFeedEligibleForCleanup(

      #nullable disable
      FeedChange deletedFeedChange,
      DateTime maxEligiblePermanentDeletedDate)
    {
      if (!deletedFeedChange.Feed.DeletedDate.HasValue || !deletedFeedChange.Feed.PermanentDeletedDate.HasValue || DateTime.MinValue == deletedFeedChange.Feed.PermanentDeletedDate.Value)
        throw new MissingRequiredDataException("Unexpected feedchange data. Missing field PermanentDeletedDate");
      return deletedFeedChange.Feed.PermanentDeletedDate.Value.ToUniversalTime() <= maxEligiblePermanentDeletedDate.ToUniversalTime();
    }

    public static IAsyncEnumerable<DfpjFilteredFeedChange> FilterFeedsToDeleteAsyncEnumerable(
      IAsyncEnumerable<FeedChange> changes,
      DateTime maxEligiblePermanentDeletedDate,
      long initialContinuationToken)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IAsyncEnumerable<DfpjFilteredFeedChange>) new DfpjChangeFilter.\u003CFilterFeedsToDeleteAsyncEnumerable\u003Ed__1(-2)
      {
        \u003C\u003E3__changes = changes,
        \u003C\u003E3__maxEligiblePermanentDeletedDate = maxEligiblePermanentDeletedDate,
        \u003C\u003E3__initialContinuationToken = initialContinuationToken
      };
    }
  }
}
