// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamFieldValue
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [DataContract]
  public class TeamFieldValue : ITeamFieldValue
  {
    private const int TeamFieldMaxLength = 256;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    [DataMember(Name = "value", EmitDefaultValue = false)]
    public string Value { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    [DataMember(Name = "includeChildren", EmitDefaultValue = false)]
    public bool IncludeChildren { get; set; }

    public void Validate()
    {
      ArgumentUtility.CheckForNull<string>(this.Value, "Value");
      if (this.Value.Length > 256)
        throw new TeamFoundationValidationException(Resources.Validation_InvalidTeamFieldLength, "Value");
    }
  }
}
