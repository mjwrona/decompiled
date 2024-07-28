// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.NpmSharedPackageMetadata
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class NpmSharedPackageMetadata
  {
    public string Revision { get; set; }

    public IDictionary<string, string> DistributionTags { get; set; }
  }
}
