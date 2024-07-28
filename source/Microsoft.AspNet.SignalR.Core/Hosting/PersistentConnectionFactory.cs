// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hosting.PersistentConnectionFactory
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Globalization;

namespace Microsoft.AspNet.SignalR.Hosting
{
  public class PersistentConnectionFactory
  {
    private readonly IDependencyResolver _resolver;

    public PersistentConnectionFactory(IDependencyResolver resolver) => this._resolver = resolver != null ? resolver : throw new ArgumentNullException(nameof (resolver));

    public PersistentConnection CreateInstance(Type connectionType)
    {
      if (connectionType == (Type) null)
        throw new ArgumentNullException(nameof (connectionType));
      if (!((this._resolver.Resolve(connectionType) ?? Activator.CreateInstance(connectionType)) is PersistentConnection instance))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IsNotA, new object[2]
        {
          (object) connectionType.FullName,
          (object) typeof (PersistentConnection).FullName
        }));
      return instance;
    }
  }
}
