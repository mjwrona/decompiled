// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class RequestRestrictionsAttribute : Attribute
  {
    private readonly Regex m_regEx;

    public RequestRestrictionsAttribute(
      RequiredAuthentication requiredAuthentication,
      bool allowNonSsl = false,
      bool allowCors = true,
      AuthenticationMechanisms mechanisms = AuthenticationMechanisms.All,
      string description = "",
      UserAgentFilterType agentFilterType = UserAgentFilterType.None,
      string agentFilter = null)
    {
      this.RequiredAuthentication = requiredAuthentication;
      this.AllowNonSsl = allowNonSsl;
      this.AllowCORS = allowCors;
      this.MechanismsToAdvertise = mechanisms;
      this.Description = description;
      this.AgentFilterType = agentFilterType;
      this.AgentFilter = agentFilter;
      if (this.AgentFilterType == UserAgentFilterType.None)
      {
        if (!string.IsNullOrEmpty(this.AgentFilter))
          throw new ArgumentException("agentFilter must be provided when agentFilterType is not None");
      }
      else
      {
        ArgumentUtility.CheckStringForNullOrEmpty(this.AgentFilter, nameof (AgentFilter));
        if (this.AgentFilterType != UserAgentFilterType.Regex)
          return;
        this.m_regEx = new Regex(this.AgentFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
      }
    }

    public RequiredAuthentication RequiredAuthentication { get; private set; }

    public bool AllowNonSsl { get; private set; }

    public bool AllowCORS { get; private set; }

    public AuthenticationMechanisms MechanismsToAdvertise { get; private set; }

    public string Description { get; private set; }

    public UserAgentFilterType AgentFilterType { get; private set; }

    public string AgentFilter { get; private set; }

    public virtual void ApplyRequestRestrictions(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      IWebRequestContextInternal requestContextInternal = requestContext.WebRequestContextInternal(false);
      if (requestContextInternal == null)
        return;
      AuthenticationMechanisms mechanismsToAdvertise = this.MechanismsToAdvertise;
      if ((mechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirectOnlyIfAcceptHtml) != AuthenticationMechanisms.None && (mechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirect) == AuthenticationMechanisms.None && requestContextInternal.HttpContext != null && requestContextInternal.HttpContext.Request.AcceptTypes != null && ((IEnumerable<string>) requestContextInternal.HttpContext.Request.AcceptTypes).Contains<string>("text/html", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        mechanismsToAdvertise |= AuthenticationMechanisms.FederatedRedirect;
      RequestRestrictions requestRestrictions = new RequestRestrictions(this.GetLabel(routeValues), this.RequiredAuthentication, RequestRestrictions.DefaultRequestRestrictions.AllowedHandlers, this.Description, this.AllowNonSsl, this.AllowCORS, mechanismsToAdvertise);
      requestContextInternal.RequestRestrictions = requestRestrictions;
    }

    public bool MatchUserAgent(string userAgent)
    {
      if (this.AgentFilter == null)
        return true;
      if (string.IsNullOrEmpty(userAgent))
        return false;
      switch (this.AgentFilterType)
      {
        case UserAgentFilterType.StartsWith:
          return userAgent.StartsWith(this.AgentFilter, StringComparison.OrdinalIgnoreCase);
        case UserAgentFilterType.Regex:
          return this.m_regEx.IsMatch(userAgent);
        default:
          throw new NotSupportedException();
      }
    }

    protected string GetLabel(IDictionary<string, object> routeValues)
    {
      if (routeValues == null || !routeValues.ContainsKey("controller") || !routeValues.ContainsKey("action"))
        return "";
      string routeValue1 = routeValues["controller"] as string;
      string routeValue2 = routeValues["action"] as string;
      return string.IsNullOrEmpty(routeValue1) || string.IsNullOrEmpty(routeValue2) ? "" : routeValue1 + "." + routeValue2;
    }

    public override string ToString() => string.Format("RequiredAuthentication : {0}, AllowNonSsl: {1}, AllowCors: {2}, MechanismsToAdvertise: {3}, Description: {4}, AgentFilterType: {5}, AgentFilter: {6}", (object) this.RequiredAuthentication, (object) this.AllowNonSsl, (object) this.AllowCORS, (object) this.MechanismsToAdvertise, (object) this.Description, (object) this.AgentFilterType, (object) this.AgentFilter);
  }
}
