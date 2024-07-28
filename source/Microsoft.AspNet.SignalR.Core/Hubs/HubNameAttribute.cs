// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubNameAttribute
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Hubs
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class HubNameAttribute : Attribute
  {
    public HubNameAttribute(string hubName) => this.HubName = !string.IsNullOrEmpty(hubName) ? hubName : throw new ArgumentNullException(nameof (hubName));

    public string HubName { get; private set; }
  }
}
