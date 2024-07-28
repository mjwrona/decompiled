// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.RuleValidationException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  [Serializable]
  public class RuleValidationException : WorkItemTrackingServiceException
  {
    public RuleValidationException(
      string fieldReferenceName,
      string fieldName,
      FieldStatusFlags status)
      : base(ServerResources.RuleError((object) fieldName, (object) status))
    {
      this.FieldReferenceName = fieldReferenceName;
      this.ErrorCode = 600171;
      this.FieldStatusFlags = status;
    }

    public RuleValidationException(string fieldReferenceName, string fieldName)
      : base(ServerResources.RuleEvaluationFailed((object) fieldName))
    {
      this.FieldReferenceName = fieldReferenceName;
      this.ErrorCode = 600171;
    }

    public RuleValidationException(
      string fieldReferenceName,
      string fieldName,
      FieldStatusFlags status,
      object value)
      : base(RuleValidationException.GetDetailedMessage(fieldName, status, value))
    {
      this.FieldReferenceName = fieldReferenceName;
      this.ErrorCode = 600171;
      this.FieldStatusFlags = status;
    }

    public RuleValidationException(string message, IEnumerable<RuleValidationException> exceptions)
      : base(message)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) exceptions, nameof (exceptions));
      this.RuleValidationErrors = exceptions;
    }

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public string FieldReferenceName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public FieldStatusFlags FieldStatusFlags { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public string ErrorMessage => this.Message;

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public int FieldStatusCode => (int) this.FieldStatusFlags;

    [DataMember(EmitDefaultValue = false)]
    [JsonProperty]
    public IEnumerable<RuleValidationException> RuleValidationErrors { get; private set; }

    private static string GetDetailedMessage(
      string fieldName,
      FieldStatusFlags status,
      object value)
    {
      return (status & FieldStatusFlags.InvalidListValue) != FieldStatusFlags.None ? ServerResources.InvalidListRuleError((object) fieldName, value) : ServerResources.RuleError((object) fieldName, (object) status);
    }
  }
}
