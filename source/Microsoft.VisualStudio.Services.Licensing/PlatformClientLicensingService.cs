// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.PlatformClientLicensingService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class PlatformClientLicensingService : IPlatformClientLicensingService, IVssFrameworkService
  {
    private readonly ServiceFactory<IVssRegistryService> registryServiceFactory;
    private readonly IVssDateTimeProvider dateTimeProvider;
    private const string s_licensingRegistryRoot = "/Service/Licensing/";
    private const string s_registryNotificationFilter = "/Service/Licensing/VisualStudioRelease/...";
    private const string s_VSReleaseRegistryPrefix = "/Service/Licensing/VisualStudioRelease/";
    private const string s_VSReleaseMinVersionRegistryEntryName = "MinVersion";
    private const string s_VSReleaseMaxVersionRegistryEntryName = "MaxVersion";
    private const string s_VSReleaseBuildLabRegistryEntryName = "BuildLab";
    private const string s_VSReleaseReleaseTypeRegistryEntryName = "ReleaseType";
    private const string s_VSReleasePreviewExpirationDateRegistryEntryName = "ExpirationDate";
    private const string s_area = "Licensing";
    private const string s_layer = "PlatformClientLicensingService";

    public PlatformClientLicensingService()
      : this((ServiceFactory<IVssRegistryService>) (x => x.GetService<IVssRegistryService>()), VssDateTimeProvider.DefaultProvider)
    {
    }

    internal PlatformClientLicensingService(
      ServiceFactory<IVssRegistryService> registryServiceFactory,
      IVssDateTimeProvider dateTimeProvider)
    {
      this.VisualStudioReleases = (IList<ClientRelease>) new List<ClientRelease>();
      this.LastKnownGoodVisualStudioReleases = (IList<ClientRelease>) new List<ClientRelease>();
      this.registryServiceFactory = registryServiceFactory;
      this.dateTimeProvider = dateTimeProvider;
    }

    public void ServiceStart(IVssRequestContext deploymentContext)
    {
      deploymentContext.CheckSystemRequestContext();
      deploymentContext.CheckDeploymentRequestContext();
      deploymentContext.CheckHostedDeployment();
      this.registryServiceFactory(deploymentContext).RegisterNotification(deploymentContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/Service/Licensing/VisualStudioRelease/...");
      this.PopulateSettings(deploymentContext);
    }

    public void ServiceEnd(IVssRequestContext deploymentContext)
    {
    }

    public virtual ClientReleaseType GetClientReleaseType(
      IVssRequestContext deploymentContext,
      Version version,
      string buildLab)
    {
      foreach (ClientRelease visualStudioRelease in (IEnumerable<ClientRelease>) this.VisualStudioReleases)
      {
        if (visualStudioRelease.IsInRange(version, buildLab))
          return visualStudioRelease.ReleaseType;
      }
      return ClientReleaseType.None;
    }

    public virtual DateTimeOffset GetClientReleaseExpirationDate(
      IVssRequestContext deploymentContext,
      IRightsQueryContext queryContext,
      DateTimeOffset defaultExpirationDate,
      ClientReleaseType releaseType)
    {
      Version version = queryContext.ProductVersion;
      string buildLab = queryContext.ProductVersionBuildLab;
      DateTimeOffset dateTimeOffset = this.VisualStudioReleases.Where<ClientRelease>((Func<ClientRelease, bool>) (releases => releases.IsInRange(version, buildLab) && releases.ReleaseType == releaseType && releases.ExpirationDate != DateTimeOffset.MinValue)).Select<ClientRelease, DateTimeOffset>((Func<ClientRelease, DateTimeOffset>) (release => release.ExpirationDate)).FirstOrDefault<DateTimeOffset>();
      return !(dateTimeOffset != DateTimeOffset.MinValue) ? defaultExpirationDate : dateTimeOffset;
    }

    public virtual List<string> AddOrUpdateVisualStudioRelease(
      IVssRequestContext deploymentContext,
      ClientRelease newRelease,
      bool forceUpdate)
    {
      ClientRelease existingRelease = this.VisualStudioReleases.FirstOrDefault<ClientRelease>((Func<ClientRelease, bool>) (r =>
      {
        if (!r.Name.Equals(newRelease.Name, StringComparison.OrdinalIgnoreCase))
          return false;
        return LicensingComparers.RightNameComparer.Equals(newRelease.BuildLab, r.BuildLab) || string.Equals(newRelease.BuildLab, "*", StringComparison.Ordinal);
      }));
      IVssRegistryService registryService = this.registryServiceFactory(deploymentContext);
      List<RegistryEntry> registryEntryList = new List<RegistryEntry>()
      {
        new RegistryEntry()
        {
          Path = "/Service/Licensing/VisualStudioRelease/" + newRelease.Name + "/BuildLab",
          Value = newRelease.BuildLab
        },
        new RegistryEntry()
        {
          Path = "/Service/Licensing/VisualStudioRelease/" + newRelease.Name + "/ReleaseType",
          Value = newRelease.ReleaseType.ToString()
        },
        new RegistryEntry()
        {
          Path = "/Service/Licensing/VisualStudioRelease/" + newRelease.Name + "/MaxVersion",
          Value = newRelease.MaxVersion.ToString()
        },
        new RegistryEntry()
        {
          Path = "/Service/Licensing/VisualStudioRelease/" + newRelease.Name + "/MinVersion",
          Value = newRelease.MinVersion.ToString()
        }
      };
      List<string> source = new List<string>();
      if (newRelease.ExpirationDate != new DateTimeOffset())
      {
        if (newRelease.ExpirationDate < (DateTimeOffset) this.GetUtcNow() && !forceUpdate)
          source.Add(LicensingResources.VsReleasePastExpirationDate());
        registryEntryList.Add(new RegistryEntry()
        {
          Path = "/Service/Licensing/VisualStudioRelease/" + newRelease.Name + "/ExpirationDate",
          Value = newRelease.ExpirationDate.ToString("d", (IFormatProvider) CultureInfo.InvariantCulture)
        });
      }
      if (!forceUpdate)
      {
        ClientRelease clientRelease = this.VisualStudioReleases.FirstOrDefault<ClientRelease>((Func<ClientRelease, bool>) (r =>
        {
          if (r == existingRelease)
            return false;
          return r.IsInRange(newRelease.MinVersion, newRelease.BuildLab) || r.IsInRange(newRelease.MaxVersion, newRelease.BuildLab);
        }));
        if (clientRelease != null)
          source.Add(LicensingResources.VsReleaseOverlapDetected((object) clientRelease.Name, (object) clientRelease.BuildLab, (object) clientRelease.MinVersion, (object) clientRelease.MaxVersion));
        else if (!this.VisualStudioReleases.Any<ClientRelease>((Func<ClientRelease, bool>) (r => newRelease.MinVersion > r.MaxVersion && newRelease.MinVersion.Build - r.MaxVersion.Build <= 1 && newRelease.MinVersion.Major - r.MaxVersion.Major <= 1 && newRelease.MinVersion.Minor - r.MaxVersion.Minor <= 1 && (int) newRelease.MinVersion.MajorRevision - (int) r.MaxVersion.MajorRevision <= 1 && (int) newRelease.MinVersion.MinorRevision - (int) r.MaxVersion.MinorRevision <= 1)) && this.VisualStudioReleases.Any<ClientRelease>((Func<ClientRelease, bool>) (r => r.MinVersion.Major == newRelease.MinVersion.Major)))
          source.Add(string.Format(LicensingResources.VsReleaseNoPriorRelease((object) newRelease.MinVersion, (object) newRelease.MaxVersion)));
      }
      if (!source.Any<string>() | forceUpdate)
      {
        if (existingRelease != null)
          this.RemoveVisualStudioRelease(deploymentContext, newRelease.Name);
        registryService.WriteEntries(deploymentContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      return source;
    }

    public virtual void RemoveVisualStudioRelease(IVssRequestContext deploymentContext, string name)
    {
      ClientRelease clientRelease = this.VisualStudioReleases.FirstOrDefault<ClientRelease>((Func<ClientRelease, bool>) (r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
      if (clientRelease == null)
        throw new ClientReleaseNotFoundException(name);
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      string str = clientRelease.RegistryPath + "/...";
      IVssRequestContext requestContext = deploymentContext;
      string registryPathPattern = str;
      service.DeleteEntries(requestContext, registryPathPattern);
      this.VisualStudioReleases.Remove(clientRelease);
    }

    internal void PopulateSettings(IVssRequestContext deploymentContext)
    {
      RegistryEntryCollection registryEntries = deploymentContext.GetService<IVssRegistryService>().ReadEntriesFallThru(deploymentContext, (RegistryQuery) "/Service/Licensing/VisualStudioRelease/...");
      this.VisualStudioReleases = this.GenerateVisualStudioReleases(deploymentContext, registryEntries);
      this.LastKnownGoodVisualStudioReleases = this.VisualStudioReleases;
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.dateTimeProvider.UtcNow;

    private void OnRegistryChanged(
      IVssRequestContext deploymentContext,
      RegistryEntryCollection changedEntries)
    {
      deploymentContext.TraceEnter(1030070, "Licensing", nameof (PlatformClientLicensingService), nameof (OnRegistryChanged));
      try
      {
        if (changedEntries.IsNullOrEmpty<RegistryEntry>())
          return;
        this.PopulateSettings(deploymentContext);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(deploymentContext, ex.Message, ex);
        deploymentContext.TraceException(1030078, "Licensing", nameof (PlatformClientLicensingService), ex);
      }
      finally
      {
        deploymentContext.TraceLeave(1030079, "Licensing", nameof (PlatformClientLicensingService), nameof (OnRegistryChanged));
      }
    }

    private IList<ClientRelease> GenerateVisualStudioReleases(
      IVssRequestContext deploymentContext,
      RegistryEntryCollection registryEntries)
    {
      if (registryEntries.IsNullOrEmpty<RegistryEntry>())
      {
        deploymentContext.Trace(1031640, TraceLevel.Error, "Licensing", nameof (PlatformClientLicensingService), "No registry entries for VisualStudioRelease.");
        return this.GenerateDefaultVisualStudioReleases();
      }
      try
      {
        Dictionary<string, IList<Tuple<string, string, string>>> dictionary = new Dictionary<string, IList<Tuple<string, string, string>>>();
        foreach (RegistryEntry registryEntry in registryEntries)
        {
          int length = registryEntry.Path.LastIndexOf('/');
          if (length < 0)
            throw new LicensingInvalidSettingsException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Registry path {0} should have delimiters.", (object) registryEntry.Path));
          string key = registryEntry.Path.Substring(0, length);
          Tuple<string, string, string> tuple = new Tuple<string, string, string>(registryEntry.Name, registryEntry.Value, registryEntry.Path);
          if (!dictionary.ContainsKey(key))
            dictionary.Add(key, (IList<Tuple<string, string, string>>) new List<Tuple<string, string, string>>());
          dictionary[key].Add(tuple);
        }
        List<ClientRelease> visualStudioReleases = new List<ClientRelease>();
        foreach (KeyValuePair<string, IList<Tuple<string, string, string>>> keyValuePair in dictionary)
        {
          string minVersionValue = (string) null;
          string maxVersionValue = (string) null;
          string buildLabValue = (string) null;
          string releaseTypeValue = (string) null;
          DateTimeOffset? expirationDate = new DateTimeOffset?();
          foreach (Tuple<string, string, string> tuple in (IEnumerable<Tuple<string, string, string>>) keyValuePair.Value)
          {
            string x = tuple.Item1;
            string input = tuple.Item2;
            if (LicensingComparers.RightNameComparer.Equals(x, "MinVersion"))
              minVersionValue = input;
            else if (LicensingComparers.RightNameComparer.Equals(x, "MaxVersion"))
              maxVersionValue = input;
            else if (LicensingComparers.RightNameComparer.Equals(x, "BuildLab"))
              buildLabValue = input;
            else if (LicensingComparers.RightNameComparer.Equals(x, "ReleaseType"))
            {
              releaseTypeValue = input;
            }
            else
            {
              DateTimeOffset result;
              if (LicensingComparers.RightNameComparer.Equals(x, "ExpirationDate") && DateTimeOffset.TryParse(input, out result))
                expirationDate = new DateTimeOffset?(result);
            }
          }
          try
          {
            int startIndex = keyValuePair.Key.LastIndexOf('/') + 1;
            ClientRelease clientRelease = new ClientRelease(deploymentContext, keyValuePair.Key.Substring(startIndex), minVersionValue, maxVersionValue, buildLabValue, releaseTypeValue, expirationDate);
            visualStudioReleases.Add(clientRelease);
          }
          catch (Exception ex)
          {
            deploymentContext.TraceException(1031648, "Licensing", nameof (PlatformClientLicensingService), ex);
            if (this.LastKnownGoodVisualStudioReleases.IsNullOrEmpty<ClientRelease>())
            {
              deploymentContext.TraceAlways(1031645, TraceLevel.Info, "Licensing", nameof (PlatformClientLicensingService), string.Format("Release with versions {0}-{1} expiring on {2} had errors but no last known good configuration to fall back to. Ignoring release.", (object) minVersionValue, (object) maxVersionValue, (object) expirationDate));
            }
            else
            {
              deploymentContext.TraceAlways(1031645, TraceLevel.Info, "Licensing", nameof (PlatformClientLicensingService), string.Format("Release with versions {0}-{1} expiring on {2} had errors. Ignoring new configuration and falling back to last known good configuration.", (object) minVersionValue, (object) maxVersionValue, (object) expirationDate));
              return this.LastKnownGoodVisualStudioReleases;
            }
          }
        }
        return (IList<ClientRelease>) visualStudioReleases;
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(1031649, "Licensing", nameof (PlatformClientLicensingService), ex);
        if (this.LastKnownGoodVisualStudioReleases.IsNullOrEmpty<ClientRelease>())
        {
          deploymentContext.TraceAlways(1031646, TraceLevel.Info, "Licensing", nameof (PlatformClientLicensingService), "New licensing configuration was invalid. No last known good configuration, falling back to defaults.");
          return this.GenerateDefaultVisualStudioReleases();
        }
        deploymentContext.TraceAlways(1031646, TraceLevel.Info, "Licensing", nameof (PlatformClientLicensingService), "New licensing configuration was invalid. Falling back to last known good configuration.");
        return this.LastKnownGoodVisualStudioReleases;
      }
    }

    private IList<ClientRelease> GenerateDefaultVisualStudioReleases() => (IList<ClientRelease>) new List<ClientRelease>();

    internal virtual IList<ClientRelease> VisualStudioReleases { get; private set; }

    internal virtual IList<ClientRelease> LastKnownGoodVisualStudioReleases { get; private set; }
  }
}
