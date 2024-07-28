// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages.ProblemPackagesDocumentProcessor
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages
{
  public class ProblemPackagesDocumentProcessor : 
    IAggregationDocumentProcessor<ProblemPackagesFile>,
    IEmptyDocumentProvider<ProblemPackagesFile>
  {
    private readonly Func<string, IPackageName> nameParser;
    private readonly Func<string, IPackageVersion> versionParser;

    public ProblemPackagesDocumentProcessor(
      Func<string, IPackageName> nameParser,
      Func<string, IPackageVersion> versionParser)
    {
      this.nameParser = nameParser;
      this.versionParser = versionParser;
    }

    public ProblemPackagesFile GetEmptyDocument() => ProblemPackagesFile.Empty;

    public ProblemPackagesFile Deserialize(byte[] buffer) => JsonUtilities.Deserialize<ProblemPackagesFile.Stored>(Encoding.UTF8.GetString(buffer), true).Unpack(this.nameParser, this.versionParser);

    public byte[] Serialize(ProblemPackagesFile doc) => Encoding.UTF8.GetBytes(doc.Pack().Serialize<ProblemPackagesFile.Stored>(true));

    public void NotifySaved(ProblemPackagesFile doc)
    {
    }
  }
}
