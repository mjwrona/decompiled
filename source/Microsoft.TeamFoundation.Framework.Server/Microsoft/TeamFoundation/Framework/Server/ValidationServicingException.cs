// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ValidationServicingException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class ValidationServicingException : TeamFoundationServicingException
  {
    public ValidationServicingException(
      string servicingOperation,
      params Exception[] validationExceptions)
      : base(ValidationServicingException.GenerateMessage(servicingOperation, validationExceptions))
    {
      this.ValidationExceptions = validationExceptions;
    }

    protected ValidationServicingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public Exception[] ValidationExceptions { get; protected set; }

    private static string GenerateMessage(
      string servicingOperation,
      Exception[] validationExceptions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format("Errors encountered during Validation operation '{0}'", (object) servicingOperation));
      stringBuilder.AppendLine(string.Format("{0} validation exceptions. (Check ValidationExceptions properties for full details).", (object) validationExceptions.Length));
      stringBuilder.AppendLine();
      int num = 0;
      foreach (Exception validationException in validationExceptions)
      {
        ++num;
        stringBuilder.AppendLine(string.Format("Validation Exception Message {0}:", (object) num));
        stringBuilder.AppendLine(validationException.Message);
        if (num < validationExceptions.Length)
          stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }
  }
}
