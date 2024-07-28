// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DestroyedContentUnavailableException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class DestroyedContentUnavailableException : ServerException
  {
    public DestroyedContentUnavailableException()
      : base(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get(nameof (DestroyedContentUnavailableException)))
    {
      this.LogException = false;
    }

    public DestroyedContentUnavailableException(string message)
      : base(message)
    {
      this.LogException = false;
    }
  }
}
