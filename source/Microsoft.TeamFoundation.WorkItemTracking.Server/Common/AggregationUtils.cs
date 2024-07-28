// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.AggregationUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  internal class AggregationUtils
  {
    public static IEqualityComparer<T> GetEqualityComparer<T>(IVssRequestContext requestContext) => typeof (T) == typeof (string) ? requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer as IEqualityComparer<T> : (IEqualityComparer<T>) EqualityComparer<T>.Default;

    public static IEqualityComparer GetEqualityComparer(IVssRequestContext requestContext, Type t)
    {
      if (t == typeof (int))
        return (IEqualityComparer) EqualityComparer<int>.Default;
      if (t == typeof (double))
        return (IEqualityComparer) EqualityComparer<double>.Default;
      if (t == typeof (DateTime))
        return (IEqualityComparer) EqualityComparer<DateTime>.Default;
      if (t == typeof (Guid))
        return (IEqualityComparer) EqualityComparer<Guid>.Default;
      if (t == typeof (bool))
        return (IEqualityComparer) EqualityComparer<bool>.Default;
      if (t == typeof (string))
        return (IEqualityComparer) requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
      throw new InvalidOperationException();
    }
  }
}
