// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.ProcessRuleModelFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class ProcessRuleModelFactory
  {
    public static string GetLocationUrlForWorkItemTypeRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      Guid ruleId)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processes", WorkItemTrackingLocationIds.ProcessWorkItemTypesRules, (object) new
      {
        processId = processId,
        witRefName = witRefName,
        ruleId = ruleId
      }).AbsoluteUri;
    }
  }
}
