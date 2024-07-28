// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ReencryptResults
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ReencryptResults
  {
    private const int MaxFailures = 512;

    public ReencryptResults() => this.Failures = new List<Exception>();

    public int SuccessCount { get; set; }

    public int FailureCount { get; set; }

    public List<Exception> Failures { get; set; }

    internal void Merge(ReencryptResults results)
    {
      this.SuccessCount += results.SuccessCount;
      this.FailureCount += results.FailureCount;
      int count = 512 - this.Failures.Count;
      if (count <= 0)
        return;
      this.Failures.AddRange(results.Failures.Take<Exception>(count));
    }

    public override string ToString()
    {
      StringWriter stringWriter = new StringWriter();
      if (this.SuccessCount > 0)
        stringWriter.WriteLine("{0} item(s) re-encrypted.", (object) this.SuccessCount);
      if (this.FailureCount > 0)
        stringWriter.WriteLine("{0} item(s) failed.", (object) this.FailureCount);
      foreach (Exception failure in this.Failures)
        stringWriter.WriteLine((object) failure);
      return stringWriter.ToString();
    }
  }
}
