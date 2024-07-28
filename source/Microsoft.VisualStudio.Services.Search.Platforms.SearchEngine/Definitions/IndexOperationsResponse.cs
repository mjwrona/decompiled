// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.IndexOperationsResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public class IndexOperationsResponse
  {
    public bool Success { get; set; }

    public long ItemsCount { get; set; }

    public long FailedItemsCount { get; set; }

    public IEnumerable<FailedItem> FailedItems { get; set; }

    public bool IsOperationIncomplete { get; set; }

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("{0} = {1}, {2} = {3}, {4} = {5}", (object) "Success", (object) this.Success, (object) "ItemsCount", (object) this.ItemsCount, (object) "FailedItemsCount", (object) this.FailedItemsCount));
  }
}
