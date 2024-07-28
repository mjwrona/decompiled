// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.PackageNotFoundException
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  public class PackageNotFoundException : VssServiceException
  {
    public PackageNotFoundException(string message)
      : base(message)
    {
    }

    public PackageNotFoundException(string protocolType, string packageName, string feedId)
      : base(Resources.Error_PackageNotFoundMessage((object) protocolType, (object) packageName, (object) feedId))
    {
    }

    public static PackageNotFoundException Create(string packageId, string feedId) => new PackageNotFoundException(Resources.Error_PackageNotFoundByIdInFeedIdMessage((object) packageId, (object) feedId));
  }
}
