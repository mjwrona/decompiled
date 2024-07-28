// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.ClientAssemblyMigrationSerializationBinder
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  internal class ClientAssemblyMigrationSerializationBinder : DefaultSerializationBinder
  {
    private readonly ClientAssemblyMigration[] migrations;

    public ClientAssemblyMigrationSerializationBinder(ClientAssemblyMigration[] migrations) => this.migrations = migrations;

    public override Type BindToType(string assemblyName, string typeName)
    {
      ClientAssemblyMigration assemblyMigration = ((IEnumerable<ClientAssemblyMigration>) this.migrations).SingleOrDefault<ClientAssemblyMigration>((Func<ClientAssemblyMigration, bool>) (p => p.FromAssembly == assemblyName && p.FromType == typeName));
      return assemblyMigration != null ? assemblyMigration.ToType : base.BindToType(assemblyName, typeName);
    }
  }
}
