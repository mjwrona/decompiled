// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.IRequest
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR
{
  public interface IRequest
  {
    Uri Url { get; }

    string LocalPath { get; }

    INameValueCollection QueryString { get; }

    INameValueCollection Headers { get; }

    IDictionary<string, Cookie> Cookies { get; }

    IPrincipal User { get; }

    IDictionary<string, object> Environment { get; }

    Task<INameValueCollection> ReadForm();
  }
}
