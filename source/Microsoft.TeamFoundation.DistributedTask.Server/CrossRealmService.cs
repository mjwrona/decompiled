// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.CrossRealmService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class CrossRealmService : IVssFrameworkService
  {
    private const string c_layer = "CrossRealmService";
    private CrossRealm.OAuth2Settings m_oauth2Settings;
    private ConcurrentDictionary<string, CrossRealm> m_realms;

    public CrossRealmService() => this.m_realms = new ConcurrentDictionary<string, CrossRealm>();

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemContext)
    {
      systemContext.CheckDeploymentRequestContext();
      systemContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged), "ConfigurationSecrets", (IEnumerable<string>) CrossRealmService.Registration.All.Select<CrossRealmService.Registration, string>((Func<CrossRealmService.Registration, string>) (x => x.LightRailSetting)).ToList<string>());
      Interlocked.CompareExchange<CrossRealm.OAuth2Settings>(ref this.m_oauth2Settings, this.LoadSettings(systemContext), (CrossRealm.OAuth2Settings) null);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemContext) => systemContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged));

    public (CrossRealmService.RequestContextAdapter requestContextAdapter, CrossRealm crossRealm) GetRealm(
      IVssRequestContext requestContext,
      string realm,
      Uri mmsUrl)
    {
      CrossRealmService.RequestContextAdapter instance = CrossRealmService.RequestContextAdapter.GetInstance(requestContext);
      ArgumentUtility.CheckForNull<string>(realm, nameof (realm));
      DeploymentRealm realmEnum;
      if (!Enum.TryParse<DeploymentRealm>(realm, true, out realmEnum))
        throw new InvalidOperationException("CrossRealmService target realm not supported: " + realm);
      CrossRealm orAdd = this.m_realms.GetOrAdd(mmsUrl.AbsoluteUri, (Func<string, CrossRealm>) (key =>
      {
        DeploymentRealm deploymentRealm = realmEnum;
        if (deploymentRealm == 3)
          return new CrossRealm((Func<Guid, Uri>) (instanceType => new Uri("https://vstoken.codedev.ms/_apis/oauth2/token/" + instanceType.ToString())), new Uri("https://vsmps.codedev.ms"), new Uri("https://vstsmms.codedev.ms"), (Func<CrossRealm.OAuth2Settings>) (() => this.m_oauth2Settings));
        if (deploymentRealm == 5)
          return new CrossRealm((Func<Guid, Uri>) (instanceType => new Uri("https://vstoken.actions.githubusercontent.com/_apis/oauth2/token/" + instanceType.ToString())), new Uri("https://vsmps.actions.githubusercontent.com"), mmsUrl, (Func<CrossRealm.OAuth2Settings>) (() => this.m_oauth2Settings));
        throw new InvalidOperationException("CrossRealmService target realm not supported: " + realm);
      }));
      return (instance, orAdd);
    }

    private void OnStrongBoxChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      Volatile.Write<CrossRealm.OAuth2Settings>(ref this.m_oauth2Settings, this.LoadSettings(requestContext));
    }

    private CrossRealm.OAuth2Settings LoadSettings(IVssRequestContext systemContext)
    {
      ITeamFoundationStrongBoxService service = systemContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(systemContext, "ConfigurationSecrets", true);
      (CrossRealmService.Registration, X509Certificate2) valueTuple = ();
      foreach (CrossRealmService.Registration registration1 in (IEnumerable<CrossRealmService.Registration>) CrossRealmService.Registration.All)
      {
        X509Certificate2 x509Certificate2_1 = service.RetrieveFileAsCertificate(systemContext, drawerId, registration1.LightRailSetting, throwOnFailure: false);
        systemContext.TraceAlways(280492494, TraceLevel.Info, "DistributedTask", nameof (CrossRealmService), string.Format("Considering registration: {0}, Thumbprint: {1}, NotBefore: {2}", (object) registration1, (object) x509Certificate2_1?.Thumbprint, (object) x509Certificate2_1?.NotBefore));
        (CrossRealmService.Registration registration2, X509Certificate2 x509Certificate2_2) = valueTuple;
        if (registration2 != null || x509Certificate2_2 != null)
        {
          if (x509Certificate2_1 != null)
          {
            DateTime notBefore = x509Certificate2_1.NotBefore;
            X509Certificate2 x509Certificate2_3 = valueTuple.Item2;
            DateTime dateTime = x509Certificate2_3 != null ? x509Certificate2_3.NotBefore : DateTime.MinValue;
            if (!(notBefore > dateTime))
              continue;
          }
          else
            continue;
        }
        valueTuple = (registration1, x509Certificate2_1);
      }
      if (valueTuple.Item2 == null)
        throw new InvalidOperationException("No MMS Cross Realm settings found");
      CrossRealm.OAuth2Settings oauth2Settings = new CrossRealm.OAuth2Settings(valueTuple.Item1.Id.ToString(), valueTuple.Item2);
      systemContext.TraceAlways(280492494, TraceLevel.Info, "DistributedTask", nameof (CrossRealmService), string.Format("Chose result: {0}", (object) oauth2Settings));
      return oauth2Settings;
    }

    public class Registration
    {
      private Registration(Guid Id, string lightRailSetting)
      {
        this.Id = Id;
        this.LightRailSetting = lightRailSetting;
      }

      public override string ToString() => string.Format("(Id: {0}, LightRailSetting: {1})", (object) this.Id, (object) this.LightRailSetting);

      public static CrossRealmService.Registration TfsMmsCrossRealmOAuthSigningCert { get; } = new CrossRealmService.Registration(new Guid("1b0683a5-de7d-43f1-9417-42958c20f21e"), nameof (TfsMmsCrossRealmOAuthSigningCert));

      public static CrossRealmService.Registration TfsMmsCrossRealmSecondaryOAuthSigningCert { get; } = new CrossRealmService.Registration(new Guid("8bf6de95-1061-4df0-a31b-9aab08361e2a"), nameof (TfsMmsCrossRealmSecondaryOAuthSigningCert));

      public static IReadOnlyList<CrossRealmService.Registration> All { get; } = (IReadOnlyList<CrossRealmService.Registration>) new CrossRealmService.Registration[2]
      {
        CrossRealmService.Registration.TfsMmsCrossRealmOAuthSigningCert,
        CrossRealmService.Registration.TfsMmsCrossRealmSecondaryOAuthSigningCert
      };

      public Guid Id { get; }

      public string LightRailSetting { get; }
    }

    public sealed class RequestContextAdapter : CrossRealm.RequestContextAdapter
    {
      private RequestContextAdapter(IVssRequestContext requestContext)
      {
        requestContext.CheckSystemRequestContext();
        requestContext.CheckDeploymentRequestContext();
        this.RequestContext = requestContext;
        this.UseCachedAccessToken = requestContext.IsFeatureEnabled("DistributedTask.EnableMMSCrossRealmTokenCaching");
      }

      public static CrossRealmService.RequestContextAdapter GetInstance(
        IVssRequestContext requestContext)
      {
        CrossRealmService.RequestContextAdapter instance;
        if (!requestContext.TryGetItem<CrossRealmService.RequestContextAdapter>("CrossRealm.RequestContextAdapter", out instance))
        {
          instance = new CrossRealmService.RequestContextAdapter(requestContext);
          requestContext.AddDisposableResource((IDisposable) instance);
          requestContext.Items.Add("CrossRealm.RequestContextAdapter", (object) instance);
        }
        return instance;
      }

      public override Task<List<DelegatingHandler>> CreateDelegatingHandlersAsync<TClient>() => Task.FromResult<List<DelegatingHandler>>(ClientProviderHelper.GetMinimalDelegatingHandlers(this.RequestContext, typeof (TClient), ClientProviderHelper.Options.CreateDefault(this.RequestContext), "CrossRealm-" + typeof (TClient).Name));

      public IVssRequestContext RequestContext { get; }
    }
  }
}
