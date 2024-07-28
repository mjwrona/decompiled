// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageNotFoundException
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions
{
  [Obsolete("Use Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException")]
  public class PackageNotFoundException : Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException
  {
    public PackageNotFoundException(string message)
      : base(message)
    {
    }

    public PackageNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected PackageNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
