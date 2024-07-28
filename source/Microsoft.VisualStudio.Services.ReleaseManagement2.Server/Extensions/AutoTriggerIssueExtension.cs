// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.AutoTriggerIssueExtension
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class AutoTriggerIssueExtension
  {
    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue> ToAutoTriggerIssuesContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue> autoTriggerIssueList,
      IVssRequestContext context)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return autoTriggerIssueList.ToAutoTriggerIssuesContract(context, AutoTriggerIssueExtension.\u003C\u003EO.\u003C0\u003E__ToWebApiAutoTriggerIssueContract ?? (AutoTriggerIssueExtension.\u003C\u003EO.\u003C0\u003E__ToWebApiAutoTriggerIssueContract = new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue, IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue>(AutoTriggerIssueExtension.ToWebApiAutoTriggerIssueContract)));
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue> ToAutoTriggerIssuesContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue> autoTriggerIssueList,
      IVssRequestContext context,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue, IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue> convertToWebApiAutoTriggerIssueContract)
    {
      if (convertToWebApiAutoTriggerIssueContract == null)
        throw new ArgumentNullException(nameof (convertToWebApiAutoTriggerIssueContract));
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue>) autoTriggerIssueList.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue>) (autoTriggerIssue => convertToWebApiAutoTriggerIssueContract(autoTriggerIssue, context))).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue>();
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.AutoTriggerIssue ToWebApiAutoTriggerIssueContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue autoTriggerIssueList,
      IVssRequestContext context)
    {
      return autoTriggerIssueList.ConvertAutoTriggerIssueModelToContract(context);
    }
  }
}
