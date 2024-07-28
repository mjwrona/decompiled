// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.License.MissingLicenseException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.License
{
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Not required at this moment.")]
  [Serializable]
  public class MissingLicenseException : TeamFoundationServiceException
  {
    public MissingLicenseException(string featureNames)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(featureNames, nameof (featureNames));
      this.FeatureName = featureNames;
    }

    protected MissingLicenseException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    [SuppressMessage("Microsoft.Globalization", "CA1304:Specify CultureInfo", Justification = "Logs don't need be localized")]
    public override string Message => FrameworkResources.MissingLicenseException((object) this.FeatureName);

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);

    public string FeatureName { get; private set; }
  }
}
