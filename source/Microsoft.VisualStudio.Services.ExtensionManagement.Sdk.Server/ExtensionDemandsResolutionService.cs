// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionDemandsResolutionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class ExtensionDemandsResolutionService : 
    IExtensionDemandsResolutionService,
    IVssFrameworkService
  {
    private const string c_area = "DemandsResolutionService";
    private const string c_layer = "Contributions";
    private Dictionary<string, IList<IDemandResolver>> m_demandResolvers;

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.TraceBlock(10013500, 10013501, 10013504, "DemandsResolutionService", "Contributions", (Action) (() =>
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.LoadDemandResolvers(systemRequestContext);
    }), nameof (ServiceStart));

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ExtensionDemandsResolutionResult ResolveDemands(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      ExtensionManifest extensionManifest,
      DemandsResolutionType resolutionType)
    {
      return requestContext.TraceBlock<ExtensionDemandsResolutionResult>(10013500, 10013501, 10013504, "DemandsResolutionService", "Contributions", (Func<ExtensionDemandsResolutionResult>) (() =>
      {
        ArgumentUtility.CheckForNull<ExtensionManifest>(extensionManifest, nameof (extensionManifest));
        IEnumerable<string> demands = (IEnumerable<string>) null;
        ExtensionDemandsResolutionResult resolutionResult = new ExtensionDemandsResolutionResult()
        {
          ExtensionIdentifier = new ExtensionIdentifier(publisherName, extensionName)
        };
        if (extensionManifest.Demands == null || extensionManifest.Demands.Count == 0)
        {
          requestContext.TraceConditionally(10013502, TraceLevel.Info, "DemandsResolutionService", "Contributions", (Func<string>) (() => "No explicit 'demands' to process in extension '" + publisherName + "." + extensionName + "'."));
        }
        else
        {
          demands = extensionManifest.Demands.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          List<DemandIssue> demandIssueList = this.ResolveDemandsInternal(requestContext, publisherName, extensionName, resolutionType, demands);
          if (demandIssueList != null && demandIssueList.Count > 0)
          {
            resolutionResult.Status = DemandsResolutionStatus.Error;
            resolutionResult.DemandIssues = demandIssueList;
          }
        }
        if (resolutionResult.Status != DemandsResolutionStatus.Error && requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.Sdk.CheckImplicitDemands") && resolutionType == DemandsResolutionType.Installing)
        {
          HashSet<string> implicitDemands = ExtensionDemandsResolutionService.GetImplicitDemands(publisherName, extensionName, extensionManifest.Contributions);
          if (demands != null)
            implicitDemands.RemoveWhere((Predicate<string>) (d => extensionManifest.Demands.Contains<string>(d, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
          List<DemandIssue> demandIssueList = this.ResolveDemandsInternal(requestContext, publisherName, extensionName, resolutionType, (IEnumerable<string>) implicitDemands, DemandsResolutionStatus.Warning);
          if (demandIssueList != null && demandIssueList.Count > 0)
          {
            resolutionResult.Status = DemandsResolutionStatus.Warning;
            resolutionResult.DemandIssues = demandIssueList;
          }
        }
        if (resolutionResult.Status == DemandsResolutionStatus.None)
          resolutionResult.Status = DemandsResolutionStatus.Success;
        return resolutionResult;
      }), nameof (ResolveDemands));
    }

    public bool TryResolveTargetsWithDemands(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      ExtensionManifest extensionManifest,
      out List<InstallationTarget> installationTargets)
    {
      bool flag = false;
      installationTargets = (List<InstallationTarget>) null;
      if (extensionManifest.Demands != null && extensionManifest.Demands.Count > 0)
      {
        installationTargets = new List<InstallationTarget>((IEnumerable<InstallationTarget>) publishedExtension.InstallationTargets);
        IEnumerable<Contribution> demandTargets = requestContext.GetService<IContributionService>().QueryContributionsForType(requestContext, "ms.VstsProducts.demand-target");
        foreach (string demand in extensionManifest.Demands)
        {
          JObject propertiesForDemand = this.GetDemandTargetPropertiesForDemand(demandTargets, demand);
          if (propertiesForDemand != null)
          {
            JArray jarray = propertiesForDemand[nameof (installationTargets)] as JArray;
            Dictionary<string, InstallationTarget> dictionary = new Dictionary<string, InstallationTarget>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            if (jarray != null && jarray.Count > 0)
            {
              foreach (JToken jtoken in jarray)
              {
                string key = jtoken.Value<string>((object) "id");
                string version = jtoken.Value<string>((object) "minVersion");
                dictionary[key] = new InstallationTarget()
                {
                  Target = key,
                  MinInclusive = !string.IsNullOrEmpty(version),
                  MinVersion = !string.IsNullOrEmpty(version) ? new Version(version) : (Version) null
                };
              }
              for (int index = installationTargets.Count - 1; index >= 0; --index)
              {
                InstallationTarget installationTarget1 = installationTargets[index];
                if (!dictionary.ContainsKey(installationTarget1.Target))
                {
                  installationTargets.RemoveAt(index);
                  flag = true;
                }
                else
                {
                  InstallationTarget installationTarget2 = dictionary[installationTarget1.Target];
                  if (installationTarget2.MinVersion != (Version) null)
                  {
                    if (installationTarget1.MaxVersion != (Version) null && installationTarget2.MinVersion.CompareTo(installationTarget1.MaxVersion) > 0)
                    {
                      installationTargets.RemoveAt(index);
                      flag = true;
                    }
                    else if (installationTarget1.MinVersion == (Version) null || installationTarget2.MinVersion.CompareTo(installationTarget1.MinVersion) > 0)
                    {
                      installationTarget1.MinVersion = installationTarget2.MinVersion;
                      installationTarget1.MinInclusive = true;
                      flag = true;
                    }
                  }
                }
              }
            }
          }
        }
      }
      return flag;
    }

    private JObject GetDemandTargetPropertiesForDemand(
      IEnumerable<Contribution> demandTargets,
      string demandString)
    {
      foreach (Contribution demandTarget in demandTargets)
      {
        JObject properties = demandTarget.Properties;
        if (properties != null && string.Equals(properties.Value<string>((object) "demand"), demandString, StringComparison.OrdinalIgnoreCase))
          return properties;
      }
      return (JObject) null;
    }

    private static HashSet<string> GetImplicitDemands(
      string publisherName,
      string extensionName,
      IEnumerable<Contribution> contributions)
    {
      HashSet<string> implicitDemands = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (contributions != null)
      {
        foreach (Contribution contribution in contributions)
        {
          ExtensionDemandsResolutionService.CollectImplicitDemands(implicitDemands, publisherName, extensionName, contribution.Targets);
          ExtensionDemandsResolutionService.CollectImplicitDemands(implicitDemands, publisherName, extensionName, contribution.Includes);
        }
      }
      return implicitDemands;
    }

    private static void CollectImplicitDemands(
      HashSet<string> implicitDemands,
      string publisherName,
      string extensionName,
      List<string> identifiers)
    {
      if (identifiers == null)
        return;
      foreach (string identifier in identifiers)
      {
        ContributionIdentifier contributionIdentifier;
        if (ContributionIdentifier.TryParse(identifier, out contributionIdentifier) && !ExtensionDemandsResolutionService.IsLocalContribution(contributionIdentifier, publisherName, extensionName))
          implicitDemands.Add("contribution/" + identifier);
      }
    }

    private static bool IsLocalContribution(
      ContributionIdentifier contributionIdentifier,
      string publisherName,
      string extensionName)
    {
      return publisherName.Equals(contributionIdentifier.PublisherName, StringComparison.OrdinalIgnoreCase) && extensionName.Equals(contributionIdentifier.ExtensionName, StringComparison.OrdinalIgnoreCase);
    }

    private List<DemandIssue> ResolveDemandsInternal(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      DemandsResolutionType resolutionType,
      IEnumerable<string> demands,
      DemandsResolutionStatus failResolutionStatus = DemandsResolutionStatus.Error)
    {
      List<DemandIssue> demandIssues = (List<DemandIssue>) null;
      DemandIssueType issueType = failResolutionStatus == DemandsResolutionStatus.Warning ? DemandIssueType.Warning : DemandIssueType.Error;
      foreach (string demand1 in demands)
      {
        string demandId = demand1;
        requestContext.TraceConditionally(10013502, TraceLevel.Info, "DemandsResolutionService", "Contributions", (Func<string>) (() => string.Format("Evaluating demand '{0}' in extension '{1}.{2}' with status {3}.", (object) demandId, (object) publisherName, (object) extensionName, (object) failResolutionStatus)));
        if (string.IsNullOrWhiteSpace(demandId))
        {
          requestContext.TraceConditionally(10013502, TraceLevel.Info, "DemandsResolutionService", "Contributions", closure_2 ?? (closure_2 = (Func<string>) (() => "Ignoring empty demand in extension '" + publisherName + "." + extensionName + "'.")));
        }
        else
        {
          Demand demand;
          if (!Demand.TryParse(demandId, out demand))
          {
            requestContext.TraceConditionally(10013503, TraceLevel.Warning, "DemandsResolutionService", "Contributions", (Func<string>) (() => "Non wellformed demand '" + demandId + "' in extension '" + publisherName + "." + extensionName + "'. Leave with Error."));
            demandIssues = ExtensionDemandsResolutionService.AddDemandIssue(demandIssues, demandId, ExtMgmtResources.DemandDefaultErrorFormat((object) demandId), issueType);
          }
          else
          {
            IList<IDemandResolver> demandResolvers = this.GetDemandResolvers(requestContext, demand.DemandType, resolutionType);
            if (demandResolvers == null || demandResolvers.Count == 0)
            {
              requestContext.TraceConditionally(10013503, TraceLevel.Warning, "DemandsResolutionService", "Contributions", (Func<string>) (() => string.Format("Unknown demand  type '{0}' in extension '{1}.{2}'. Leave with {3}.", (object) demandId, (object) publisherName, (object) extensionName, (object) failResolutionStatus)));
              demandIssues = ExtensionDemandsResolutionService.AddDemandIssue(demandIssues, demandId, ExtMgmtResources.DemandDefaultErrorFormat((object) demandId), issueType);
            }
            else
            {
              foreach (IDemandResolver demandResolver in (IEnumerable<IDemandResolver>) demandResolvers)
              {
                IDemandResolver resolver = demandResolver;
                requestContext.TraceBlock(10013500, 10013501, 10013504, "DemandsResolutionService", "Contributions", (Action) (() =>
                {
                  try
                  {
                    DemandResult demandResult = resolver.ResolveDemand(requestContext, demand, resolutionType);
                    if (demandResult == null || demandResult.Success)
                      return;
                    requestContext.TraceConditionally(10013503, TraceLevel.Warning, "DemandsResolutionService", "Contributions", (Func<string>) (() => "Demand '" + demandId + "' could not be resolved '" + resolver.GetType().Name + "' '" + demandId + "' in extension '" + publisherName + "." + extensionName + "'. Assuming an Error."));
                    demandIssues = ExtensionDemandsResolutionService.AddDemandIssue(demandIssues, demandResult.Demand.Id, demandResult.ErrorMessage, issueType);
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(10013504, "DemandsResolutionService", "Contributions", ex);
                    demandIssues = ExtensionDemandsResolutionService.AddDemandIssue(demandIssues, demand.Id, ExtMgmtResources.DemandResolutionErrorFormat((object) demandId, (object) requestContext.ActivityId.ToString()), issueType);
                  }
                }), resolver.GetType().Name + ".ResolveDemand");
              }
            }
          }
        }
      }
      return demandIssues;
    }

    private static List<DemandIssue> AddDemandIssue(
      List<DemandIssue> demandIssues,
      string demandId,
      string errorMessage,
      DemandIssueType issueType)
    {
      if (demandIssues == null)
        demandIssues = new List<DemandIssue>(1);
      demandIssues.Add(new DemandIssue()
      {
        Demand = demandId,
        Message = errorMessage,
        Type = issueType
      });
      return demandIssues;
    }

    private void LoadDemandResolvers(IVssRequestContext systemRequestContext)
    {
      this.m_demandResolvers = new Dictionary<string, IList<IDemandResolver>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IDemandResolver extension in (IEnumerable<IDemandResolver>) systemRequestContext.GetExtensions<IDemandResolver>(ExtensionLifetime.Service))
      {
        foreach (string demandType in extension.DemandTypes)
          this.m_demandResolvers.GetOrAddValue<string, IList<IDemandResolver>>(demandType, (Func<IList<IDemandResolver>>) (() => (IList<IDemandResolver>) new List<IDemandResolver>(1))).Add(extension);
      }
    }

    private IList<IDemandResolver> GetDemandResolvers(
      IVssRequestContext requestContext,
      string demandType,
      DemandsResolutionType resolutionType)
    {
      IList<IDemandResolver> source = (IList<IDemandResolver>) null;
      this.m_demandResolvers.TryGetValue(demandType, out source);
      return source == null ? (IList<IDemandResolver>) null : (IList<IDemandResolver>) source.Where<IDemandResolver>((Func<IDemandResolver, bool>) (r => r.CanAnswerToResolutionType(demandType, resolutionType))).ToList<IDemandResolver>();
    }
  }
}
