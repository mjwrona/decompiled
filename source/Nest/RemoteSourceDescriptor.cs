// Decompiled with JetBrains decompiler
// Type: Nest.RemoteSourceDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RemoteSourceDescriptor : 
    DescriptorBase<RemoteSourceDescriptor, IRemoteSource>,
    IRemoteSource
  {
    Uri IRemoteSource.Host { get; set; }

    string IRemoteSource.Password { get; set; }

    string IRemoteSource.Username { get; set; }

    Time IRemoteSource.SocketTimeout { get; set; }

    Time IRemoteSource.ConnectTimeout { get; set; }

    public RemoteSourceDescriptor Host(Uri host) => this.Assign<Uri>(host, (Action<IRemoteSource, Uri>) ((a, v) => a.Host = v));

    public RemoteSourceDescriptor Username(string username) => this.Assign<string>(username, (Action<IRemoteSource, string>) ((a, v) => a.Username = v));

    public RemoteSourceDescriptor Password(string password) => this.Assign<string>(password, (Action<IRemoteSource, string>) ((a, v) => a.Password = v));

    public RemoteSourceDescriptor SocketTimeout(Time socketTimeout) => this.Assign<Time>(socketTimeout, (Action<IRemoteSource, Time>) ((a, v) => a.SocketTimeout = v));

    public RemoteSourceDescriptor ConnectTimeout(Time connectTimeout) => this.Assign<Time>(connectTimeout, (Action<IRemoteSource, Time>) ((a, v) => a.ConnectTimeout = v));
  }
}
