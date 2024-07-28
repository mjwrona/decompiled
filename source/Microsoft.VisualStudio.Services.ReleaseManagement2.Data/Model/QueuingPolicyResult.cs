// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.QueuingPolicyResult
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class QueuingPolicyResult
  {
    public QueuingPolicyResult()
    {
      this.EnvironmentsToQueue = Enumerable.Empty<ReleaseEnvironmentQueueData>();
      this.EnvironmentsCanceled = Enumerable.Empty<ReleaseEnvironmentQueueData>();
    }

    public IEnumerable<ReleaseEnvironmentQueueData> EnvironmentsToQueue { get; set; }

    public IEnumerable<ReleaseEnvironmentQueueData> EnvironmentsCanceled { get; set; }
  }
}
