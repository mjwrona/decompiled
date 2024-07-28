// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.TestContributionExtension
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class TestContributionExtension
  {
    public const string TestManagerAdvancedPublisherName = "ms";
    public const string TestManagerAdvancedExtensionName = "vss-testmanager-web";

    public static string GetExtensionFullName() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}.{1}", (object) "ms", (object) "vss-testmanager-web");
  }
}
