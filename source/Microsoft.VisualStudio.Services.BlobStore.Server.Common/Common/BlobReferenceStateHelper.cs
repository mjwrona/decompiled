// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobReferenceStateHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class BlobReferenceStateHelper
  {
    public static void AssertValidTransition(IBlobMetadata metadata) => BlobReferenceStateHelper.AssertValidTransition(metadata.StoredReferenceState, metadata.DesiredReferenceState);

    public static void AssertValidTransition(
      BlobReferenceState oldState,
      BlobReferenceState newState)
    {
      if (!BlobReferenceStateHelper.IsValidTransition(oldState, newState))
        throw new ArgumentException(string.Format("Transitioning from {0} to {1} is invalid.", (object) oldState, (object) newState));
    }

    public static bool IsValidTransition(BlobReferenceState oldState, BlobReferenceState newState)
    {
      switch (oldState)
      {
        case BlobReferenceState.Empty:
          return newState == BlobReferenceState.AddingBlob;
        case BlobReferenceState.AddingBlob:
          return newState == BlobReferenceState.AddedBlob;
        case BlobReferenceState.AddedBlob:
          switch (newState)
          {
            case BlobReferenceState.AddedBlob:
            case BlobReferenceState.DeletingBlob:
              return true;
            default:
              return false;
          }
        case BlobReferenceState.DeletingBlob:
          return newState == BlobReferenceState.Empty || newState == BlobReferenceState.DeletingBlob;
        default:
          return false;
      }
    }
  }
}
