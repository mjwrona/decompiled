// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.IdentityDirectoryWrapperReadFailedException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Directories
{
  [Serializable]
  public class IdentityDirectoryWrapperReadFailedException : IdentityDirectoryWrapperException
  {
    public IdentityDirectoryWrapperReadFailedException()
    {
    }

    public IdentityDirectoryWrapperReadFailedException(string message)
      : base(message)
    {
    }

    public IdentityDirectoryWrapperReadFailedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public IdentityDirectoryWrapperReadFailedException(
      IReadOnlyList<DirectoryEntityResult> entityResults,
      IReadOnlyList<IVssIdentity> identities)
    {
    }

    public IdentityDirectoryWrapperReadFailedException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
