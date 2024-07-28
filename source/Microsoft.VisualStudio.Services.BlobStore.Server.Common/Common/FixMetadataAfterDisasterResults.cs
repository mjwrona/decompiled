// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.FixMetadataAfterDisasterResults
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  [DataContract]
  public class FixMetadataAfterDisasterResults
  {
    [DataMember]
    public long MetadataEnumerated;
    [DataMember]
    public long ExceptionsThrown;
    [DataMember]
    public long MissingBlob;
    [DataMember]
    public long RemovedMetadata;
    [DataMember]
    public long FailedToRemoveMetadata;
    [DataMember]
    public long BlobsPassed;
    [DataMember]
    public long BlobEtagMismatch;
  }
}
