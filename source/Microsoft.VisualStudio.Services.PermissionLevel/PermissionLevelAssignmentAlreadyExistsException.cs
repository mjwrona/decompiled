// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelAssignmentAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [Serializable]
  public class PermissionLevelAssignmentAlreadyExistsException : PermissionLevelAssignmentException
  {
    public PermissionLevelAssignmentAlreadyExistsException()
    {
    }

    public PermissionLevelAssignmentAlreadyExistsException(string message)
      : base(message)
    {
    }

    public PermissionLevelAssignmentAlreadyExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected PermissionLevelAssignmentAlreadyExistsException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
