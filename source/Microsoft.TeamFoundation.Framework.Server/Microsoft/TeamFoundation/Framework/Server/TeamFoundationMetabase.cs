// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationMetabase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationMetabase
  {
    private SparseTree<TeamFoundationMetabase.MetabasePathEntry> m_hierarchy = new SparseTree<TeamFoundationMetabase.MetabasePathEntry>('/', StringComparison.OrdinalIgnoreCase);
    private List<TeamFoundationMetabase.UserAgentRestriction> m_userAgentRestrictions = new List<TeamFoundationMetabase.UserAgentRestriction>();
    private List<TeamFoundationMetabase.ApplicationRelativePathContainsRestriction> m_applicationRelativePathContainsRestrictions = new List<TeamFoundationMetabase.ApplicationRelativePathContainsRestriction>();
    private static readonly IReadOnlyList<RegistryQuery> s_sharedQueries = (IReadOnlyList<RegistryQuery>) new RegistryQuery[1]
    {
      (RegistryQuery) (MetabaseRegistryConstants.Shared + "/**")
    };
    private static readonly IReadOnlyList<RegistryQuery> s_deploymentQueries = (IReadOnlyList<RegistryQuery>) new RegistryQuery[2]
    {
      (RegistryQuery) (MetabaseRegistryConstants.Shared + "/**"),
      (RegistryQuery) (MetabaseRegistryConstants.Deployment + "/**")
    };
    public static readonly IReadOnlyList<RegistryQuery> Queries = (IReadOnlyList<RegistryQuery>) TeamFoundationMetabase.s_sharedQueries.Concat<RegistryQuery>((IEnumerable<RegistryQuery>) TeamFoundationMetabase.s_deploymentQueries).Distinct<RegistryQuery>(RegistryQueryComparer.Instance).ToArray<RegistryQuery>();
    private static readonly char[] s_singleNameValueDelimiter = new char[1]
    {
      '='
    };
    private static readonly char[] s_singleSettingDelimiter = new char[1]
    {
      ';'
    };
    private static readonly string[] s_emptyLabels = Array.Empty<string>();
    private const string c_area = "TeamFoundationMetabase";
    private const string c_layer = "HostManagement";

    public TeamFoundationMetabase(IVssRequestContext requestContext, bool isDeploymentMetabase)
    {
      requestContext.CheckDeploymentRequestContext();
      ISqlRegistryService service = requestContext.GetService<ISqlRegistryService>();
      foreach (RegistryItem registryItem in !isDeploymentMetabase ? service.Read(requestContext, (IEnumerable<RegistryQuery>) TeamFoundationMetabase.s_sharedQueries).SelectMany<IEnumerable<RegistryItem>, RegistryItem>((Func<IEnumerable<RegistryItem>, IEnumerable<RegistryItem>>) (s => s)) : service.Read(requestContext, (IEnumerable<RegistryQuery>) TeamFoundationMetabase.s_deploymentQueries).SelectMany<IEnumerable<RegistryItem>, RegistryItem>((Func<IEnumerable<RegistryItem>, IEnumerable<RegistryItem>>) (s => s)))
      {
        string keyName = RegistryUtility.GetKeyName(registryItem.Path);
        string[] strArray1 = registryItem.Value.Split(TeamFoundationMetabase.s_singleSettingDelimiter, StringSplitOptions.RemoveEmptyEntries);
        string empty1 = string.Empty;
        string empty2 = string.Empty;
        string pattern = (string) null;
        string str1 = (string) null;
        string description = keyName;
        RequiredAuthentication requiredAuthentication = RequiredAuthentication.Undefined;
        AllowedHandler allowedHandlers = AllowedHandler.None;
        AuthenticationMechanisms mechanismsToAdvertise = AuthenticationMechanisms.All;
        bool result1 = false;
        bool result2 = true;
        bool result3 = false;
        bool result4 = false;
        foreach (string str2 in strArray1)
        {
          string[] strArray2 = str2.Split(TeamFoundationMetabase.s_singleNameValueDelimiter, StringSplitOptions.RemoveEmptyEntries);
          if (strArray2.Length != 2)
            requestContext.Trace(0, TraceLevel.Error, nameof (TeamFoundationMetabase), "HostManagement", "Registry {0} has bad value {1}.", (object) registryItem.Path, (object) str2);
          else if (strArray2[0].Equals(MetabaseRegistryConstants.ApplicationRelativePathValue, StringComparison.OrdinalIgnoreCase))
            empty1 = strArray2[1];
          else if (strArray2[0].Equals(MetabaseRegistryConstants.ApplicationRelativePathContainsValue, StringComparison.OrdinalIgnoreCase))
            empty2 = strArray2[1];
          else if (strArray2[0].Equals(MetabaseRegistryConstants.UserAgentMatchValue, StringComparison.OrdinalIgnoreCase))
            pattern = strArray2[1];
          else if (strArray2[0].Equals(MetabaseRegistryConstants.UserAgentStartsWithFilter, StringComparison.OrdinalIgnoreCase))
            str1 = strArray2[1];
          else if (strArray2[0].Equals(MetabaseRegistryConstants.RequiredAuthenticationValue, StringComparison.OrdinalIgnoreCase))
          {
            int result5;
            int.TryParse(strArray2[1], out result5);
            requiredAuthentication = (RequiredAuthentication) result5;
          }
          else if (strArray2[0].Equals(MetabaseRegistryConstants.AllowedHandlersValue, StringComparison.OrdinalIgnoreCase))
          {
            int result6;
            int.TryParse(strArray2[1], out result6);
            allowedHandlers = (AllowedHandler) result6;
          }
          else if (strArray2[0].Equals(MetabaseRegistryConstants.AuthenticationMechanismsValue, StringComparison.OrdinalIgnoreCase))
          {
            int result7;
            int.TryParse(strArray2[1], out result7);
            mechanismsToAdvertise = (AuthenticationMechanisms) result7;
          }
          else if (strArray2[0].Equals(MetabaseRegistryConstants.AllowNonSslValue, StringComparison.OrdinalIgnoreCase))
            bool.TryParse(strArray2[1], out result1);
          else if (strArray2[0].Equals(MetabaseRegistryConstants.AllowCORS, StringComparison.OrdinalIgnoreCase))
            bool.TryParse(strArray2[1], out result2);
          else if (strArray2[0].Equals(MetabaseRegistryConstants.ExactPathMatchOnlyValue, StringComparison.OrdinalIgnoreCase))
            bool.TryParse(strArray2[1], out result3);
          else if (strArray2[0].Equals(MetabaseRegistryConstants.HostedOnlyValue, StringComparison.OrdinalIgnoreCase))
            bool.TryParse(strArray2[1], out result4);
          else
            requestContext.Trace(0, TraceLevel.Error, nameof (TeamFoundationMetabase), "HostManagement", "Registry {0} has unknown value {1}.", (object) registryItem.Path, (object) str2);
        }
        bool flag1 = !string.IsNullOrWhiteSpace(empty1);
        bool flag2 = !string.IsNullOrWhiteSpace(empty2);
        bool flag3 = !string.IsNullOrWhiteSpace(pattern) || !string.IsNullOrWhiteSpace(str1);
        if (!flag1 && !flag2 && !flag3 || flag1 & flag2 & flag3 || requiredAuthentication == RequiredAuthentication.Undefined || allowedHandlers == AllowedHandler.None)
          requestContext.Trace(0, TraceLevel.Error, nameof (TeamFoundationMetabase), "HostManagement", "Cannot set request restriction {0} with applicationRelativePath={1} userAgentMatch={2} userAgentStartsWith={3} requiredAuthentication={4} allowedHandlers={5} applicationRelativePathContains={6}.", (object) description, (object) empty1, (object) pattern, (object) str1, (object) requiredAuthentication, (object) allowedHandlers, (object) empty2);
        else if (!result4 || requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          RequestRestrictions restrictions = new RequestRestrictions(keyName, requiredAuthentication, allowedHandlers, description, result1, result2, mechanismsToAdvertise);
          if (flag1)
          {
            TeamFoundationMetabase.MetabasePathEntry referencedObject;
            if (!this.m_hierarchy.TryGetValue(empty1, out referencedObject))
              referencedObject = new TeamFoundationMetabase.MetabasePathEntry();
            referencedObject = !result3 ? referencedObject.WithRecursiveMatch(restrictions) : referencedObject.WithExactMatch(restrictions);
            this.m_hierarchy[empty1] = referencedObject;
            requestContext.Trace(0, TraceLevel.Verbose, nameof (TeamFoundationMetabase), "HostManagement", "Set request restriction {0} on path {1}.", (object) restrictions, (object) empty1);
          }
          else if (flag2)
          {
            this.m_applicationRelativePathContainsRestrictions.Add(new TeamFoundationMetabase.ApplicationRelativePathContainsRestriction()
            {
              SubstringToSearchFor = empty2,
              Restrictions = restrictions
            });
            requestContext.Trace(0, TraceLevel.Verbose, nameof (TeamFoundationMetabase), "HostManagement", "Set request restriction {0} on path (contains) {1}.", (object) restrictions, (object) empty2);
          }
          else
          {
            TeamFoundationMetabase.UserAgentRestriction agentRestriction = new TeamFoundationMetabase.UserAgentRestriction();
            if (!string.IsNullOrEmpty(pattern))
              agentRestriction.AgentRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (!string.IsNullOrEmpty(str1))
              agentRestriction.StartsWithFilter = str1;
            agentRestriction.Restrictions = restrictions;
            this.m_userAgentRestrictions.Add(agentRestriction);
            requestContext.Trace(0, TraceLevel.Verbose, nameof (TeamFoundationMetabase), "HostManagement", "Set request restriction {0} on user agent {1}.", (object) restrictions, (object) pattern);
          }
        }
      }
    }

    public RequestRestrictions GetRequestRestrictions(
      string applicationRelativePath,
      string userAgent,
      IEnumerable<MediaTypeWithQualityHeaderValue> acceptableMediaTypes)
    {
      RequestRestrictions restrictions2_1 = (RequestRestrictions) null;
      if (!string.IsNullOrEmpty(userAgent))
      {
        foreach (TeamFoundationMetabase.UserAgentRestriction agentRestriction in this.m_userAgentRestrictions)
        {
          if (agentRestriction.AgentRegex != null && agentRestriction.AgentRegex.IsMatch(userAgent) || agentRestriction.StartsWithFilter != null && userAgent.StartsWith(agentRestriction.StartsWithFilter, StringComparison.OrdinalIgnoreCase))
            restrictions2_1 = this.MergeRestrictions(agentRestriction.Restrictions, restrictions2_1);
        }
      }
      RequestRestrictions restrictions2_2 = (RequestRestrictions) null;
      foreach (TeamFoundationMetabase.ApplicationRelativePathContainsRestriction containsRestriction in this.m_applicationRelativePathContainsRestrictions)
      {
        if (applicationRelativePath.IndexOf(containsRestriction.SubstringToSearchFor, StringComparison.OrdinalIgnoreCase) >= 0)
          restrictions2_2 = this.MergeRestrictions(containsRestriction.Restrictions, restrictions2_2);
      }
      RequestRestrictions restrictions1 = (RequestRestrictions) null;
      foreach (EnumeratedSparseTreeNode<TeamFoundationMetabase.MetabasePathEntry> enumParent in this.m_hierarchy.EnumParents(applicationRelativePath, EnumParentsOptions.None))
      {
        if (enumParent.ReferencedObject.ExactMatch != null && enumParent.IsExactMatch)
        {
          restrictions1 = enumParent.ReferencedObject.ExactMatch;
          break;
        }
        if (enumParent.ReferencedObject.RecursiveMatch != null)
        {
          restrictions1 = enumParent.ReferencedObject.RecursiveMatch;
          break;
        }
      }
      RequestRestrictions requestRestrictions = this.MergeRestrictions(this.MergeRestrictions(restrictions1, restrictions2_1), restrictions2_2) ?? RequestRestrictions.DefaultRequestRestrictions;
      if ((requestRestrictions.MechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirectOnlyIfAcceptHtml) != AuthenticationMechanisms.None && (requestRestrictions.MechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirect) == AuthenticationMechanisms.None && acceptableMediaTypes.Any<MediaTypeWithQualityHeaderValue>((Func<MediaTypeWithQualityHeaderValue, bool>) (x => x.MediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase))))
        requestRestrictions = requestRestrictions.WithMechanismsToAdvertise(requestRestrictions.MechanismsToAdvertise | AuthenticationMechanisms.FederatedRedirect);
      return requestRestrictions;
    }

    private RequestRestrictions MergeRestrictions(
      RequestRestrictions restrictions1,
      RequestRestrictions restrictions2)
    {
      if (restrictions1 == null)
        return restrictions2;
      if (restrictions2 == null)
        return restrictions1;
      RequiredAuthentication requiredAuthentication = restrictions1.RequiredAuthentication <= restrictions2.RequiredAuthentication ? restrictions2.RequiredAuthentication : restrictions1.RequiredAuthentication;
      StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
      string[] singleLabelDelimiter = RequestRestrictions.s_SingleLabelDelimiter;
      string[] first = string.IsNullOrWhiteSpace(restrictions1.Label) ? TeamFoundationMetabase.s_emptyLabels : restrictions1.Label.Split(singleLabelDelimiter, StringSplitOptions.RemoveEmptyEntries);
      string[] second = string.IsNullOrWhiteSpace(restrictions2.Label) ? TeamFoundationMetabase.s_emptyLabels : restrictions2.Label.Split(singleLabelDelimiter, StringSplitOptions.RemoveEmptyEntries);
      string label1 = string.Join(singleLabelDelimiter[0], (IEnumerable<string>) ((IEnumerable<string>) first).Union<string>((IEnumerable<string>) second, (IEqualityComparer<string>) ordinalIgnoreCase).OrderBy<string, string>((Func<string, string>) (label => label), (IComparer<string>) ordinalIgnoreCase));
      AllowedHandler allowedHandler = restrictions1.AllowedHandlers & restrictions2.AllowedHandlers;
      bool flag1 = restrictions1.AllowNonSsl && restrictions2.AllowNonSsl;
      bool flag2 = restrictions1.AllowCORS && restrictions2.AllowCORS;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1}", (object) restrictions1.Description, (object) restrictions2.Description);
      AuthenticationMechanisms authenticationMechanisms = restrictions1.MechanismsToAdvertise & restrictions2.MechanismsToAdvertise;
      int num1 = (int) requiredAuthentication;
      int allowedHandlers = (int) allowedHandler;
      string description = str;
      int num2 = flag1 ? 1 : 0;
      int num3 = flag2 ? 1 : 0;
      int mechanismsToAdvertise = (int) authenticationMechanisms;
      return new RequestRestrictions(label1, (RequiredAuthentication) num1, (AllowedHandler) allowedHandlers, description, num2 != 0, num3 != 0, (AuthenticationMechanisms) mechanismsToAdvertise);
    }

    private struct MetabasePathEntry
    {
      public RequestRestrictions ExactMatch;
      public RequestRestrictions RecursiveMatch;

      public TeamFoundationMetabase.MetabasePathEntry WithExactMatch(
        RequestRestrictions restrictions)
      {
        return new TeamFoundationMetabase.MetabasePathEntry()
        {
          ExactMatch = restrictions,
          RecursiveMatch = this.RecursiveMatch
        };
      }

      public TeamFoundationMetabase.MetabasePathEntry WithRecursiveMatch(
        RequestRestrictions restrictions)
      {
        return new TeamFoundationMetabase.MetabasePathEntry()
        {
          ExactMatch = this.ExactMatch,
          RecursiveMatch = restrictions
        };
      }
    }

    private class UserAgentRestriction
    {
      public Regex AgentRegex;
      public string StartsWithFilter;
      public RequestRestrictions Restrictions;
    }

    private class ApplicationRelativePathContainsRestriction
    {
      public string SubstringToSearchFor;
      public RequestRestrictions Restrictions;
    }
  }
}
