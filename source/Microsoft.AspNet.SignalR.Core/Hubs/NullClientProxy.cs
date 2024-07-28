// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.NullClientProxy
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Dynamic;
using System.Globalization;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class NullClientProxy : DynamicObject
  {
    public override bool TryGetMember(GetMemberBinder binder, out object result) => throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_UsingHubInstanceNotCreatedUnsupported));

    public override bool TryInvokeMember(
      InvokeMemberBinder binder,
      object[] args,
      out object result)
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_UsingHubInstanceNotCreatedUnsupported));
    }
  }
}
