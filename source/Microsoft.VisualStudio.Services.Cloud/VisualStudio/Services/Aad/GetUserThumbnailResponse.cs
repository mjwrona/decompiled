// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetUserThumbnailResponse
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetUserThumbnailResponse : AadServiceResponse
  {
    public byte[] Thumbnail { get; set; }

    public override string CompareAndGetDifference(AadServiceResponse anotherResponse) => !(anotherResponse is GetUserThumbnailResponse thumbnailResponse) ? string.Format("Target response is null or not of type '{0}'.", (object) typeof (GetUserThumbnailResponse)) : AadObjectCompareHelpers.CompareUserThumbnail(this.Thumbnail, thumbnailResponse.Thumbnail);
  }
}
