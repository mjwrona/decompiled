// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.IdentityDirectoryWrapperReadCountMismatchException
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
  public class IdentityDirectoryWrapperReadCountMismatchException : IdentityDirectoryWrapperException
  {
    public IdentityDirectoryWrapperReadCountMismatchException()
    {
    }

    public IdentityDirectoryWrapperReadCountMismatchException(string message)
      : base(message)
    {
    }

    public IdentityDirectoryWrapperReadCountMismatchException(
      string message,
      Exception innerException)
      : base(message, innerException)
    {
    }

    public IdentityDirectoryWrapperReadCountMismatchException(
      IReadOnlyList<DirectoryEntityResult> entityResults,
      IReadOnlyList<IVssIdentity> identities)
    {
    }

    public IdentityDirectoryWrapperReadCountMismatchException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
