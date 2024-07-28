// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupLicensingRule.GroupRuleStatusExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.GroupLicensingRule
{
  public static class GroupRuleStatusExtensions
  {
    public static GroupLicensingRuleStatus HighestSeverity(
      this IEnumerable<GroupLicensingRuleStatus> statuses)
    {
      return statuses.Max<GroupLicensingRuleStatus>();
    }
  }
}
