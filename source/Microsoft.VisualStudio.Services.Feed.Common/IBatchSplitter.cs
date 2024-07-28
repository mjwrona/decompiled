// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.IBatchSplitter
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public interface IBatchSplitter
  {
    IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> items);
  }
}
