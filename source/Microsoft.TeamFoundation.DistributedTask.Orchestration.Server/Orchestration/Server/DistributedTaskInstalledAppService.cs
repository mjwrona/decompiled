// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DistributedTaskInstalledAppService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class DistributedTaskInstalledAppService : 
    IDistributedTaskInstalledAppService,
    IVssFrameworkService
  {
    private static readonly char[] c_invalidInstallationIdChars = new char[2]
    {
      '\\',
      '/'
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.CheckRequestContext(systemRequestContext);
      systemRequestContext.CheckHostedDeployment();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddInstallation(
      IVssRequestContext requestContext,
      string appId,
      string installationId,
      DistributedTaskInstalledAppData appData)
    {
      this.CheckRequestContext(requestContext);
      this.CheckAppId(appId);
      this.CheckInstallationId(installationId);
      ArgumentUtility.CheckForNull<DistributedTaskInstalledAppData>(appData, nameof (appData));
      Guid billingHostId = appData.BillingHostId;
      string str1 = appData.Data;
      if (string.IsNullOrEmpty(str1))
        str1 = installationId;
      string str2 = string.Format("{0}|{1}", (object) billingHostId, (object) str1);
      requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, this.GetInstallationPath(appId, installationId), str2);
    }

    public void RemoveInstallation(
      IVssRequestContext requestContext,
      string appId,
      string installationId)
    {
      this.CheckRequestContext(requestContext);
      this.CheckAppId(appId);
      this.CheckInstallationId(installationId);
      requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, this.GetInstallationPath(appId, installationId));
    }

    public IDictionary<string, DistributedTaskInstalledAppData> GetInstallations(
      IVssRequestContext requestContext,
      string appId)
    {
      this.CheckRequestContext(requestContext);
      this.CheckAppId(appId);
      return (IDictionary<string, DistributedTaskInstalledAppData>) requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, this.GetInstallationsQuery(appId)).ToDictionary<RegistryEntry, string, DistributedTaskInstalledAppData>((Func<RegistryEntry, string>) (e => e.Name), (Func<RegistryEntry, DistributedTaskInstalledAppData>) (e => this.ParseRegistryValue(requestContext, e.Value)));
    }

    public bool TryGetInstallationData(
      IVssRequestContext requestContext,
      string appId,
      string installationId,
      out DistributedTaskInstalledAppData appData)
    {
      this.CheckRequestContext(requestContext);
      this.CheckAppId(appId);
      this.CheckInstallationId(installationId);
      string str = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) this.GetInstallationPath(appId, installationId), (string) null);
      appData = this.ParseRegistryValue(requestContext, str);
      return !string.IsNullOrEmpty(appData.Data);
    }

    public bool IsInstalled(IVssRequestContext requestContext, string appId)
    {
      this.CheckRequestContext(requestContext);
      this.CheckAppId(appId);
      return requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, this.GetInstallationsQuery(appId)).Any<RegistryEntry>();
    }

    private DistributedTaskInstalledAppData ParseRegistryValue(
      IVssRequestContext requestContext,
      string value)
    {
      DistributedTaskInstalledAppData registryValue = new DistributedTaskInstalledAppData();
      if (value != null)
      {
        int length = value.IndexOf('|');
        Guid result;
        if (length > 0 && Guid.TryParse(value.Substring(0, length), out result))
        {
          registryValue.BillingHostId = result;
          registryValue.Data = value.Substring(length + 1);
        }
        else
        {
          registryValue.BillingHostId = requestContext.ServiceHost.InstanceId;
          registryValue.Data = value;
        }
      }
      else
      {
        registryValue.BillingHostId = Guid.Empty;
        registryValue.Data = (string) null;
      }
      return registryValue;
    }

    private void CheckRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
    }

    private void CheckAppId(string appId) => ArgumentUtility.CheckStringForNullOrEmpty(appId, nameof (appId));

    private void CheckInstallationId(string installationId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(installationId, nameof (installationId));
      ArgumentUtility.CheckStringForInvalidCharacters(installationId, nameof (installationId), DistributedTaskInstalledAppService.c_invalidInstallationIdChars);
    }

    private string GetInstallationPath(string appId, string installationId) => "/Service/DistributedTask/InstalledApps/" + appId + "/" + installationId;

    private RegistryQuery GetInstallationsQuery(string appId) => new RegistryQuery("/Service/DistributedTask/InstalledApps/" + appId + "/*");
  }
}
