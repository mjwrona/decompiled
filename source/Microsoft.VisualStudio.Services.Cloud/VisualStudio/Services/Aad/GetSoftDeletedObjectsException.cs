// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetSoftDeletedObjectsException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Aad
{
  [Serializable]
  public class GetSoftDeletedObjectsException : AadGraphException
  {
    internal GetSoftDeletedObjectsException(string message)
      : base(message)
    {
    }

    internal GetSoftDeletedObjectsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
