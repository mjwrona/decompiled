// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration.MsalConfigHelper
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration
{
  public class MsalConfigHelper : IMsalConfigHelper
  {
    private static readonly IConfigPrototype<string> CacheLocationConfigPrototype = ConfigPrototype.Create<string>("VisualStudio.WebPlatform.MsalConfig.CacheLocation", "localStorage");
    private readonly IConfigQueryable<string> _cacheLocationConfig;
    private static readonly IConfigPrototype<HashSet<string>> FatalErrorCodesConfigPrototype = ConfigPrototype.Create<HashSet<string>>("VisualStudio.WebPlatform.MsalConfig.FatalErrorCodes", MsalConfigHelper.DefaultFatalErrorCodes);
    private readonly IConfigQueryable<HashSet<string>> _fatalErrorCodesConfig;
    private static readonly IConfigPrototype<int> LogLevelConfigPrototype = ConfigPrototype.Create<int>("VisualStudio.WebPlatform.MsalConfig.LogLevel", 1);
    private readonly IConfigQueryable<int> _logLevelConfig;
    private static readonly IConfigPrototype<int> CacheLookupPolicyConfigPrototype = ConfigPrototype.Create<int>("VisualStudio.WebPlatform.MsalConfig.CacheLookupPolicy", 0);
    private readonly IConfigQueryable<int> _cacheLookupPolicyConfig;
    private static readonly IConfigPrototype<int?> IframeHashTimeoutConfigPrototype = ConfigPrototype.Create<int?>("VisualStudio.WebPlatform.MsalConfig.IframeHashTimeout", new int?());
    private readonly IConfigQueryable<int?> _iframeHashTimeoutConfig;
    private static readonly IConfigPrototype<long?> CacheWatermarkConfigPrototype = ConfigPrototype.Create<long?>("VisualStudio.WebPlatform.MsalConfig.CacheWatermark", new long?());
    private readonly IConfigQueryable<long?> _cacheWatermarkConfig;
    public static readonly MsalConfigHelper Instance = new MsalConfigHelper();

    public MsalConfigHelper()
      : this(ConfigProxy.Create<string>(MsalConfigHelper.CacheLocationConfigPrototype), ConfigProxy.Create<HashSet<string>>(MsalConfigHelper.FatalErrorCodesConfigPrototype), ConfigProxy.Create<int>(MsalConfigHelper.LogLevelConfigPrototype), ConfigProxy.Create<int>(MsalConfigHelper.CacheLookupPolicyConfigPrototype), ConfigProxy.Create<int?>(MsalConfigHelper.IframeHashTimeoutConfigPrototype), ConfigProxy.Create<long?>(MsalConfigHelper.CacheWatermarkConfigPrototype))
    {
    }

    public MsalConfigHelper(
      IConfigQueryable<string> cacheLocationConfig,
      IConfigQueryable<HashSet<string>> fatalErrorCodesConfig,
      IConfigQueryable<int> logLevelConfig,
      IConfigQueryable<int> cacheLookupPolicyConfig,
      IConfigQueryable<int?> iframeHashTimeoutPolicyConfig,
      IConfigQueryable<long?> cacheWatermarkConfig)
    {
      this._cacheLocationConfig = cacheLocationConfig;
      this._fatalErrorCodesConfig = fatalErrorCodesConfig;
      this._logLevelConfig = logLevelConfig;
      this._cacheLookupPolicyConfig = cacheLookupPolicyConfig;
      this._iframeHashTimeoutConfig = iframeHashTimeoutPolicyConfig;
      this._cacheWatermarkConfig = cacheWatermarkConfig;
    }

    public MsalConfig GetMsalConfig(IVssRequestContext requestContext) => new MsalConfig()
    {
      CacheLocation = this._cacheLocationConfig.QueryByCtx<string>(requestContext),
      FatalErrorCodes = new HashSet<string>((IEnumerable<string>) this._fatalErrorCodesConfig.QueryByCtx<HashSet<string>>(requestContext)),
      LogLevel = this._logLevelConfig.QueryByCtx<int>(requestContext),
      CacheLookupPolicy = this._cacheLookupPolicyConfig.QueryByCtx<int>(requestContext),
      IframeHashTimeout = this._iframeHashTimeoutConfig.QueryByCtx<int?>(requestContext),
      CacheWatermark = this._cacheWatermarkConfig.QueryByCtx<long?>(requestContext)
    };

    private static HashSet<string> DefaultFatalErrorCodes => new HashSet<string>()
    {
      "multiple_matching_tokens"
    };
  }
}
