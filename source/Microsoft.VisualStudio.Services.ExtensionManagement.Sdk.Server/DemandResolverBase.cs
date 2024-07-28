// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.DemandResolverBase
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal abstract class DemandResolverBase : IDemandResolver
  {
    public abstract IEnumerable<string> DemandTypes { get; }

    public virtual bool CanAnswerToResolutionType(
      string demandType,
      DemandsResolutionType resolutionType)
    {
      return true;
    }

    public DemandResult ResolveDemand(
      IVssRequestContext requestContext,
      Demand demandIdentifer,
      DemandsResolutionType resolutionType)
    {
      ArgumentUtility.CheckForNull<Demand>(demandIdentifer, nameof (demandIdentifer));
      if (!this.DemandTypes.Contains<string>(demandIdentifer.DemandType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new ArgumentException(ExtMgmtResources.DemandInvalidDemandResolverErrorFormat((object) this.GetType().Name, (object) demandIdentifer.DemandType));
      DemandResult demandResult = new DemandResult()
      {
        Demand = demandIdentifer
      };
      string errorMessage;
      if (this.IsDemandSupported(requestContext, demandIdentifer, resolutionType, out errorMessage))
      {
        demandResult.Success = true;
      }
      else
      {
        demandResult.Success = false;
        demandResult.ErrorMessage = errorMessage;
      }
      return demandResult;
    }

    public IEnumerable<DemandResult> ResolveDemands(
      IVssRequestContext requestContext,
      IEnumerable<Demand> demands,
      DemandsResolutionType resolutionType)
    {
      if (demands == null)
        return (IEnumerable<DemandResult>) null;
      List<DemandResult> demandResultList = new List<DemandResult>(demands.Count<Demand>());
      foreach (Demand demand in demands)
        demandResultList.Add(this.ResolveDemand(requestContext, demand, resolutionType));
      return (IEnumerable<DemandResult>) demandResultList;
    }

    protected abstract bool IsDemandSupported(
      IVssRequestContext requestContext,
      Demand demandIdentifer,
      DemandsResolutionType resolutionType,
      out string errorMessage);
  }
}
