// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestCaseResultIdAndRevConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestCaseResultIdAndRevConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev idAndRev)
    {
      if (idAndRev == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev()
      {
        Id = TestCaseResultIdentifierConverter.Convert(idAndRev.Id),
        Revision = idAndRev.Revision
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev idAndRev)
    {
      if (idAndRev == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev()
      {
        Id = TestCaseResultIdentifierConverter.Convert(idAndRev.Id),
        Revision = idAndRev.Revision
      };
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> idAndRevs)
    {
      return idAndRevs == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>) null : idAndRevs.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>) (idAndRev => TestCaseResultIdAndRevConverter.Convert(idAndRev)));
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev> idAndRevs)
    {
      return idAndRevs == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>) null : idAndRevs.Select<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>) (idAndRev => TestCaseResultIdAndRevConverter.Convert(idAndRev)));
    }
  }
}
