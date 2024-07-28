// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.TimeSpanOrInfiniteConverter
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Diagnostics;
using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs
{
  internal class TimeSpanOrInfiniteConverter : TimeSpanConverter
  {
    public override object ConvertTo(
      ITypeDescriptorContext ctx,
      CultureInfo ci,
      object value,
      Type type)
    {
      if (value == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (value));
      if (!(value is TimeSpan timeSpan))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(nameof (value), SR.GetString(Resources.SFxWrongType2, (object) typeof (TimeSpan), (object) value.GetType()));
      return timeSpan == TimeSpan.MaxValue ? (object) "Infinite" : base.ConvertTo(ctx, ci, value, type);
    }

    public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data) => string.Equals((string) data, "infinite", StringComparison.OrdinalIgnoreCase) ? (object) TimeSpan.MaxValue : base.ConvertFrom(ctx, ci, data);
  }
}
