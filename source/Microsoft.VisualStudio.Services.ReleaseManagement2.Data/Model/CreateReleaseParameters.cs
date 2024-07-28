// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.CreateReleaseParameters
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class CreateReleaseParameters
  {
    private PropertiesCollection properties;

    public int DefinitionId { get; set; }

    public string Description { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? CreatedFor { get; set; }

    public IList<ArtifactMetadata> ArtifactData { get; private set; }

    public bool IsDraft { get; set; }

    public ReleaseReason Reason { get; set; }

    public IList<string> ManualEnvironments { get; private set; }

    public IDictionary<string, ConfigurationVariableValue> Variables { get; private set; }

    public IList<ReleaseStartEnvironmentMetadata> EnvironmentsMetadata { get; private set; }

    public string TriggeringArtifactAlias { get; set; }

    public string Comment => !string.IsNullOrEmpty(this.Description) ? this.Description : (string) null;

    public CreateReleaseParameters()
    {
      this.ArtifactData = (IList<ArtifactMetadata>) new List<ArtifactMetadata>();
      this.ManualEnvironments = (IList<string>) new List<string>();
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.EnvironmentsMetadata = (IList<ReleaseStartEnvironmentMetadata>) new List<ReleaseStartEnvironmentMetadata>();
    }

    public void PopulateArtifactVersions(IList<ArtifactMetadata> artifactsMetadata)
    {
      if (artifactsMetadata == null)
        return;
      this.ArtifactData = (IList<ArtifactMetadata>) new List<ArtifactMetadata>((IEnumerable<ArtifactMetadata>) artifactsMetadata);
    }

    public PropertiesCollection Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = new PropertiesCollection();
        return this.properties;
      }
    }
  }
}
