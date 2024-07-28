// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.CommandQueryBuildsBase2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal abstract class CommandQueryBuildsBase2010 : BuildCommand
  {
    protected CommandQueryBuildsBase2010.State m_state;
    protected HashSet<string> m_controllerUris = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    protected IDictionary<string, BuildDefinition2010> m_allDefinitions = (IDictionary<string, BuildDefinition2010>) new Dictionary<string, BuildDefinition2010>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    protected CommandQueryBuildsBase2010(IVssRequestContext requestContext)
      : base(requestContext, Guid.Empty)
    {
    }

    protected void ReadDefinitions(
      ResultCollection dbResult,
      List<BuildDefinition2010> definitions,
      Func<BuildDefinition2010, bool> includeDefinition,
      QueryOptions2010 queryOptions)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ReadDefinitions));
      bool flag = (queryOptions & QueryOptions2010.Definitions) == QueryOptions2010.Definitions;
      ObjectBinder<BuildDefinition2010> current1 = dbResult.GetCurrent<BuildDefinition2010>();
      while (current1.MoveNext())
      {
        BuildDefinition2010 current2 = current1.Current;
        if (this.HasBuildPermission(this.RequestContext, current2.Uri, current2.SecurityToken, BuildPermissions.ViewBuildDefinition) && flag && includeDefinition(current2))
        {
          definitions.Add(current2);
          this.m_controllerUris.Add(current2.BuildControllerUri);
          this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read definition '{0}'", (object) current2.Uri);
        }
        this.m_allDefinitions[current2.Uri] = current2;
      }
      if (flag)
      {
        BuildDefinition2010.ReadDatabaseResults((IEnumerable<BuildDefinition2010>) definitions, dbResult);
        BuildDefinition2010.ConvertDataspacedValues(dbResult.RequestContext, (IEnumerable<BuildDefinition2010>) definitions);
      }
      else
      {
        this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Skipped query results as definitions not to be included");
        dbResult.NextResult();
        dbResult.NextResult();
        dbResult.NextResult();
        dbResult.NextResult();
        dbResult.NextResult();
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ReadDefinitions));
    }

    protected void ReadBuildServices(
      ResultCollection dbResult,
      QueryOptions2010 options,
      List<BuildController2010> controllers,
      List<BuildAgent2010> agents,
      List<BuildServiceHost2010> serviceHosts)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ReadBuildServices));
      if ((options & QueryOptions2010.Controllers | QueryOptions2010.Agents) == QueryOptions2010.None || !this.BuildHost.SecurityManager.HasPrivilege(this.RequestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Exiting no build resources");
      }
      else
      {
        Dictionary<string, BuildServiceHost2010> dictionary = new Dictionary<string, BuildServiceHost2010>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if ((options & QueryOptions2010.Controllers) == QueryOptions2010.Controllers)
        {
          ObjectBinder<BuildController2010> current = dbResult.GetCurrent<BuildController2010>();
          while (current.MoveNext())
          {
            if (this.m_controllerUris.Contains(current.Current.Uri))
            {
              controllers.Add(current.Current);
              dictionary[current.Current.ServiceHostUri] = (BuildServiceHost2010) null;
              this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read controller '{0}'", (object) current.Current.Uri);
            }
          }
        }
        dbResult.NextResult();
        if ((options & QueryOptions2010.Agents) == QueryOptions2010.Agents)
        {
          ObjectBinder<BuildAgent2010> current = dbResult.GetCurrent<BuildAgent2010>();
          while (current.MoveNext())
          {
            if (this.m_controllerUris.Contains(current.Current.ControllerUri))
            {
              agents.Add(current.Current);
              dictionary[current.Current.ServiceHostUri] = (BuildServiceHost2010) null;
              this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read agent '{0}'", (object) current.Current.Uri);
            }
          }
        }
        dbResult.NextResult();
        ObjectBinder<BuildServiceHost2010> current1 = dbResult.GetCurrent<BuildServiceHost2010>();
        while (current1.MoveNext())
        {
          if (dictionary.ContainsKey(current1.Current.Uri))
          {
            serviceHosts.Add(current1.Current);
            dictionary[current1.Current.Uri] = current1.Current;
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read service host '{0}'", (object) current1.Current.Uri);
          }
        }
        this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ReadBuildServices));
      }
    }

    protected enum State
    {
      BuildDefinition,
      BuildDetail,
      BuildInformation,
      BuildServices,
    }
  }
}
