// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryMacroExtensionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryMacroExtensionService : IVssFrameworkService
  {
    private IDisposableReadOnlyList<QueryMacroExtension> m_macroExtensions;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.InitializeExtensions(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.DisposeExtensions();

    public virtual object GetValue(
      IVssRequestContext requestContext,
      string macroName,
      ProjectInfo project = null,
      WebApiTeam team = null,
      NodeParameters parameters = null,
      TimeZone timeZone = null,
      CultureInfo cultureInfo = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(macroName, nameof (macroName));
      IEnumerable<QueryMacroExtension> matchedExtensions = this.GetMatchedExtensions(macroName);
      object obj = matchedExtensions == null || !matchedExtensions.Any<QueryMacroExtension>() ? (object) null : matchedExtensions.First<QueryMacroExtension>().GetValue(requestContext, macroName, project, team, parameters, timeZone, cultureInfo);
      requestContext.Trace(909501, TraceLevel.Verbose, "Query", nameof (QueryMacroExtensionService), "Macro: {0}, Value: {1}", (object) macroName, obj);
      return obj;
    }

    public virtual DataType GetDataType(string macroName)
    {
      IEnumerable<QueryMacroExtension> matchedExtensions = this.GetMatchedExtensions(macroName);
      return matchedExtensions == null || !matchedExtensions.Any<QueryMacroExtension>() ? DataType.Unknown : matchedExtensions.First<QueryMacroExtension>().DataType;
    }

    internal bool DoesMacroExtensionHandleOffset(string macroName)
    {
      IEnumerable<QueryMacroExtension> matchedExtensions = this.GetMatchedExtensions(macroName);
      return matchedExtensions != null && matchedExtensions.Any<QueryMacroExtension>() && matchedExtensions.First<QueryMacroExtension>().DoesMacroExtensionHandleOffset;
    }

    public bool RewriteCondition(
      IVssRequestContext requestContext,
      NodeCondition condition,
      out Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node rewritten)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) null;
      int num = this.m_macroExtensions == null ? 0 : (this.m_macroExtensions.Any<QueryMacroExtension>((Func<QueryMacroExtension, bool>) (e => e.Rewrite(requestContext, condition, out node))) ? 1 : 0);
      rewritten = node;
      return num != 0;
    }

    public virtual bool IsSupportedMacro(
      IVssRequestContext requestContext,
      string macroName,
      out object defaultValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(macroName, nameof (macroName));
      defaultValue = (object) null;
      IEnumerable<QueryMacroExtension> matchedExtensions = this.GetMatchedExtensions(macroName);
      if (matchedExtensions == null || !matchedExtensions.Any<QueryMacroExtension>())
        return false;
      defaultValue = matchedExtensions.First<QueryMacroExtension>().DefaultValue;
      return true;
    }

    private IEnumerable<QueryMacroExtension> GetMatchedExtensions(string macroName)
    {
      IEnumerable<QueryMacroExtension> matchedExtensions = (IEnumerable<QueryMacroExtension>) null;
      if (this.m_macroExtensions != null && this.m_macroExtensions.Any<QueryMacroExtension>())
        matchedExtensions = this.m_macroExtensions.Where<QueryMacroExtension>((Func<QueryMacroExtension, bool>) (e => e.IsMatch(macroName)));
      return matchedExtensions;
    }

    private void InitializeExtensions(IVssRequestContext requestContext) => this.m_macroExtensions = requestContext.GetExtensions<QueryMacroExtension>();

    private void DisposeExtensions()
    {
      if (this.m_macroExtensions == null)
        return;
      this.m_macroExtensions.Dispose();
      this.m_macroExtensions = (IDisposableReadOnlyList<QueryMacroExtension>) null;
    }

    internal virtual void ValidateParameters(
      IVssRequestContext requestContext,
      string macroName,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters)
    {
      IEnumerable<QueryMacroExtension> matchedExtensions = this.GetMatchedExtensions(macroName);
      if (matchedExtensions != null && matchedExtensions.Any<QueryMacroExtension>())
        matchedExtensions.First<QueryMacroExtension>().Validate(requestContext, tableContext, fieldContext, parameters);
      else
        QueryMacroExtension.DefaultValidate(macroName, tableContext, fieldContext, parameters);
    }
  }
}
