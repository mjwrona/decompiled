// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.RegistrationBinder3
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class RegistrationBinder3 : RegistrationBinder2
  {
    private SqlColumnBinder SecretValidToColumn = new SqlColumnBinder("SecretValidTo");

    protected override Registration Bind()
    {
      Registration registration1 = base.Bind();
      DateTimeOffset? nullable1 = new DateTimeOffset?(this.SecretValidToColumn.GetDateTimeOffset(this.Reader));
      if (nullable1.HasValue)
      {
        DateTimeOffset dateTimeOffset = nullable1.Value;
        if (dateTimeOffset.DateTime > DateTime.MinValue)
        {
          Registration registration2 = registration1;
          dateTimeOffset = nullable1.Value;
          DateTimeOffset? nullable2 = new DateTimeOffset?((DateTimeOffset) dateTimeOffset.DateTime);
          registration2.SecretValidTo = nullable2;
          goto label_4;
        }
      }
      registration1.SecretValidTo = new DateTimeOffset?();
label_4:
      return registration1;
    }
  }
}
