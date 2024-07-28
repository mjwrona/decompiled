// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Exceptions.ProvenanceSessionNotFoundException
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 92A332A6-CB54-4593-8984-3FC6CEE8C30D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Exceptions
{
  public class ProvenanceSessionNotFoundException : VssServiceException
  {
    public ProvenanceSessionNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ProvenanceSessionNotFoundException(string message)
      : base(message)
    {
    }
  }
}
