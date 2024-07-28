// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.HttpInvalidLicenseException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class HttpInvalidLicenseException : HttpException
  {
    public HttpInvalidLicenseException(string featureNames)
      : base(403, string.Format((IFormatProvider) CultureInfo.InvariantCulture, WACommonResources.InvalidLicenseException, (object) featureNames))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(featureNames, nameof (featureNames));
      this.FeatureName = featureNames;
    }

    private HttpInvalidLicenseException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public string FeatureName { get; private set; }
  }
}
