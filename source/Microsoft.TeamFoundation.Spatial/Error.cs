// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.Error
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;

namespace Microsoft.Spatial
{
  internal static class Error
  {
    internal static Exception ArgumentNull(string paramName) => (Exception) new ArgumentNullException(paramName);

    internal static Exception ArgumentOutOfRange(string paramName) => (Exception) new ArgumentOutOfRangeException(paramName);

    internal static Exception NotImplemented() => (Exception) new NotImplementedException();

    internal static Exception NotSupported() => (Exception) new NotSupportedException();
  }
}
