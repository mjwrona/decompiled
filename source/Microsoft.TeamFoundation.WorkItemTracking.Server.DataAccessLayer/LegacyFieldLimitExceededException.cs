// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyFieldLimitExceededException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacyFieldLimitExceededException : LegacyValidationException
  {
    public LegacyFieldLimitExceededException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException ex,
      SqlError sqlError)
      : base(InternalsResourceStrings.Format("ErrorNotEnoughFields", ExceptionHelper.ExtractStrings(sqlError, (object) "FieldsAddedCount", (object) "FieldsRemainingCount")), errorNumber, (Exception) ex, ExceptionHelper.TranslateFieldLimitExceededException(requestContext, errorNumber, ex, sqlError))
    {
    }

    public LegacyFieldLimitExceededException(int fieldsAdded, int fieldsRemaining)
      : base(InternalsResourceStrings.Format("ErrorNotEnoughFields", (object) fieldsAdded, (object) fieldsRemaining), 600173, (Exception) null, ExceptionHelper.TranslateFieldLimitExceededException(600173, fieldsAdded.ToString((IFormatProvider) CultureInfo.InvariantCulture), fieldsRemaining.ToString((IFormatProvider) CultureInfo.InvariantCulture)))
    {
    }
  }
}
