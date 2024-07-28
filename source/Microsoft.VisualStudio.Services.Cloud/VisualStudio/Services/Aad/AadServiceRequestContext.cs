// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadServiceRequestContext
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.VisualStudio.Services.Aad
{
  internal class AadServiceRequestContext
  {
    private readonly IVssRequestContext context;
    private readonly string tenantId;
    private readonly bool application;
    private readonly IDictionary<string, object> settings;
    private readonly Func<IAadGraphClient> graphClient;
    private readonly Func<IMicrosoftGraphClient> msGraphClient;
    private readonly Func<IVssRequestContext, string, bool, bool, JwtSecurityToken> accessToken;

    internal AadServiceRequestContext(
      IVssRequestContext context,
      string tenantId,
      bool application,
      IDictionary<string, object> settings,
      Func<IAadGraphClient> graphClient,
      Func<IMicrosoftGraphClient> msGraphClient,
      Func<IVssRequestContext, string, bool, bool, JwtSecurityToken> accessToken)
    {
      this.context = context;
      this.tenantId = tenantId;
      this.application = application;
      this.settings = settings;
      this.graphClient = graphClient;
      this.msGraphClient = msGraphClient;
      this.accessToken = accessToken;
    }

    internal IVssRequestContext VssRequestContext => this.context;

    internal string TenantId => this.tenantId;

    internal T GetSetting<T>(string key, T defaultValue)
    {
      T obj;
      return !this.settings.TryGetValue<T>(key, out obj) ? defaultValue : obj;
    }

    internal IAadGraphClient GetGraphClient() => this.graphClient();

    internal IMicrosoftGraphClient GetMsGraphClient() => this.msGraphClient();

    internal JwtSecurityToken GetAccessToken() => this.accessToken(this.context, this.tenantId, this.application, false);

    internal JwtSecurityToken GetAccessToken(string tenantId) => this.accessToken(this.context, tenantId, this.application, false);

    internal JwtSecurityToken GetAccessToken(string tenantId, bool application) => this.accessToken(this.context, tenantId, application, false);

    internal JwtSecurityToken GetAccessToken(bool isMicrosoftGraphApi, string tenantId) => this.accessToken(this.context, tenantId, this.application, isMicrosoftGraphApi);

    internal JwtSecurityToken GetAccessToken(bool isMicrosoftGraphApi) => this.accessToken(this.context, this.tenantId, this.application, isMicrosoftGraphApi);

    internal JwtSecurityToken GetAccessToken(bool application, bool isMicrosoftGraphApi) => this.accessToken(this.context, this.tenantId, application, isMicrosoftGraphApi);

    internal JwtSecurityToken GetAccessToken(
      string tenantId,
      bool application,
      bool isMicrosoftGraphApi)
    {
      return this.accessToken(this.context, tenantId, application, isMicrosoftGraphApi);
    }
  }
}
