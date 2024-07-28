// Decompiled with JetBrains decompiler
// Type: Nest.IRemoteSource
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IRemoteSource
  {
    [DataMember(Name = "host")]
    Uri Host { get; set; }

    [DataMember(Name = "password")]
    string Password { get; set; }

    [DataMember(Name = "username")]
    string Username { get; set; }

    [DataMember(Name = "socket_timeout")]
    Time SocketTimeout { get; set; }

    [DataMember(Name = "connect_timeout")]
    Time ConnectTimeout { get; set; }
  }
}
