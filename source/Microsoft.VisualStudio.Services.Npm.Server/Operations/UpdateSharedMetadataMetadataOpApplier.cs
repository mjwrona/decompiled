// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Operations.UpdateSharedMetadataMetadataOpApplier
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.Operations
{
  public class UpdateSharedMetadataMetadataOpApplier : 
    IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>
  {
    private readonly IExecutionEnvironment executionEnv;
    private readonly IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>> delegatedApplier;

    public UpdateSharedMetadataMetadataOpApplier(
      IExecutionEnvironment executionEnv,
      IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>> delegatedApplier)
    {
      this.executionEnv = executionEnv;
      this.delegatedApplier = delegatedApplier;
    }

    public INpmMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      INpmMetadataEntry currentState,
      MetadataDocument<INpmMetadataEntry> doc)
    {
      INpmMetadataEntry npmMetadataEntry = this.delegatedApplier.Apply(commitLogEntry, currentState);
      if (doc.Properties == null)
        doc.Properties = (IMetadataDocumentProperties) new MetadataDocumentProperties();
      IMetadataDocumentPropertiesWriteable properties = (IMetadataDocumentPropertiesWriteable) doc.Properties;
      IDictionary<string, string> distTags = (properties?.NameMetadata is NpmLocalAndUpstreamSharedPackageMetadata nameMetadata ? nameMetadata.LocalDistributionTags : (IDictionary<string, string>) null) ?? (IDictionary<string, string>) new Dictionary<string, string>();
      IDictionary<string, string> dictionary1 = nameMetadata?.UpstreamDistributionTags ?? (IDictionary<string, string>) new Dictionary<string, string>();
      IDictionary<string, string> dictionary2 = this.UpdateDistTags(commitLogEntry, doc, distTags);
      properties.NameMetadata = (object) new NpmLocalAndUpstreamSharedPackageMetadata()
      {
        LocalDistributionTags = dictionary2,
        UpstreamDistributionTags = dictionary1,
        Revision = Guid.NewGuid().ToString("N")
      };
      ICommitLogEntry commitEntry = commitLogEntry;
      return (INpmMetadataEntry) npmMetadataEntry.CreateWriteable((ICommitLogEntryHeader) commitEntry);
    }

    private IDictionary<string, string> UpdateDistTags(
      ICommitLogEntry commitLogEntry,
      MetadataDocument<INpmMetadataEntry> doc,
      IDictionary<string, string> distTags)
    {
      if (this.executionEnv.IsHostProcessType(HostProcessType.ApplicationTier) || !(commitLogEntry.CommitOperationData is INpmAddOperationData commitOperationData))
        return distTags;
      bool flag1 = distTags.ContainsKey("latest");
      bool flag2 = !commitOperationData.DistTag.IsNullOrEmpty<char>() && commitOperationData.DistTag != "latest";
      if (flag2)
        distTags[commitOperationData.DistTag] = commitOperationData.Identity.Version.ToString();
      if (!commitOperationData.SourceChain.Any<UpstreamSourceInfo>() && (!flag1 || !flag2))
        distTags["latest"] = commitOperationData.Identity.Version.ToString();
      return distTags;
    }
  }
}
