// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ListExtensions
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class ListExtensions
  {
    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize) => source.Select((x, i) => new
    {
      Index = i,
      Value = x
    }).GroupBy(x => x.Index / chunkSize).Select<IGrouping<int, \u003C\u003Ef__AnonymousType3<int, T>>, List<T>>(x => x.Select(v => v.Value).ToList<T>()).ToList<List<T>>();
  }
}
