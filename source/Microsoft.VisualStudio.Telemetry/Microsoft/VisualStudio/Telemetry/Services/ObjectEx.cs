// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.ObjectEx
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal static class ObjectEx
  {
    public static bool TryConvertToType<T>(
      this object originalValue,
      T defaultValue,
      out T returnValue)
    {
      if (originalValue == null)
      {
        returnValue = defaultValue;
        return false;
      }
      if (originalValue is T obj)
        returnValue = obj;
      try
      {
        returnValue = (T) Convert.ChangeType(originalValue, typeof (T));
        return true;
      }
      catch
      {
        returnValue = defaultValue;
        return false;
      }
    }
  }
}
