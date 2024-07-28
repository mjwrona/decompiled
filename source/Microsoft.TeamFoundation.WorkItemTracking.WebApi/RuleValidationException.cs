// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.RuleValidationException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi
{
  [DataContract]
  [JsonObject]
  public class RuleValidationException : VssServiceException
  {
    public RuleValidationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public IEnumerable<RuleValidationException> RuleValidationErrors { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public string FieldReferenceName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public FieldStatusFlags FieldStatusFlags { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public string ErrorMessage { get; set; }
  }
}
