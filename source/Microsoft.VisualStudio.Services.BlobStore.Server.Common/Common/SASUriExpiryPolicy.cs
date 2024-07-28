// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.SASUriExpiryPolicy
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class SASUriExpiryPolicy
  {
    public static readonly SASUriExpiryBounds DefaultBounds = new SASUriExpiryBounds(TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(60.0));
    internal static readonly TimeSpan DefaultExpiry;
    private static readonly double DefaultMaxExpiryHours;
    private static readonly double DefaultMinExpiryHours;
    internal static readonly string OverrideMaxExpiryHoursRegistryPath;
    internal static readonly string OverrideMinExpiryHoursRegistryPath;
    private static readonly RegistryQuery OverrideMaxExpiryHoursRegistryQuery;
    private static readonly RegistryQuery OverrideMinExpiryHoursRegistryQuery;
    internal const int TracePointInvalidOverrideInRegistry = 5700102;
    private const string TracePointArea = "BlobStore";
    private const string TracePointLayer = "Service";
    public readonly IClock Clock;

    public SASUriExpiryPolicy(IClock clock)
    {
      ArgumentUtility.CheckForNull<IClock>(clock, nameof (clock));
      this.Clock = clock;
    }

    internal SASUriExpiryBounds GetExpiryBounds(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      double num = SASUriExpiryPolicy.GetRegistryValueAndWarnIfInvalid<double>(context, service, (RegistryQuery) SASUriExpiryPolicy.OverrideMaxExpiryHoursRegistryPath, true, SASUriExpiryPolicy.DefaultMaxExpiryHours);
      if (num < SASUriExpiryPolicy.DefaultMaxExpiryHours)
      {
        context.Trace(5700102, TraceLevel.Warning, "BlobStore", "Service", string.Format("Ignoring \"{0}\" at \"{1}\" because it's < {2} of {3}", (object) num, (object) SASUriExpiryPolicy.OverrideMaxExpiryHoursRegistryPath.ToString(), (object) "DefaultMaxExpiryHours", (object) SASUriExpiryPolicy.DefaultMaxExpiryHours));
        num = SASUriExpiryPolicy.DefaultMaxExpiryHours;
      }
      double defaultMinExpiryHours = service.GetValue<double>(context, (RegistryQuery) SASUriExpiryPolicy.OverrideMinExpiryHoursRegistryPath, true, SASUriExpiryPolicy.DefaultMinExpiryHours);
      if (defaultMinExpiryHours >= num)
      {
        context.Trace(5700102, TraceLevel.Warning, "BlobStore", "Service", string.Format("Ignoring \"{0}\" at \"{1}\" because it's >= {2} of {3}", (object) defaultMinExpiryHours, (object) SASUriExpiryPolicy.OverrideMinExpiryHoursRegistryPath.ToString(), (object) "maxExpiryHours", (object) num));
        defaultMinExpiryHours = SASUriExpiryPolicy.DefaultMinExpiryHours;
      }
      return new SASUriExpiryBounds(TimeSpan.FromHours(defaultMinExpiryHours), TimeSpan.FromHours(num));
    }

    private static T GetRegistryValueAndWarnIfInvalid<T>(
      IVssRequestContext context,
      IVssRegistryService registry,
      RegistryQuery query,
      bool fallThru,
      T defaultValue)
    {
      string str = registry.GetValue(context, in query, fallThru);
      if (string.IsNullOrEmpty(str))
        return defaultValue;
      try
      {
        return SASUriExpiryPolicy.FromStringOrThrow<T>(str, defaultValue);
      }
      catch (Exception ex)
      {
        context.Trace(5700102, TraceLevel.Warning, "BlobStore", "Service", "Registry value \"" + str + "\" at \"" + query.ToString() + "\" cannot be converted to " + typeof (T).Name + " because: " + ex.GetType().Name + ": " + ex.Message);
        return defaultValue;
      }
    }

    private static T FromStringOrThrow<T>(string value, T defaultValue)
    {
      if (value == null)
        return defaultValue;
      if (typeof (T).IsAssignableFrom(typeof (string)))
        return (T) value;
      TypeConverter converter = TypeDescriptor.GetConverter(typeof (T));
      return !converter.CanConvertFrom(typeof (string)) ? defaultValue : (T) converter.ConvertFromInvariantString(value);
    }

    static SASUriExpiryPolicy()
    {
      TimeSpan timeSpan = SASUriExpiryPolicy.DefaultBounds.MinExpiry;
      long ticks1 = timeSpan.Ticks;
      timeSpan = SASUriExpiryPolicy.DefaultBounds.MaxExpiry;
      long ticks2 = timeSpan.Ticks;
      SASUriExpiryPolicy.DefaultExpiry = new TimeSpan((ticks1 + ticks2) / 2L);
      SASUriExpiryPolicy.DefaultMaxExpiryHours = SASUriExpiryPolicy.DefaultBounds.MaxExpiry.TotalHours;
      SASUriExpiryPolicy.DefaultMinExpiryHours = SASUriExpiryPolicy.DefaultBounds.MinExpiry.TotalHours;
      SASUriExpiryPolicy.OverrideMaxExpiryHoursRegistryPath = "/Configuration/BlobStoreShared/SASUriExpiryMaxHours";
      SASUriExpiryPolicy.OverrideMinExpiryHoursRegistryPath = "/Configuration/BlobStoreShared/SASUriExpiryMinHours";
      SASUriExpiryPolicy.OverrideMaxExpiryHoursRegistryQuery = new RegistryQuery(SASUriExpiryPolicy.OverrideMaxExpiryHoursRegistryPath);
      SASUriExpiryPolicy.OverrideMinExpiryHoursRegistryQuery = new RegistryQuery(SASUriExpiryPolicy.OverrideMinExpiryHoursRegistryPath);
    }
  }
}
