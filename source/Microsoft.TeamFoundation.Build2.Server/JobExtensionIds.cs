// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.JobExtensionIds
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class JobExtensionIds
  {
    public static readonly Guid PausedBuildsQueueEvaluationJob = new Guid("E5355F36-6F5B-4708-8A64-68E6DF6CDDA5");
    public static readonly Guid BuildCheckEventDeliveryJob = new Guid("DCA4FEA8-3292-490F-8839-BDAF3C3EFD1E");
    public static readonly Guid AbortPoisonedBuildsJob = new Guid("40CA746D-50E4-4137-BD23-66E604FA24CD");
  }
}
