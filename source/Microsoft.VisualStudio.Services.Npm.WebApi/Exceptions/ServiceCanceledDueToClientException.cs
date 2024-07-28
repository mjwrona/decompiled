// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.ServiceCanceledDueToClientException
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions
{
  public class ServiceCanceledDueToClientException : VssServiceException
  {
    public ServiceCanceledDueToClientException()
      : base(string.Empty)
    {
    }

    public ServiceCanceledDueToClientException(Exception innerException)
      : base(string.Empty, innerException)
    {
    }
  }
}
