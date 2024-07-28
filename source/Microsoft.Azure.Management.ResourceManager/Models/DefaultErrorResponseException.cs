// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DefaultErrorResponseException
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using System;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DefaultErrorResponseException : RestException
  {
    public HttpRequestMessageWrapper Request { get; set; }

    public HttpResponseMessageWrapper Response { get; set; }

    public DefaultErrorResponse Body { get; set; }

    public DefaultErrorResponseException()
    {
    }

    public DefaultErrorResponseException(string message)
      : this(message, (Exception) null)
    {
    }

    public DefaultErrorResponseException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
