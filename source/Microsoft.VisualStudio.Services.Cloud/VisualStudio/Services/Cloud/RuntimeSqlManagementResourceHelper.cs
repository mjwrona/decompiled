// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RuntimeSqlManagementResourceHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class RuntimeSqlManagementResourceHelper
  {
    public static SqlManagementResourceHelper Create(
      IVssRequestContext requestContext,
      ITFLogger logger)
    {
      Guid guid1 = new Guid(AzureRoleUtil.GetOverridableConfigurationSetting("AzureSubscriptionId"));
      string configurationSetting1 = AzureRoleUtil.GetOverridableConfigurationSetting("ResourceManagerEndpointUrl");
      string configurationSetting2 = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint");
      string configurationSetting3 = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalClientId");
      string configurationSetting4 = AzureRoleUtil.GetOverridableConfigurationSetting("ResourceManagerAadTenantId");
      if (requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.ResourceManagement.TenantIdFromSubscription"))
        configurationSetting4 = AadTenantHelper.WithManagedIdentityClient(guid1, logger).GetSingleTenantId().ToString();
      bool flag = requestContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.ManagementAccess");
      Guid guid2 = guid1;
      string str1 = configurationSetting4;
      string str2 = configurationSetting1;
      string str3 = configurationSetting3;
      string str4 = configurationSetting2;
      string str5 = str3;
      int num = flag ? 1 : 0;
      ITFLogger tfLogger = logger;
      return new SqlManagementResourceHelper(guid2, str1, str2, str4, str5, num != 0, tfLogger);
    }
  }
}
