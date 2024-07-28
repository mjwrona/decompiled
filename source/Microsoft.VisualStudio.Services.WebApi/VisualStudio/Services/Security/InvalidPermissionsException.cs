// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.InvalidPermissionsException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Security
{
  [Serializable]
  public class InvalidPermissionsException : SecurityException
  {
    public InvalidPermissionsException(Guid namespaceId, int bitmask)
      : this(SecurityResources.InvalidPermissionsException((object) namespaceId, (object) bitmask))
    {
    }

    public InvalidPermissionsException(string message)
      : base(message)
    {
    }

    public InvalidPermissionsException(string message, Exception ex)
      : base(message, ex)
    {
    }
  }
}
