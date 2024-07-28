// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.ProcessWorkItemTypeFieldExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  public static class ProcessWorkItemTypeFieldExtensions
  {
    private const string c_currentDateTime = "$currentdatetime";
    private const string c_currentUser = "$currentuser";

    public static WorkItemTypeletFieldRuleProperties ToServerProperties(
      this AddProcessWorkItemTypeFieldRequest request,
      IVssRequestContext requestContext)
    {
      return new WorkItemTypeletFieldRuleProperties(request.Required, request.ReadOnly, ProcessWorkItemTypeFieldExtensions.GetDefaultValueAsString(requestContext, request.DefaultValue), ProcessWorkItemTypeFieldExtensions.GetDefaultValueFrom(request.DefaultValue), request.AllowGroups, request.AllowedValues);
    }

    public static WorkItemTypeletFieldRuleProperties ToServerProperties(
      this UpdateProcessWorkItemTypeFieldRequest request,
      IVssRequestContext requestContext)
    {
      return new WorkItemTypeletFieldRuleProperties(request.Required, request.ReadOnly, ProcessWorkItemTypeFieldExtensions.GetDefaultValueAsString(requestContext, request.DefaultValue), ProcessWorkItemTypeFieldExtensions.GetDefaultValueFrom(request.DefaultValue), request.AllowGroups, request.AllowedValues);
    }

    private static RuleValueFrom GetDefaultValueFrom(object defaultValue)
    {
      if (defaultValue is string)
      {
        switch ((defaultValue as string).ToLowerInvariant())
        {
          case "$currentdatetime":
            return RuleValueFrom.Clock;
          case "$currentuser":
            return RuleValueFrom.CurrentUser;
        }
      }
      return RuleValueFrom.Value;
    }

    private static string GetDefaultValueAsString(
      IVssRequestContext requestContext,
      object defaultValue)
    {
      switch (defaultValue)
      {
        case string _:
        case DateTime _:
          return defaultValue.ToString();
        case IdentityRef _:
          IdentityRef identityRef = defaultValue as IdentityRef;
          WorkItemIdentity workItemIdentity = new WorkItemIdentity()
          {
            IdentityRef = identityRef
          };
          ResolvedIdentityNamesInfo identityNamesInfo = requestContext.GetService<IWorkItemIdentityService>().ResolveIdentityNames(requestContext.WitContext(), Enumerable.Empty<string>(), (IEnumerable<WorkItemIdentity>) new List<WorkItemIdentity>()
          {
            workItemIdentity
          }, false);
          if (identityNamesInfo.AllRecords.Count<ConstantsSearchRecord>() == 1)
            return identityNamesInfo.AllRecords.First<ConstantsSearchRecord>().TeamFoundationId.ToString();
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.CannotResolveIdentityByDescriptor((object) workItemIdentity.IdentityRef.Descriptor), "defaultIdentityValue");
        default:
          return string.Empty;
      }
    }
  }
}
