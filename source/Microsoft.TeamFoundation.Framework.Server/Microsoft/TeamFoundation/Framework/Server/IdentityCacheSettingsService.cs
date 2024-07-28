// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityCacheSettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class IdentityCacheSettingsService : IIdentityCacheSettingsService, IVssFrameworkService
  {
    private ImmutableHashSet<Guid> m_orgsToSkipAadGroupsMembershipChange;
    private ImmutableHashSet<string> m_aadGroupSidsToSkipMembershipChange;
    private ImmutableHashSet<string> m_wellKnownGroupSidsToSkipMembershipChange;
    private static readonly RegistryQuery s_aadGroupOidsToSkipMembershipChangeRegistryQuery = new RegistryQuery("/Service/Identity/Settings/AadGroupOidsToSkipMembershipChange");
    private static readonly RegistryQuery s_wellKnownGroupSidsToSkipMembershipChangeRegistryQuery = new RegistryQuery("/Service/Identity/Settings/WellKnownGroupSidsToSkipMembershipChange");
    private static readonly RegistryQuery s_orgsToSkipAadGroupsMembershipChangeRegistryQuery = new RegistryQuery("/Service/Identity/Settings/OrgsToSkipAadGroupsMembershipChange");

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckDeploymentRequestContext();
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      service.RegisterNotification(context, new RegistrySettingsChangedCallback(this.OnAadGroupsToSkipRegistrySettingsChanged), in IdentityCacheSettingsService.s_aadGroupOidsToSkipMembershipChangeRegistryQuery);
      Interlocked.CompareExchange<ImmutableHashSet<string>>(ref this.m_aadGroupSidsToSkipMembershipChange, IdentityCacheSettingsService.GetAadGroupSidsToSkipMembershipChangeInternal(context), (ImmutableHashSet<string>) null);
      service.RegisterNotification(context, new RegistrySettingsChangedCallback(this.OnWellKnownGroupsToSkipRegistrySettingsChanged), in IdentityCacheSettingsService.s_wellKnownGroupSidsToSkipMembershipChangeRegistryQuery);
      Interlocked.CompareExchange<ImmutableHashSet<string>>(ref this.m_wellKnownGroupSidsToSkipMembershipChange, IdentityCacheSettingsService.GetWellKnownGroupSidsToSkipMembershipChangeInternal(context), (ImmutableHashSet<string>) null);
      service.RegisterNotification(context, new RegistrySettingsChangedCallback(this.OnOrganizationsToSkipRegistrySettingsChanged), in IdentityCacheSettingsService.s_orgsToSkipAadGroupsMembershipChangeRegistryQuery);
      Interlocked.CompareExchange<ImmutableHashSet<Guid>>(ref this.m_orgsToSkipAadGroupsMembershipChange, IdentityCacheSettingsService.GetOrgsToSkipAadGroupsMembershipChangeInternal(context), (ImmutableHashSet<Guid>) null);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      service.UnregisterNotification(context, new RegistrySettingsChangedCallback(this.OnAadGroupsToSkipRegistrySettingsChanged));
      service.UnregisterNotification(context, new RegistrySettingsChangedCallback(this.OnOrganizationsToSkipRegistrySettingsChanged));
    }

    public ImmutableHashSet<Guid> GetOrgsToSkipAadGroupsMembershipChange(IVssRequestContext context) => this.m_orgsToSkipAadGroupsMembershipChange;

    public ImmutableHashSet<string> GetAadGroupSidsToSkipMembershipChange(IVssRequestContext context) => this.m_aadGroupSidsToSkipMembershipChange;

    public ImmutableHashSet<string> GetWellKnownGroupSidsToSkipMembershipChange(
      IVssRequestContext context)
    {
      return this.m_wellKnownGroupSidsToSkipMembershipChange;
    }

    private void OnAadGroupsToSkipRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<ImmutableHashSet<string>>(ref this.m_aadGroupSidsToSkipMembershipChange, IdentityCacheSettingsService.GetAadGroupSidsToSkipMembershipChangeInternal(requestContext));
    }

    private void OnWellKnownGroupsToSkipRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<ImmutableHashSet<string>>(ref this.m_wellKnownGroupSidsToSkipMembershipChange, IdentityCacheSettingsService.GetWellKnownGroupSidsToSkipMembershipChangeInternal(requestContext));
    }

    private void OnOrganizationsToSkipRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<ImmutableHashSet<Guid>>(ref this.m_orgsToSkipAadGroupsMembershipChange, IdentityCacheSettingsService.GetOrgsToSkipAadGroupsMembershipChangeInternal(requestContext));
    }

    private static ImmutableHashSet<Guid> GetOrgsToSkipAadGroupsMembershipChangeInternal(
      IVssRequestContext context)
    {
      context.CheckDeploymentRequestContext();
      string str = context.GetService<IVssRegistryService>().GetValue<string>(context, in IdentityCacheSettingsService.s_orgsToSkipAadGroupsMembershipChangeRegistryQuery, (string) null);
      return str.IsNullOrEmpty<char>() ? (ImmutableHashSet<Guid>) null : ImmutableHashSet.Create<Guid>().Union((IEnumerable<Guid>) IdentityCacheSettingsService.ParseCommaSeparetedGuids(str));
    }

    private static ImmutableHashSet<string> GetAadGroupSidsToSkipMembershipChangeInternal(
      IVssRequestContext context)
    {
      context.CheckDeploymentRequestContext();
      string str = context.GetService<IVssRegistryService>().GetValue<string>(context, in IdentityCacheSettingsService.s_aadGroupOidsToSkipMembershipChangeRegistryQuery, (string) null);
      if (str.IsNullOrEmpty<char>())
        return (ImmutableHashSet<string>) null;
      IList<Guid> commaSeparetedGuids = IdentityCacheSettingsService.ParseCommaSeparetedGuids(str);
      return commaSeparetedGuids.IsNullOrEmpty<Guid>() ? (ImmutableHashSet<string>) null : ImmutableHashSet.Create<string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase).Union(commaSeparetedGuids.Select<Guid, string>((Func<Guid, string>) (x => SidIdentityHelper.ConstructAadGroupSid(x).ToString())));
    }

    private static ImmutableHashSet<string> GetWellKnownGroupSidsToSkipMembershipChangeInternal(
      IVssRequestContext context)
    {
      context.CheckDeploymentRequestContext();
      string str = context.GetService<IVssRegistryService>().GetValue<string>(context, in IdentityCacheSettingsService.s_wellKnownGroupSidsToSkipMembershipChangeRegistryQuery, (string) null);
      if (str.IsNullOrEmpty<char>())
        return (ImmutableHashSet<string>) null;
      IList<string> separetedStrings = IdentityCacheSettingsService.ParseCommaSeparetedStrings(str);
      return separetedStrings.IsNullOrEmpty<string>() ? (ImmutableHashSet<string>) null : ImmutableHashSet.Create<string>((IEqualityComparer<string>) SidIdentityHelper.WellKnownGroupSidComparer).Union((IEnumerable<string>) separetedStrings);
    }

    private static IList<Guid> ParseCommaSeparetedGuids(string commaSeparatedGuidsAsString)
    {
      IList<Guid> commaSeparetedGuids = (IList<Guid>) new List<Guid>();
      if (string.IsNullOrWhiteSpace(commaSeparatedGuidsAsString))
        return commaSeparetedGuids;
      string str = commaSeparatedGuidsAsString;
      char[] chArray = new char[1]{ ',' };
      foreach (string input in str.Split(chArray))
      {
        Guid result;
        if (Guid.TryParse(input, out result))
          commaSeparetedGuids.Add(result);
      }
      return commaSeparetedGuids;
    }

    private static IList<string> ParseCommaSeparetedStrings(string commaSeparatedString)
    {
      IList<string> separetedStrings = (IList<string>) new List<string>();
      if (string.IsNullOrWhiteSpace(commaSeparatedString))
        return separetedStrings;
      string str1 = commaSeparatedString;
      char[] chArray = new char[1]{ ',' };
      foreach (string str2 in str1.Split(chArray))
      {
        if (!string.IsNullOrWhiteSpace(str2))
          separetedStrings.Add(str2);
      }
      return separetedStrings;
    }
  }
}
