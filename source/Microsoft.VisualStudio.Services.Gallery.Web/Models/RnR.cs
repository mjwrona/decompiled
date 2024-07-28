// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.RnR
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  [DataContract]
  public class RnR
  {
    [DataMember]
    public int RatingCount;
    [DataMember]
    public double Rating;
    [DataMember]
    public string BannerAverageRatingText;
    [DataMember]
    public string RatingsScreenReaderText;
    [DataMember]
    public string ResourcesPath;
    [DataMember]
    public string StarType;
  }
}
