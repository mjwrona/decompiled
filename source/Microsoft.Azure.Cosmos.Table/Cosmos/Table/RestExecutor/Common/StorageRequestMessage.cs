// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.StorageRequestMessage
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth;
using System;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
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
