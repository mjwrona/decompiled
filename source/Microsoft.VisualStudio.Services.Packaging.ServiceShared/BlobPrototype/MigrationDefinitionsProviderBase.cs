// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationDefinitionsProviderBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public abstract class MigrationDefinitionsProviderBase : IMigrationDefinitionsProvider
  {
    public abstract IEnumerable<MigrationDefinition> GetMigrations();

    public MigrationDefinition GetMigration(string migrationName, IProtocol protocol) => this.GetMigrations().SingleOrDefault<MigrationDefinition>((Func<MigrationDefinition, bool>) (m => m.Name.Equals(migrationName, StringComparison.OrdinalIgnoreCase))).ThrowIfNull<MigrationDefinition>((Func<Exception>) (() => (Exception) new InvalidMigrationException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_MigrationNotFound((object) migrationName))));

    public abstract MigrationDefinition GetDefaultMigration(IProtocol protocol, FeedId feedId);
  }
}
