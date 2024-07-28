// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.InvalidArtifactTypeException
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [Serializable]
  public class InvalidArtifactTypeException : TeamFoundationServerException
  {
    public InvalidArtifactTypeException()
    {
    }

    public InvalidArtifactTypeException(string message)
      : base(message)
    {
    }

    public InvalidArtifactTypeException(string message, Exception exception)
      : base(message, exception)
    {
    }
  }
}
