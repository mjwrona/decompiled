// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityServiceHelpers
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SecurityServiceHelpers
  {
    private const string c_badFormattingMessage = "The passed trace is not formatted correctly";
    private const string c_area = "Security";
    private const string c_layer = "SecurityServiceHelpers";

    public static void ChangeInheritance(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string securityToken,
      bool inheritPermissions)
    {
      string parentSecurityToken = SecurityServiceHelpers.GetParentSecurityToken(securityNamespace, securityToken);
      IAccessControlList accessControlList1 = securityNamespace.QueryAccessControlLists(requestContext.Elevate(), parentSecurityToken, true, false).FirstOrDefault<IAccessControlList>();
      IAccessControlList accessControlList2 = securityNamespace.QueryAccessControlLists(requestContext, securityToken, true, false).FirstOrDefault<IAccessControlList>();
      if (accessControlList1 == null || !accessControlList1.AccessControlEntries.Any<IAccessControlEntry>())
        return;
      Dictionary<IdentityDescriptor, IAccessControlEntry> dictionary = new Dictionary<IdentityDescriptor, IAccessControlEntry>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      if (accessControlList2 != null && accessControlList2.AccessControlEntries.Any<IAccessControlEntry>())
      {
        foreach (IAccessControlEntry accessControlEntry in accessControlList2.AccessControlEntries)
          dictionary[accessControlEntry.Descriptor] = accessControlEntry;
      }
      if (inheritPermissions)
      {
        List<IdentityDescriptor> identityDescriptorList = new List<IdentityDescriptor>();
        foreach (IAccessControlEntry accessControlEntry1 in accessControlList1.AccessControlEntries)
        {
          if (accessControlEntry1 != null)
          {
            IAccessControlEntry accessControlEntry2 = (IAccessControlEntry) null;
            if (!dictionary.TryGetValue(accessControlEntry1.Descriptor, out accessControlEntry2))
              identityDescriptorList.Add(accessControlEntry1.Descriptor);
          }
        }
        if (identityDescriptorList.Count > 0)
        {
          foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) identityDescriptorList.ToArray(), QueryMembership.None, (IEnumerable<string>) null))
          {
            if (readIdentity != null)
            {
              IAccessControlEntry accessControlEntry = securityNamespace.QueryAccessControlList(requestContext, securityToken, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
              {
                readIdentity.Descriptor
              }, false).AccessControlEntries.FirstOrDefault<IAccessControlEntry>() ?? (IAccessControlEntry) new AccessControlEntry(readIdentity.Descriptor, 0, 0);
              dictionary[readIdentity.Descriptor] = accessControlEntry;
            }
          }
        }
      }
      List<IAccessControlEntry> accessControlEntryList = new List<IAccessControlEntry>();
      foreach (IAccessControlEntry accessControlEntry3 in accessControlList1.AccessControlEntries)
      {
        IAccessControlEntry accessControlEntry4;
        if (dictionary.TryGetValue(accessControlEntry3.Descriptor, out accessControlEntry4))
        {
          if (inheritPermissions)
          {
            accessControlEntry4.Allow = 0;
            accessControlEntry4.Deny = 0;
          }
          else
          {
            foreach (ActionDefinition action in securityNamespace.Description.Actions)
            {
              if ((accessControlEntry3.EffectiveDeny & action.Bit) == action.Bit && (accessControlEntry4.Allow & action.Bit) != action.Bit)
                accessControlEntry4.Deny |= action.Bit;
              if ((accessControlEntry3.EffectiveAllow & action.Bit) == action.Bit && (accessControlEntry4.Deny & action.Bit) != action.Bit)
                accessControlEntry4.Allow |= action.Bit;
            }
          }
          accessControlEntryList.Add(accessControlEntry4);
        }
      }
      securityNamespace.SetInheritFlag(requestContext, securityToken, inheritPermissions);
      securityNamespace.SetAccessControlEntries(requestContext.Elevate(), securityToken, (IEnumerable<IAccessControlEntry>) accessControlEntryList, false);
    }

    public static IdentityDescriptor CreateStatelessAceDescriptor(
      Guid namespaceId,
      string token,
      int allow,
      int deny)
    {
      ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      if (allow == 0 && deny == 0)
        throw new ArgumentOutOfRangeException();
      return new IdentityDescriptor("System:AccessControl", string.Format("{0};{1};{2};{3}", (object) namespaceId.ToString("D"), (object) allow, (object) deny, (object) token));
    }

    internal static string EvaluationPrincipalToString(
      IVssRequestContext requestContext,
      EvaluationPrincipal evaluationPrincipal)
    {
      return evaluationPrincipal != null ? SecurityServiceHelpers.DescriptorToString(requestContext, evaluationPrincipal.PrimaryDescriptor) : string.Empty;
    }

    internal static string DescriptorToString(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('(');
      stringBuilder.Append(EuiiUtility.MaskEmail(descriptor.ToString()));
      if (!descriptor.Equals(new IdentityDescriptor("System:ServicePrincipal", "*")))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptor
        }, QueryMembership.None, (IEnumerable<string>) null);
        if (source != null)
          identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
        {
          stringBuilder.Append(" VSID: ");
          stringBuilder.Append(identity.Id.ToString("D"));
        }
      }
      stringBuilder.Append(')');
      return stringBuilder.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string CanonicalizeToken(SecurityNamespaceDescription description, string token)
    {
      if (token != null && SecurityNamespaceStructure.Hierarchical == description.NamespaceStructure && description.SeparatorValue != char.MinValue)
      {
        while (token.Length > 0 && (int) token[token.Length - 1] == (int) description.SeparatorValue)
          token = token.Substring(0, token.Length - 1);
      }
      return token;
    }

    internal static bool IsTracingSecurityEvaluation(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer)
    {
      IDictionary<string, object> items = requestContext.RootContext.Items;
      return (items != null ? (items.ContainsKey(RequestContextItemsKeys.SecurityEvaluationLogKey) ? 1 : 0) : 0) != 0 || requestContext.IsTracing(tracepoint, level, area, layer);
    }

    internal static void TraceSecurityEvaluation(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      StringBuilder stringBuilder;
      if (requestContext.Items != null && requestContext.RootContext.Items.TryGetValue<StringBuilder>(RequestContextItemsKeys.SecurityEvaluationLogKey, out stringBuilder))
        stringBuilder.AppendLine(SecurityServiceHelpers.SanitizeMessage(format, args));
      VssRequestContextExtensions.Trace(requestContext, tracepoint, level, area, layer, format, args);
    }

    internal static void ClearCachedSecurityEvaluators(IVssRequestContext requestContext)
    {
      foreach (string key in requestContext.Items.Keys.Where<string>((Func<string, bool>) (x => x != null && x.StartsWith(RequestContextItemsKeys.SecurityEvaluatorKeyBase, StringComparison.OrdinalIgnoreCase))).ToArray<string>())
        requestContext.Items.Remove(key);
    }

    private static string SanitizeMessage(string format, object[] args)
    {
      try
      {
        return args == null || args.Length == 0 ? format ?? string.Empty : string.Format((IFormatProvider) CultureInfo.InvariantCulture, format ?? string.Empty, args);
      }
      catch (FormatException ex)
      {
        return "The passed trace is not formatted correctly";
      }
    }

    private static string GetParentSecurityToken(IVssSecurityNamespace @namespace, string token)
    {
      token = @namespace.IsHierarchical() ? @namespace.CheckAndCanonicalizeToken(token) : throw new InvalidOperationException(FrameworkResources.NoParentTokenForFlatSecurityNamespace((object) token));
      if (@namespace.Description.SeparatorValue != char.MinValue)
      {
        int length = token.LastIndexOf(@namespace.Description.SeparatorValue);
        return length >= 0 ? token.Substring(0, length) : throw new InvalidOperationException(FrameworkResources.NoParentTokenForRootToken((object) token));
      }
      if (token.Length == 0)
        throw new InvalidOperationException(FrameworkResources.NoParentTokenForRootToken((object) token));
      return token.Substring(0, token.Length - @namespace.Description.ElementLength);
    }
  }
}
