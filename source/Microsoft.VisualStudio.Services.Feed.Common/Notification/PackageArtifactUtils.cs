// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.Notification.PackageArtifactUtils
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Common.Notification
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PackageArtifactUtils
  {
    public static string GetArtifactUriForPackage(string fullyQualifiedFeedId, Guid packageId) => LinkingUtilities.EncodeUri(PackageArtifactUtils.GetArtifactIdForPackage(fullyQualifiedFeedId, packageId));

    public static ArtifactId GetArtifactIdForPackage(string fullyQualifiedFeedId, Guid packageId)
    {
      if (string.IsNullOrWhiteSpace(fullyQualifiedFeedId))
        throw new ArgumentNullException(nameof (fullyQualifiedFeedId));
      PackageArtifactUtils.ValidateFullyQualifiedFeedId(fullyQualifiedFeedId);
      return new ArtifactId("PackageManagement", "Package", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) fullyQualifiedFeedId, (object) packageId));
    }

    public static bool TryDecode(
      ArtifactId artifactId,
      out Guid feedId,
      out Guid viewId,
      out Guid packageId)
    {
      try
      {
        PackageArtifactUtils.DecodeArtifactUri(UriUtility.UrlDecode(artifactId.ToolSpecificId), out feedId, out viewId, out packageId);
        return true;
      }
      catch (Exception ex) when (ex is ArgumentException || ex is NullReferenceException)
      {
        feedId = new Guid();
        viewId = new Guid();
        packageId = new Guid();
        return false;
      }
    }

    private static void ValidateFullyQualifiedFeedId(string fullyQualifiedFeedId)
    {
      string[] source = fullyQualifiedFeedId.Split(new char[1]
      {
        '@'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (source.Length < 1 || source.Length > 2)
        throw new ArgumentException(Resources.Error_FeedIdIsInvalid((object) fullyQualifiedFeedId), nameof (fullyQualifiedFeedId));
      if (((IEnumerable<string>) source).Any<string>((Func<string, bool>) (c => !Guid.TryParse(c, out Guid _))))
        throw new ArgumentException(Resources.Error_FeedIdIsInvalid((object) fullyQualifiedFeedId), nameof (fullyQualifiedFeedId));
    }

    private static void DecodeArtifactUri(
      string artifactDetails,
      out Guid feedId,
      out Guid viewId,
      out Guid packageId)
    {
      string[] strArray1 = artifactDetails.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      string[] strArray2 = strArray1.Length == 2 ? strArray1[0].Split(new char[1]
      {
        '@'
      }, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentException(Resources.Error_InvalidPackageArtifactUri((object) artifactDetails), nameof (artifactDetails));
      if (strArray2.Length == 0 || strArray2.Length > 2)
        throw new ArgumentException(Resources.Error_InvalidPackageArtifactUri((object) artifactDetails), nameof (artifactDetails));
      if (!Guid.TryParse(strArray2[0], out feedId))
        throw new ArgumentException(Resources.Error_InvalidPackageArtifactUri((object) artifactDetails), nameof (artifactDetails));
      if (strArray2.Length == 2)
      {
        if (!Guid.TryParse(strArray2[1], out viewId))
          throw new ArgumentException(Resources.Error_InvalidPackageArtifactUri((object) artifactDetails), nameof (artifactDetails));
      }
      else
        viewId = new Guid();
      if (!Guid.TryParse(strArray1[1], out packageId))
        throw new ArgumentException(Resources.Error_InvalidPackageArtifactUri((object) artifactDetails), artifactDetails);
    }
  }
}
