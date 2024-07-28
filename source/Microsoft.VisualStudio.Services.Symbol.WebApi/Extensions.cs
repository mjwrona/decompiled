// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.Extensions
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Symbol.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  public static class Extensions
  {
    public static DebugEntry SelectBestEntry(this IEnumerable<DebugEntry> candidates) => candidates.Any<DebugEntry>() ? candidates.OrderByDescending<DebugEntry, DebugInformationLevel>((Func<DebugEntry, DebugInformationLevel>) (c => c.InformationLevel)).First<DebugEntry>() : (DebugEntry) null;

    public static void ValidateDebugEntryIdFormat(this string debugEntryId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(debugEntryId, nameof (debugEntryId));
      if (Locator.Parse(debugEntryId).PathSegmentCount != 4)
        throw new FormatException("Specified debugEntryId is not valid - must conform to 'filenameOnly/clientIdentifier/filenameOnly/informationLevel(8 characters)' format.");
    }
  }
}
