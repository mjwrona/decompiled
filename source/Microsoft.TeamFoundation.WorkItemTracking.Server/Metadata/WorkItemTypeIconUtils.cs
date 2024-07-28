// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeIconUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public static class WorkItemTypeIconUtils
  {
    public static IReadOnlyList<string> Icons = (IReadOnlyList<string>) new List<string>(((IEnumerable<FieldInfo>) typeof (WorkItemTypeIconName).GetFields(BindingFlags.Static | BindingFlags.Public)).Where<FieldInfo>((Func<FieldInfo, bool>) (f => f.GetRawConstantValue() is string)).Select<FieldInfo, string>((Func<FieldInfo, string>) (f => f.GetRawConstantValue() as string)));
    public const int CurrentIconVersion = 2;

    public static string GetDefaultIcon() => "icon_clipboard";

    public static void ValidateIcon(IVssRequestContext requestContext, string icon)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(icon, nameof (icon));
      if (!WorkItemTypeIconUtils.Icons.Any<string>((Func<string, bool>) (i => string.Equals(i, icon, StringComparison.InvariantCultureIgnoreCase))))
        throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemTrackingInvalidIconId((object) icon));
    }
  }
}
