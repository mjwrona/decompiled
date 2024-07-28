// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.WorkItemCollectionOnboardingHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class WorkItemCollectionOnboardingHelper
  {
    private const string WorkItemOnboardingIsTriggeredRegistryPrefix = "WorkItemOnboardingTriggered";

    public static bool IsOnboardingTriggered(IVssRequestContext requestContext) => new RegistryManagerV2(requestContext, "Common").GetRegistryEntry("WorkItemOnboardingTriggered", requestContext.GetCollectionID().ToString()) != null;

    public static void RecordOnboardingWasTriggered(IVssRequestContext requestContext) => new RegistryManagerV2(requestContext, "Common").AddOrUpdateRegistryValue("WorkItemOnboardingTriggered", requestContext.GetCollectionID().ToString(), true.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }
}
