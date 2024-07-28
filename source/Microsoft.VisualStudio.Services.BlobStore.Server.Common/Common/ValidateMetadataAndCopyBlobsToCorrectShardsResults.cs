// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.ValidateMetadataAndCopyBlobsToCorrectShardsResults
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  [DataContract]
  public class ValidateMetadataAndCopyBlobsToCorrectShardsResults
  {
    [DataMember]
    public long MetadataEnumerated;
    [DataMember]
    public long ExceptionsThrown;
    [DataMember]
    public long MetadataChecked;
    [DataMember]
    public long MetadataOnWrongShard;
    [DataMember]
    public long MetadataCopiedToCorrectShard;
    [DataMember]
    public long MetadataCopiedToCorrectShardAfterReupload;
    [DataMember]
    public long MetadataFailedToAddReferencesEvenAfterReupload;
    [DataMember]
    public long MetadataFailedToAddReferencesReuploadFailed;
    [DataMember]
    public long BlobsChecked;
    [DataMember]
    public long BlobIsMissing;
    [DataMember]
    public long BlobEtagMismatch;
    [DataMember]
    public long BlobFoundInDifferentShard;
    [DataMember]
    public long BlobCopiedToCorrectShard;
  }
}
