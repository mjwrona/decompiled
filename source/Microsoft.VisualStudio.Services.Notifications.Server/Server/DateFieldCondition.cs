// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DateFieldCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class DateFieldCondition : FieldCondition
  {
    private Date m_target;

    public DateFieldCondition()
    {
    }

    public DateFieldCondition(Token fieldName, byte op, Date target)
    {
      this.FieldName = fieldName;
      this.m_operation = op;
      this.m_target = target;
    }

    public override bool EvaluateOneValue(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      object fieldValue)
    {
      ++evaluationContext.EvaluationsCount;
      this.ToString();
      if (!(fieldValue is DateTime result) && (!(fieldValue is string s) || !DateTime.TryParse(s, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result)))
        throw new InvalidFieldValueException(CoreRes.EventConditionFieldNotFound((object) this.FieldName));
      DateTime dateTime = this.m_target.GetDateTime();
      switch (this.m_operation)
      {
        case 8:
          return result.Date.CompareTo(dateTime.Date) < 0;
        case 9:
          return result.Date.CompareTo(dateTime.Date) > 0;
        case 10:
          return result.Date.CompareTo(dateTime.Date) <= 0;
        case 11:
          return result.Date.CompareTo(dateTime.Date) >= 0;
        case 12:
          return result.Date.Equals(dateTime.Date);
        case 13:
          return !result.Date.Equals(dateTime.Date);
        default:
          throw new InvalidOperationException();
      }
    }

    public override string GetOperandString() => this.m_target.ToString();

    public override bool Equals(object obj) => obj is DateFieldCondition dateFieldCondition && this.FieldName == dateFieldCondition.FieldName && this.m_target == dateFieldCondition.m_target && (int) this.m_operation == (int) dateFieldCondition.Operation;

    public override int GetHashCode() => this.FieldName.GetHashCode() ^ this.m_target.GetHashCode() ^ this.m_operation.GetHashCode();
  }
}
