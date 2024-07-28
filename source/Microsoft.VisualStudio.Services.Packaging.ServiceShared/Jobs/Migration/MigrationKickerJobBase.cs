// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration.MigrationKickerJobBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration
{
  public abstract class MigrationKickerJobBase : VssAsyncJobExtension
  {
    protected MigrationKickerRequest GetRequest(TeamFoundationJobDefinition jobDef)
    {
      if (jobDef.Data == null)
        throw new Exception(string.Format("Data is null for job definition with JobId {0}", (object) jobDef.JobId));
      return !string.IsNullOrWhiteSpace(jobDef.Data?.InnerText) ? TeamFoundationSerializationUtility.Deserialize<MigrationKickerRequest>(jobDef.Data) : new MigrationKickerRequest();
    }
  }
}
