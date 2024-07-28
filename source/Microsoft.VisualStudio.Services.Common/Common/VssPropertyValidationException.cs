// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssPropertyValidationException
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.VisualStudio.Services.Common
{
  [ExceptionMapping("0.0", "3.0", "VssPropertyValidationException", "Microsoft.VisualStudio.Services.Common.VssPropertyValidationException, Microsoft.VisualStudio.Services.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class VssPropertyValidationException : VssServiceException
  {
    public VssPropertyValidationException(string propertyName, string message)
      : base(message)
    {
      this.PropertyName = propertyName;
    }

    public VssPropertyValidationException(
      string propertyName,
      string message,
      Exception innerException)
      : base(message, innerException)
    {
      this.PropertyName = propertyName;
    }

    protected VssPropertyValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.PropertyName = info.GetString(nameof (PropertyName));
    }

    public string PropertyName { get; set; }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("PropertyName", (object) this.PropertyName);
    }
  }
}
