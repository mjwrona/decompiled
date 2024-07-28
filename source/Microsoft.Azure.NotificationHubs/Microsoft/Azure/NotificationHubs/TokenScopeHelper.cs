// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.TokenScopeHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Diagnostics;
using System;
using System.ComponentModel;

namespace Microsoft.Azure.NotificationHubs
{
  internal static class TokenScopeHelper
  {
    public static bool IsDefined(TokenScope v) => v == TokenScope.Entity || v == TokenScope.Namespace;

    public static void Validate(TokenScope value)
    {
      if (!TokenScopeHelper.IsDefined(value))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidEnumArgumentException(nameof (value), (int) value, typeof (TokenScope)));
    }
  }
}
