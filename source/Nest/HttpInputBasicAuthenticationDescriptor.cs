// Decompiled with JetBrains decompiler
// Type: Nest.HttpInputBasicAuthenticationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HttpInputBasicAuthenticationDescriptor : 
    DescriptorBase<HttpInputBasicAuthenticationDescriptor, IHttpInputBasicAuthentication>,
    IHttpInputBasicAuthentication
  {
    string IHttpInputBasicAuthentication.Password { get; set; }

    string IHttpInputBasicAuthentication.Username { get; set; }

    public HttpInputBasicAuthenticationDescriptor Username(string username) => this.Assign<string>(username, (Action<IHttpInputBasicAuthentication, string>) ((a, v) => a.Username = v));

    public HttpInputBasicAuthenticationDescriptor Password(string password) => this.Assign<string>(password, (Action<IHttpInputBasicAuthentication, string>) ((a, v) => a.Password = v));
  }
}
