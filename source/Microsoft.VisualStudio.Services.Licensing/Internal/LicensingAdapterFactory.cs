// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.LicensingAdapterFactory
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class LicensingAdapterFactory : ILicensingAdapterFactory
  {
    private readonly IList<ILicensingAdapter> m_clientAdapters;
    private readonly Dictionary<string, IList<ILicensingAdapter>> m_clientRightsToAdaptersMap;
    private readonly Dictionary<Type, ILicensingAdapter> m_adapters;
    private readonly IList<ILicensingAdapter> m_serviceAdapters;
    private readonly Dictionary<string, IList<ILicensingAdapter>> m_serviceRightsToAdaptersMap;

    public LicensingAdapterFactory()
    {
      VisualStudioExpressLicensingAdapter licensingAdapter1 = new VisualStudioExpressLicensingAdapter();
      VisualStudioAccountLicensingAdapter licensingAdapter2 = new VisualStudioAccountLicensingAdapter();
      MsdnLicensingAdapter licensingAdapter3 = new MsdnLicensingAdapter();
      VisualStudioTrialLicensingAdapter adapter1 = new VisualStudioTrialLicensingAdapter();
      VisualStudioCommunityLicenseAdapter adapter2 = new VisualStudioCommunityLicenseAdapter();
      VisualStudioAndroidEmulatorLicenseAdapter adapter3 = new VisualStudioAndroidEmulatorLicenseAdapter();
      ExtensionLicensingAdapter licensingAdapter4 = new ExtensionLicensingAdapter();
      this.m_adapters = new Dictionary<Type, ILicensingAdapter>();
      this.m_clientAdapters = (IList<ILicensingAdapter>) new ILicensingAdapter[4]
      {
        (ILicensingAdapter) licensingAdapter1,
        (ILicensingAdapter) licensingAdapter2,
        (ILicensingAdapter) licensingAdapter3,
        (ILicensingAdapter) licensingAdapter4
      };
      this.m_clientRightsToAdaptersMap = new Dictionary<string, IList<ILicensingAdapter>>((IEqualityComparer<string>) LicensingComparers.RightNameComparer)
      {
        {
          "VPDExpress",
          (IList<ILicensingAdapter>) new ILicensingAdapter[1]
          {
            (ILicensingAdapter) licensingAdapter1
          }
        },
        {
          "VSWinExpress",
          (IList<ILicensingAdapter>) new ILicensingAdapter[1]
          {
            (ILicensingAdapter) licensingAdapter1
          }
        },
        {
          "VWDExpress",
          (IList<ILicensingAdapter>) new ILicensingAdapter[1]
          {
            (ILicensingAdapter) licensingAdapter1
          }
        },
        {
          "WDExpress",
          (IList<ILicensingAdapter>) new ILicensingAdapter[1]
          {
            (ILicensingAdapter) licensingAdapter1
          }
        },
        {
          "TestProfessional",
          (IList<ILicensingAdapter>) new ILicensingAdapter[2]
          {
            (ILicensingAdapter) licensingAdapter2,
            (ILicensingAdapter) licensingAdapter3
          }
        },
        {
          "VisualStudio",
          (IList<ILicensingAdapter>) new ILicensingAdapter[3]
          {
            (ILicensingAdapter) licensingAdapter2,
            (ILicensingAdapter) licensingAdapter3,
            (ILicensingAdapter) adapter2
          }
        },
        {
          "VSonMac",
          (IList<ILicensingAdapter>) new ILicensingAdapter[3]
          {
            (ILicensingAdapter) licensingAdapter2,
            (ILicensingAdapter) licensingAdapter3,
            (ILicensingAdapter) adapter2
          }
        },
        {
          "TestManager",
          (IList<ILicensingAdapter>) new ILicensingAdapter[3]
          {
            (ILicensingAdapter) licensingAdapter2,
            (ILicensingAdapter) licensingAdapter3,
            (ILicensingAdapter) licensingAdapter4
          }
        },
        {
          "VisualStudioEmulatorAndroid",
          (IList<ILicensingAdapter>) new ILicensingAdapter[1]
          {
            (ILicensingAdapter) adapter3
          }
        }
      };
      this.m_serviceAdapters = (IList<ILicensingAdapter>) new ILicensingAdapter[2]
      {
        (ILicensingAdapter) licensingAdapter2,
        (ILicensingAdapter) licensingAdapter3
      };
      this.m_serviceRightsToAdaptersMap = new Dictionary<string, IList<ILicensingAdapter>>((IEqualityComparer<string>) LicensingComparers.RightNameComparer);
      this.m_serviceRightsToAdaptersMap.Add("VisualStudioOnlineService", (IList<ILicensingAdapter>) new ILicensingAdapter[2]
      {
        (ILicensingAdapter) licensingAdapter2,
        (ILicensingAdapter) licensingAdapter3
      });
      foreach (ILicensingAdapter clientAdapter in (IEnumerable<ILicensingAdapter>) this.m_clientAdapters)
        this.AddAdapterInGlobalCollection(clientAdapter);
      foreach (ILicensingAdapter serviceAdapter in (IEnumerable<ILicensingAdapter>) this.m_serviceAdapters)
        this.AddAdapterInGlobalCollection(serviceAdapter);
      this.AddAdapterInGlobalCollection((ILicensingAdapter) adapter1);
      this.AddAdapterInGlobalCollection((ILicensingAdapter) adapter2);
      this.AddAdapterInGlobalCollection((ILicensingAdapter) adapter3);
    }

    internal LicensingAdapterFactory(
      IList<ILicensingAdapter> clientAdapters = null,
      Dictionary<string, IList<ILicensingAdapter>> clientRightsToAdaptersMap = null,
      ILicensingAdapter trialAdapter = null,
      IList<ILicensingAdapter> serviceAdapters = null,
      Dictionary<string, IList<ILicensingAdapter>> serviceRightsToAdaptersMap = null,
      MsdnLicensingAdapter msdnLicensingAdapter = null)
    {
      this.m_adapters = new Dictionary<Type, ILicensingAdapter>();
      this.m_clientAdapters = clientAdapters;
      this.m_clientRightsToAdaptersMap = clientRightsToAdaptersMap;
      this.m_serviceAdapters = serviceAdapters;
      this.m_serviceRightsToAdaptersMap = clientRightsToAdaptersMap;
      if (this.m_clientAdapters != null)
      {
        foreach (ILicensingAdapter clientAdapter in (IEnumerable<ILicensingAdapter>) this.m_clientAdapters)
          this.AddAdapterInGlobalCollection(clientAdapter);
      }
      if (this.m_serviceAdapters != null)
      {
        foreach (ILicensingAdapter serviceAdapter in (IEnumerable<ILicensingAdapter>) this.m_serviceAdapters)
          this.AddAdapterInGlobalCollection(serviceAdapter);
      }
      if (trialAdapter == null)
        return;
      this.AddAdapterInGlobalCollection(trialAdapter);
    }

    public void Start(
      IVssRequestContext requestContext,
      LicensingServiceConfiguration serviceSettings,
      IVssDateTimeProvider dateTimeProvider)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<LicensingServiceConfiguration>(serviceSettings, nameof (serviceSettings));
      foreach (ILicensingAdapter licensingAdapter in this.m_adapters.Values)
        licensingAdapter.Start(requestContext, serviceSettings, dateTimeProvider);
    }

    public void Stop(IVssRequestContext requestContext)
    {
      foreach (ILicensingAdapter licensingAdapter in this.m_adapters.Values)
        licensingAdapter.Stop(requestContext);
    }

    public IList<ILicensingAdapter> GetClientAdapters() => this.m_clientAdapters;

    public IList<ILicensingAdapter> GetClientAdapters(string rightName) => LicensingAdapterFactory.GetAdapters(this.m_clientRightsToAdaptersMap, rightName);

    public IList<ILicensingAdapter> GetServiceAdapters() => this.m_clientAdapters;

    public IList<ILicensingAdapter> GetServiceAdapters(string rightName) => LicensingAdapterFactory.GetAdapters(this.m_serviceRightsToAdaptersMap, rightName);

    public T GetAdapter<T>() where T : ILicensingAdapter
    {
      ILicensingAdapter adapter;
      this.m_adapters.TryGetValue(typeof (T), out adapter);
      return (T) adapter;
    }

    private static IList<ILicensingAdapter> GetAdapters(
      Dictionary<string, IList<ILicensingAdapter>> map,
      string rightName)
    {
      IList<ILicensingAdapter> licensingAdapterList;
      return !map.TryGetValue(rightName, out licensingAdapterList) ? (IList<ILicensingAdapter>) new List<ILicensingAdapter>() : licensingAdapterList;
    }

    private void AddAdapterInGlobalCollection(ILicensingAdapter adapter) => this.m_adapters[adapter.GetType()] = adapter;
  }
}
