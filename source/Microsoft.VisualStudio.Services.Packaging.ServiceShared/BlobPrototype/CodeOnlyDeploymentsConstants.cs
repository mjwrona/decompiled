// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CodeOnlyDeploymentsConstants
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class CodeOnlyDeploymentsConstants
  {
    public static readonly string CodeOnlyEnabledPrefix = "/Configuration/Packaging/CodeOnly";
    public static readonly string ReadMigrationHeaderQuery = "/Configuration/Packaging/CodeOnly/ReadMigrationHeader";
    public static readonly string ReadMigrationHeader = "X-PACKAGING-MIGRATION";
    public static readonly string ReadMigrationHeaderKey = "Packaging.ReadMigrationHeader";
    public static readonly string DefaultMigrationQueryPrefix = "/Configuration/Packaging/CodeOnly/DefaultMigration";
    public static readonly string CodeOnlyContainerPrefix = "p-";
  }
}
