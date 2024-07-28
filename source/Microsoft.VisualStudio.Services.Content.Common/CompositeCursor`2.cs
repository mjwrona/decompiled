// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.CompositeCursor`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [Serializable]
  public class CompositeCursor<TId, TCursor> : Dictionary<TId, TCursor>
  {
    public CompositeCursor()
    {
    }

    public CompositeCursor(IEnumerable<KeyValuePair<TId, TCursor>> pairs)
      : base(pairs.ToDictionary<TId, TCursor>())
    {
    }
  }
}
