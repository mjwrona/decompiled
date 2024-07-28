// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl.MemoryPartition
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl
{
  public class MemoryPartition
  {
    public readonly SortedList<string, MemoryRow> Rows = new SortedList<string, MemoryRow>((IComparer<string>) StringComparer.Ordinal);
    private long Etag = 1;

    public string GetNextEtag()
    {
      ++this.Etag;
      return this.Etag.ToString();
    }

    public bool IsEmpty => this.Rows.Count == 0;

    public override int GetHashCode()
    {
      int hashCode = 0;
      foreach (KeyValuePair<string, MemoryRow> row in this.Rows)
      {
        hashCode = hashCode << 1 | hashCode >> 31 & 1;
        hashCode |= row.Key.GetHashCode();
        hashCode |= row.Value.GetHashCode();
      }
      return hashCode;
    }
  }
}
