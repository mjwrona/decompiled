// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.MethodExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public static class MethodExtensions
  {
    public static bool Matches(this MethodDescriptor methodDescriptor, IList<IJsonValue> parameters)
    {
      if (methodDescriptor == null)
        throw new ArgumentNullException(nameof (methodDescriptor));
      return (methodDescriptor.Parameters.Count <= 0 || parameters != null) && methodDescriptor.Parameters.Count == parameters.Count;
    }
  }
}
