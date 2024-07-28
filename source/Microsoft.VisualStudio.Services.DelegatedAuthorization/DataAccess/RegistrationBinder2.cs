// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.RegistrationBinder2
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class RegistrationBinder2 : RegistrationBinder
  {
    private SqlColumnBinder ClientTypeIdColumn = new SqlColumnBinder("ClientType");
    private SqlColumnBinder IsWellKnownColumn = new SqlColumnBinder("IsWellKnown");

    protected override Registration Bind()
    {
      Registration registration = base.Bind();
      registration.ClientType = (ClientType) this.ClientTypeIdColumn.GetByte((IDataReader) this.Reader, (byte) 0);
      registration.IsWellKnown = this.IsWellKnownColumn.GetBoolean((IDataReader) this.Reader, false);
      return registration;
    }
  }
}
