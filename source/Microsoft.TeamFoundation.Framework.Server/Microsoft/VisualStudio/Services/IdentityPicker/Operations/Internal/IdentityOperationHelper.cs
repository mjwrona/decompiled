// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.IdentityOperationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal sealed class IdentityOperationHelper
  {
    private const char queryTokenSeparator = ';';
    private const string prefixSubRegex = "\\s*(?<prefix>[\\w'\\\"`~!#$&%^:*,\\.?_+=()[\\\\-][\\w\\s'\\\"`~!#$&%^:*,\\.?_+=()@[\\]\\\\-]*)";
    private const string emailSubRegex = "\\s*(?<mailaddress>[^\\s]+@[^\\s]+\\.[^\\s]+)\\s*";
    private static readonly Regex FullyQualifiedAddressRegex = new Regex("\\A\\s*(?<prefix>[\\w'\\\"`~!#$&%^:*,\\.?_+=()[\\\\-][\\w\\s'\\\"`~!#$&%^:*,\\.?_+=()@[\\]\\\\-]*)(\\s+|(?<emailstart>[<{\\[]))\\s*(?<mailaddress>[^\\s]+@[^\\s]+\\.[^\\s]+)\\s*(?(emailstart)[>}\\]])\\s*\\Z", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex MailRegex = new Regex("\\A\\s*(?<emailstart>[<{\\[])?\\s*(?<mailaddress>[^\\s]+@[^\\s]+\\.[^\\s]+)\\s*(?(emailstart)[>}\\]])\\Z", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex PrefixRegex = new Regex("\\A\\s*(?<prefix>[\\w'\\\"`~!#$&%^:*,\\.?_+=()[\\\\-][\\w\\s'\\\"`~!#$&%^:*,\\.?_+=()@[\\]\\\\-]*)\\Z", RegexOptions.Compiled | RegexOptions.Singleline);
    private const string c_gitHubIdentityAccountNameSuffix1 = "@Live";
    private const string c_gitHubIdentityAccountNameSuffix2 = "@GitHub";
    internal const string DomainGroup = "domain";
    internal const string SamAccountNameGroup = "samAccountNameGroup";
    internal static readonly Regex DomainSamAccountNameRegex = new Regex("\\A(?<domain>[a-zA-Z0-9][a-zA-Z0-9.\\-_]+)\\\\(?![\\x20.]+$)(?<samAccountNameGroup>[^\\\\\\/\"[\\]:|<>+=;,?*@]+)\\Z", RegexOptions.Compiled | RegexOptions.Singleline);

    internal static void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, "IVssRequestContext");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application) && !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new IdentityPickerValidateException("This request should only be directed at a valid account host or a valid project-collection host.");
    }

    internal static bool IsRequestByNonMember(IVssRequestContext requestContext)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, IdentityPickerSecurityConstants.NamespaceId);
      return securityNamespace != null && !securityNamespace.HasPermission(requestContext, IdentityPickerSecurityConstants.RootToken, 2);
    }

    internal static bool IsRequestByGuest(IVssRequestContext requestContext) => requestContext.GetUserIdentity().MetaType == IdentityMetaType.Guest;

    internal static bool IsExternalGuestAccessPolicyEnabled(IVssRequestContext requestContext)
    {
      try
      {
        if (!IdentityOperationHelper.IsAadBackedOrg(requestContext))
          return true;
        bool flag = AadGuestUserAccessHelper.IsAccessEnabled(requestContext);
        if (flag)
          Tracing.TraceInfo(requestContext, 573, "Guest user is enabled for host {0}.", (object) requestContext.ServiceHost.InstanceId);
        return flag;
      }
      catch (Exception ex)
      {
        Tracing.TraceException(requestContext, 574, ex);
        return false;
      }
    }

    internal static bool IsAadBackedOrg(IVssRequestContext requestContext)
    {
      if (requestContext.IsOrganizationAadBacked())
        return true;
      Tracing.TraceInfo(requestContext, 572, "Account {0} is not AAD backed.", (object) requestContext.ServiceHost.InstanceId);
      return false;
    }

    internal static void CheckRequestByAuthorizedMember(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.Identity.CheckGuestAccessForIdentityPicker.M202") && !IdentityOperationHelper.IsExternalGuestAccessPolicyEnabled(requestContext) && IdentityOperationHelper.IsRequestByGuest(requestContext))
        throw new IdentityPickerForbiddenOperationException("This request cannot be made by a guest member if the external guest access is not allowed");
      if (IdentityOperationHelper.IsRequestByNonMember(requestContext))
        throw new IdentityPickerAuthorizationException("This request cannot be made by an anonymous or non-member user");
    }

    internal static OperationScopeEnum ParseOperationScopes(IList<string> operationScopes)
    {
      if (operationScopes == null || operationScopes.Count == 0)
        throw new IdentityPickerArgumentException("operationScopes (required parameter) is null or empty");
      OperationScopeEnum operationScopeEnum = OperationScopeEnum.None;
      foreach (string operationScope in (IEnumerable<string>) operationScopes)
      {
        OperationScopeEnum result;
        if (!string.IsNullOrWhiteSpace(operationScope) && Enum.TryParse<OperationScopeEnum>(operationScope.Trim().ToLower(), true, out result))
          operationScopeEnum |= result;
      }
      return operationScopeEnum != OperationScopeEnum.None ? operationScopeEnum : throw new IdentityPickerArgumentException("No valid operation scope could be parsed");
    }

    internal static QueryTypeHintEnum ParseQueryTypeHint(string queryTypeHint)
    {
      QueryTypeHintEnum result = QueryTypeHintEnum.None;
      return string.IsNullOrWhiteSpace(queryTypeHint) || !Enum.TryParse<QueryTypeHintEnum>(queryTypeHint.Trim().ToLower(), true, out result) ? QueryTypeHintEnum.None : result;
    }

    internal static IdentityTypeEnum ParseIdentityTypes(IList<string> identityTypes)
    {
      if (identityTypes == null || identityTypes.Count == 0)
        throw new IdentityPickerArgumentException("identityTypes (required parameter) is null or empty");
      IdentityTypeEnum identityTypeEnum = IdentityTypeEnum.None;
      foreach (string identityType in (IEnumerable<string>) identityTypes)
      {
        IdentityTypeEnum result;
        if (!string.IsNullOrWhiteSpace(identityType) && Enum.TryParse<IdentityTypeEnum>(identityType.Trim().ToLower(), true, out result))
          identityTypeEnum |= result;
      }
      return identityTypeEnum != IdentityTypeEnum.None ? identityTypeEnum : throw new IdentityPickerArgumentException("No valid identity type could be parsed");
    }

    internal static ConnectionTypeEnum ParseConnectionTypes(IList<string> connectionTypes)
    {
      if (connectionTypes == null || connectionTypes.Count == 0)
        throw new IdentityPickerArgumentException("connectionTypes (required parameter) is null or empty");
      ConnectionTypeEnum connectionTypeEnum = ConnectionTypeEnum.None;
      foreach (string connectionType in (IEnumerable<string>) connectionTypes)
      {
        ConnectionTypeEnum result;
        if (!string.IsNullOrWhiteSpace(connectionType) && Enum.TryParse<ConnectionTypeEnum>(connectionType.Trim().ToLower(), true, out result))
          connectionTypeEnum |= result;
      }
      return connectionTypeEnum != ConnectionTypeEnum.None ? connectionTypeEnum : throw new IdentityPickerArgumentException("No valid connection type could be parsed");
    }

    internal static bool TryParseTokens(
      IVssRequestContext context,
      string query,
      QueryTypeHintEnum queryTypeHint,
      out IdentityOperationHelper.ParsedTokens identifiers)
    {
      string[] source;
      if (string.IsNullOrEmpty(query))
        source = Array.Empty<string>();
      else
        source = query.Split(new char[1]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
      IEnumerable<string> strings = ((IEnumerable<string>) source).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x.Trim()))).Select<string, string>((Func<string, string>) (x => x.Trim())).Distinct<string>();
      identifiers = new IdentityOperationHelper.ParsedTokens();
      bool flag = (queryTypeHint & QueryTypeHintEnum.UID) == QueryTypeHintEnum.None;
      foreach (string str in strings)
      {
        if (flag)
        {
          if (!IdentityOperationHelper.TryUpdateTokensByPrefix(context, identifiers, str))
            identifiers.Unknowns.Add(str);
        }
        else
        {
          TeamFoundationExecutionEnvironment executionEnvironment = context.ExecutionEnvironment;
          if (executionEnvironment.IsHostedDeployment)
          {
            Group group = IdentityOperationHelper.FullyQualifiedAddressRegex.Match(str).Groups["mailaddress"];
            string lower = !string.IsNullOrEmpty(group.Value) ? group.Value.Trim().ToLower() : (string) null;
            if (group.Success && IdentityOperationHelper.IsValidEmailAddress(lower))
            {
              identifiers.EmailAddresses.Add(lower);
              continue;
            }
          }
          if (DirectoryUtils.IsEntityId(str))
          {
            identifiers.EntityIds.Add(str);
          }
          else
          {
            Guid result;
            if (Guid.TryParse(str, out result))
            {
              identifiers.DirectoryIds.Add(result);
            }
            else
            {
              SubjectDescriptor subjectDescriptor = SubjectDescriptor.FromString(str);
              if (subjectDescriptor != new SubjectDescriptor() && !subjectDescriptor.IsUnknownSubjectType() && Microsoft.VisualStudio.Services.Graph.Constants.SubjectTypeMap.ContainsKey(subjectDescriptor.SubjectType))
              {
                identifiers.SubjectDescriptors.Add(subjectDescriptor);
              }
              else
              {
                executionEnvironment = context.ExecutionEnvironment;
                if (executionEnvironment.IsHostedDeployment)
                {
                  Group group = IdentityOperationHelper.MailRegex.Match(str).Groups["mailaddress"];
                  string lower = !string.IsNullOrEmpty(group.Value) ? group.Value.Trim().ToLower() : (string) null;
                  if (group.Success && IdentityOperationHelper.IsValidEmailAddress(lower))
                  {
                    identifiers.EmailAddresses.Add(lower);
                    continue;
                  }
                  if (ArgumentUtility.IsValidEmailAddress(str))
                  {
                    identifiers.EmailAddresses.Add(str);
                    continue;
                  }
                }
                else
                {
                  if (IdentityOperationHelper.DomainSamAccountNameRegex.IsMatch(str))
                  {
                    identifiers.DomainSamAccountNames.Add(str);
                    continue;
                  }
                  if (ArgumentUtility.IsValidEmailAddress(str))
                  {
                    identifiers.EmailAddresses.Add(str);
                    continue;
                  }
                }
                executionEnvironment = context.ExecutionEnvironment;
                string accountName;
                if (executionEnvironment.IsHostedDeployment && IdentityOperationHelper.TryParseAccountName(str, out accountName))
                  identifiers.AccountNames.Add(accountName);
                else if (!IdentityOperationHelper.TryUpdateTokensByPrefix(context, identifiers, str))
                  identifiers.Unknowns.Add(str);
              }
            }
          }
        }
      }
      return 1 <= identifiers.Count;
    }

    private static bool TryUpdateTokensByPrefix(
      IVssRequestContext context,
      IdentityOperationHelper.ParsedTokens identifiers,
      string identifier)
    {
      Group group = IdentityOperationHelper.PrefixRegex.Match(identifier).Groups["prefix"];
      string lower = !string.IsNullOrEmpty(group.Value) ? group.Value.Trim().ToLower() : (string) null;
      if (group.Success && !string.IsNullOrEmpty(lower))
      {
        identifiers.Prefixes.Add(lower);
        return true;
      }
      try
      {
        if (context.ExecutionEnvironment.IsHostedDeployment && ArgumentUtility.IsValidEmailAddress(identifier))
        {
          identifiers.Prefixes.Add(identifier);
          return true;
        }
        if (TFCommonUtil.IsLegalIdentity(identifier))
        {
          identifiers.Prefixes.Add(identifier);
          return true;
        }
        string groupName = identifier;
        if (TFCommonUtil.IsValidGroupName(ref groupName))
        {
          identifiers.Prefixes.Add(identifier);
          return true;
        }
      }
      catch (Exception ex)
      {
        Tracing.TraceException(context, 638, ex);
        return false;
      }
      return false;
    }

    private static bool IsValidEmailAddress(string emailaddress)
    {
      try
      {
        MailAddress mailAddress = new MailAddress(emailaddress);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private static bool TryParseAccountName(string identifier, out string accountName)
    {
      accountName = string.Empty;
      if (identifier.IsNullOrEmpty<char>())
        return false;
      string main = identifier.Trim();
      if (!VssStringComparer.AccountName.EndsWith(main, "@Live") && !VssStringComparer.AccountName.EndsWith(main, "@GitHub"))
        return false;
      accountName = main;
      return true;
    }

    internal class ParsedTokens
    {
      internal HashSet<string> Prefixes { get; private set; }

      internal HashSet<string> EmailAddresses { get; private set; }

      internal HashSet<string> DomainSamAccountNames { get; private set; }

      internal HashSet<string> EntityIds { get; private set; }

      internal HashSet<SubjectDescriptor> SubjectDescriptors { get; private set; }

      internal HashSet<Guid> DirectoryIds { get; private set; }

      internal HashSet<string> AccountNames { get; private set; }

      internal HashSet<string> Unknowns { get; private set; }

      public ParsedTokens()
      {
        this.Prefixes = new HashSet<string>();
        this.EmailAddresses = new HashSet<string>();
        this.DomainSamAccountNames = new HashSet<string>();
        this.EntityIds = new HashSet<string>();
        this.SubjectDescriptors = new HashSet<SubjectDescriptor>();
        this.DirectoryIds = new HashSet<Guid>();
        this.AccountNames = new HashSet<string>();
        this.Unknowns = new HashSet<string>();
      }

      internal HashSet<string> All => new HashSet<string>(this.Prefixes.Union<string>((IEnumerable<string>) this.EntityIds).Union<string>(this.SubjectDescriptors.Select<SubjectDescriptor, string>((Func<SubjectDescriptor, string>) (x => x.ToString()))).Union<string>(this.DirectoryIds.Select<Guid, string>((Func<Guid, string>) (x => x.ToString()))).Union<string>((IEnumerable<string>) this.EmailAddresses).Union<string>((IEnumerable<string>) this.DomainSamAccountNames).Union<string>((IEnumerable<string>) this.AccountNames).Union<string>((IEnumerable<string>) this.Unknowns));

      internal int Count => this.Prefixes.Count + this.EntityIds.Count + this.SubjectDescriptors.Count + this.DirectoryIds.Count + this.EmailAddresses.Count + this.DomainSamAccountNames.Count + this.AccountNames.Count + this.Unknowns.Count;
    }
  }
}
