// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingApiController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class LicensingApiController : TfsApiController
  {
    protected const string s_area = "VisualStudio.Services.LicensingService";

    public override string TraceArea => "VisualStudio.Services.LicensingService";

    public override string ActivityLogArea => "Licensing";

    protected static SubjectDescriptor GetUserSubjectDescriptor(IVssRequestContext requestContext)
    {
      SubjectDescriptor subjectDescriptor = requestContext.GetUserIdentity().Descriptor.ToSubjectDescriptor(requestContext);
      return subjectDescriptor.IsClaimsUserType() ? subjectDescriptor : throw new ArgumentException(string.Format("Unsupported subject descriptor {0}.", (object) subjectDescriptor));
    }
  }
}
