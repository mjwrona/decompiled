// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ProvenanceTracepoints
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class ProvenanceTracepoints
  {
    public const string Area = "Provenance";
    public const string ServiceTraceLayer = "Service";
    public const int ServiceStartEnter = 10019500;
    public const int ServiceStartLeave = 10019501;
    public const int ServiceStartException = 10019502;
    public const int ServiceEndEnter = 10019503;
    public const int ServiceEndLeave = 10019504;
    public const int ServiceEndException = 10019505;
    public const int GetPackageVersionProvenanceEnter = 10019506;
    public const int GetPackageVersionProvenanceLeave = 10019507;
    public const int GetPackageVersionProvenanceException = 10019508;
  }
}
