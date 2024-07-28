// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.PrefixResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class PrefixResult
  {
    private string start;
    private string end;
    private List<string> prefixes;
    private List<string> corePrefixes;

    internal PrefixResult(
      List<string> prefixes,
      List<string> corePrefixes,
      string start,
      string end)
    {
      this.start = start;
      this.end = end;
      this.prefixes = prefixes;
      this.corePrefixes = corePrefixes;
    }

    public List<string> Prefixes => this.prefixes;

    public string Head => this.start;

    public string Tail => this.end;

    public List<string> CorePrefixes => this.corePrefixes;

    public string PreCorePrefix
    {
      get
      {
        string corePrefix = this.corePrefixes[0];
        string a = this.prefixes[0].Substring(0, corePrefix.Length);
        return !string.Equals(a, corePrefix, StringComparison.OrdinalIgnoreCase) ? a : (string) null;
      }
    }

    public string PostCorePrefix
    {
      get
      {
        string corePrefix = this.corePrefixes[this.corePrefixes.Count - 1];
        string a = this.prefixes[this.prefixes.Count - 1].Substring(0, corePrefix.Length);
        return !string.Equals(a, corePrefix, StringComparison.OrdinalIgnoreCase) ? a : (string) null;
      }
    }
  }
}
