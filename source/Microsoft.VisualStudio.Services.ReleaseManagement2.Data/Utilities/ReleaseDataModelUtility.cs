// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDataModelUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseDataModelUtility
  {
    private const string ReleaseVariableStrongBoxDrawerFormat = "/Service/ReleaseManagement/{0}/Releases/{1}";

    public static string GetDrawerName(Guid projectIdentifier, int releaseId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/ReleaseManagement/{0}/Releases/{1}", (object) projectIdentifier, (object) releaseId);
  }
}
