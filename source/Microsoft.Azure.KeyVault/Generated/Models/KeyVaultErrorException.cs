// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyVaultErrorException
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using System;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeyVaultErrorException : RestException
  {
    public HttpRequestMessageWrapper Request { get; set; }

    public HttpResponseMessageWrapper Response { get; set; }

    public KeyVaultError Body { get; set; }

    public KeyVaultErrorException()
    {
    }

    public KeyVaultErrorException(string message)
      : this(message, (Exception) null)
    {
    }

    public KeyVaultErrorException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
