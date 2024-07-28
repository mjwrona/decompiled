// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions.ClientToolNotFoundException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions
{
  public class ClientToolNotFoundException : VssServiceException
  {
    private ClientToolNotFoundException(string message)
      : base(message)
    {
    }

    public static ClientToolNotFoundException Create() => new ClientToolNotFoundException(Resources.ClientToolNoMatchingReleaseFound());

    public static ClientToolNotFoundException Create(
      string toolName,
      string osName,
      string architecture)
    {
      return new ClientToolNotFoundException(Resources.ClientToolNotFound((object) toolName, (object) osName, (object) architecture));
    }
  }
}
