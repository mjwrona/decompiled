// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationValidationException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class TeamFoundationValidationException : TeamFoundationServiceException
  {
    public TeamFoundationValidationException(string message, string propertyName)
      : base(message)
    {
      this.EventId = TeamFoundationEventId.TeamFoundationValidationException;
      this.PropertyName = propertyName;
    }

    public TeamFoundationValidationException(
      string message,
      string propertyName,
      Exception innerException)
      : base(message, innerException)
    {
      this.EventId = TeamFoundationEventId.TeamFoundationValidationException;
      this.PropertyName = propertyName;
    }

    public string PropertyName { get; set; }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      info.AddValue("PropertyName", (object) this.PropertyName);
      base.GetObjectData(info, context);
    }
  }
}
