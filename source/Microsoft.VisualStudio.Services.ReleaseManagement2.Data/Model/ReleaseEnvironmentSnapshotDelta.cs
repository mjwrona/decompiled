// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentSnapshotDelta
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseEnvironmentSnapshotDelta
  {
    private const string StrongBoxLookupKeyPrefixFormat = "Environments/{0}/Deployments/{1}/Variables";

    public ReleaseEnvironmentSnapshotDelta() => this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public ReleaseEnvironmentSnapshotDelta(
      int releaseId,
      int releaseEnvironmentId,
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      this.ReleaseId = releaseId;
      this.ReleaseEnvironmentId = releaseEnvironmentId;
      if (variables.IsNullOrEmpty<KeyValuePair<string, ConfigurationVariableValue>>())
        this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      else
        this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>(variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public int ReleaseId { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public int DeploymentId { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Reviewed")]
    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentGroupPhaseDelta> DeploymentGroupPhaseDelta { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Serializer uses setter")]
    public IDictionary<string, ConfigurationVariableValue> Variables { get; set; }

    public static string GetSecretVariableLookupPrefix(int releaseEnvironmentId, int deploymentId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Environments/{0}/Deployments/{1}/Variables", (object) releaseEnvironmentId, (object) deploymentId);
  }
}
