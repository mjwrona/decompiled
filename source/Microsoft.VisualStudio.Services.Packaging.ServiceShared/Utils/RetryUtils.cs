// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.RetryUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class RetryUtils
  {
    public const string DefaultMaxRetryDelayRegistryPath = "/Configuration/Packaging/RetryHelper/DefaultMaxRetryDelay";
    public const string DefaultMaxRetryProfileRegistryPathFormat = "/Configuration/Packaging/RetryHelper/{0}/RetryProfile";
    private static readonly TimeSpan OneSec = TimeSpan.FromSeconds(1.0);
    private static readonly TimeSpan MaxRetryDelayFallbackDefault = RetryUtils.OneSec;
    private static readonly TimeSpan[] RetryProfileDefault = new TimeSpan[10]
    {
      RetryUtils.OneSec,
      RetryUtils.OneSec,
      RetryUtils.OneSec,
      RetryUtils.OneSec,
      RetryUtils.OneSec,
      RetryUtils.OneSec,
      RetryUtils.OneSec,
      RetryUtils.OneSec,
      RetryUtils.OneSec,
      RetryUtils.OneSec
    };
    private static readonly string RetryProfileDefaultMarker = "default";

    public static TimeSpan[] GetRetryProfile(string useCase, IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str1 = string.Format("/Configuration/Packaging/RetryHelper/{0}/RetryProfile", (object) useCase);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str1;
      string profileDefaultMarker = RetryUtils.RetryProfileDefaultMarker;
      string str2 = service.GetValue(requestContext1, in local, true, profileDefaultMarker);
      if (str2.Equals(RetryUtils.RetryProfileDefaultMarker))
        return RetryUtils.RetryProfileDefault;
      return ((IEnumerable<string>) str2.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, TimeSpan>((Func<string, TimeSpan>) (s => TimeSpan.FromSeconds(double.Parse(s)))).ToArray<TimeSpan>();
    }

    public static TimeSpan GetDefaultMaxRetryDelay(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Configuration/Packaging/RetryHelper/DefaultMaxRetryDelay", true, RetryUtils.MaxRetryDelayFallbackDefault);

    public static bool IsRetryableStorageException(Exception e)
    {
      while (!(e is ChangeConflictException) && !(e is TargetModifiedAfterReadException))
      {
        if ((e = e.InnerException) == null)
          return false;
      }
      return true;
    }
  }
}
