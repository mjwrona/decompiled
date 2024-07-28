// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator.FeatureRequirement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator
{
  public class FeatureRequirement
  {
    public string FeatureName { get; private set; }

    public FeatureMetastateRequirement Metastate { get; private set; }

    public IEnumerable<string> TypeFields { get; private set; }

    public IEnumerable<string> Fields { get; private set; }

    public string HardCodedCategory { get; private set; }

    public IDictionary<string, string> HardCodedMetastateMap { get; private set; }

    public FeatureRequirement(
      string featureName,
      FeatureMetastateRequirement metastate = FeatureMetastateRequirement.None,
      IEnumerable<string> typeFields = null,
      IEnumerable<string> fields = null,
      string hardcodedCategory = null,
      IDictionary<string, string> hardcodedMetastateMap = null)
    {
      this.FeatureName = featureName;
      this.Metastate = metastate;
      this.TypeFields = typeFields ?? Enumerable.Empty<string>();
      this.Fields = fields ?? Enumerable.Empty<string>();
      this.HardCodedCategory = hardcodedCategory;
      this.HardCodedMetastateMap = hardcodedMetastateMap ?? (IDictionary<string, string>) new Dictionary<string, string>();
    }
  }
}
