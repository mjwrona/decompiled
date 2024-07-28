// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.ProfilingReport
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public class ProfilingReport
  {
    private Dictionary<ProfilingGroup, List<Tuple<string, long>>> dict;

    internal ProfilingReport(
      Dictionary<ProfilingGroup, List<Tuple<string, long>>> dict)
    {
      this.dict = dict;
    }

    public List<Tuple<string, long>> GetByGroup(ProfilingGroup grp)
    {
      List<Tuple<string, long>> tupleList;
      return this.dict.TryGetValue(grp, out tupleList) ? tupleList : (List<Tuple<string, long>>) null;
    }
  }
}
