// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationIdentityHelper
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class NotificationIdentityHelper
  {
    public static string FormatIdentityFieldValue(string displayName, Guid tfid)
    {
      if (displayName == null)
        displayName = string.Empty;
      return '|'.ToString() + displayName + "%" + tfid.ToString() + '|'.ToString();
    }

    public static string GetDisplayNameForIdentityField(string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        value = value.Remove(value.LastIndexOf('%'));
        value = value.Remove(value.LastIndexOf('|'), 1);
      }
      return value;
    }

    public static Guid GetTfIdForIdentityField(string value)
    {
      Guid result = Guid.Empty;
      try
      {
        if (!string.IsNullOrEmpty(value))
        {
          value = value.Remove(0, value.LastIndexOf('%') + 1);
          value = value.Remove(value.LastIndexOf('|'), 1);
          Guid.TryParse(value, out result);
        }
      }
      catch (Exception ex)
      {
      }
      return result;
    }
  }
}
