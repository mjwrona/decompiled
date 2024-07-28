// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.EmptyJavaScriptProxyGenerator
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Globalization;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class EmptyJavaScriptProxyGenerator : IJavaScriptProxyGenerator
  {
    public string GenerateProxy(string serviceUrl) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "throw new Error('{0}');", new object[1]
    {
      (object) Resources.Error_JavaScriptProxyDisabled
    });
  }
}
