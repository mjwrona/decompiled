// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.StorageRequestMessage
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core.Auth;
using System;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Core
{
  internal sealed class StorageRequestMessage : HttpRequestMessage
  {
    public ICanonicalizer Canonicalizer { get; private set; }

    public StorageCredentials Credentials { get; private set; }

    public string AccountName { get; private set; }

    public StorageRequestMessage(
      HttpMethod method,
      Uri requestUri,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      string accountName)
      : base(method, requestUri)
    {
      this.Canonicalizer = canonicalizer;
      this.Credentials = credentials;
      this.AccountName = accountName;
    }
  }
}
