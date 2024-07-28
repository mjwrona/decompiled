// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.ExceptionCiEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  [ClientSupportedCiEvent]
  public class ExceptionCiEvent : CiEvent
  {
    public const string ErrorMessageParam = "ErrorMessage";
    public const string ActivityParam = "ActivityId";

    public ExceptionCiEvent(string feature)
      : base(CustomerIntelligenceArea.Exceptions, feature)
    {
    }

    public ExceptionCiEvent(string feature, string errorMessage)
      : base(CustomerIntelligenceArea.Exceptions, feature)
    {
      this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage
    {
      get
      {
        string errorMessage;
        this.Properties.TryGetValue<string>(nameof (ErrorMessage), out errorMessage);
        return errorMessage;
      }
      set => this.Properties[nameof (ErrorMessage)] = (object) value;
    }

    public Guid ActivityId
    {
      get
      {
        Guid activityId;
        this.Properties.TryGetValue<Guid>(nameof (ActivityId), out activityId);
        return activityId;
      }
      set => this.Properties[nameof (ActivityId)] = (object) value;
    }
  }
}
