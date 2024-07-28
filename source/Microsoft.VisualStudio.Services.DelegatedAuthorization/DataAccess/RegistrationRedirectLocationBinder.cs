// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.RegistrationRedirectLocationBinder
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class RegistrationRedirectLocationBinder : ObjectBinder<RegistrationRedirectLocation>
  {
    private SqlColumnBinder RegistrationIdColumn = new SqlColumnBinder("RegistrationId");
    private SqlColumnBinder LocationColumn = new SqlColumnBinder("Location");

    protected override RegistrationRedirectLocation Bind()
    {
      Guid guid = this.RegistrationIdColumn.GetGuid((IDataReader) this.Reader, true);
      string uriString = this.LocationColumn.GetString((IDataReader) this.Reader, true);
      return new RegistrationRedirectLocation()
      {
        RegistrationId = guid,
        Location = new Uri(uriString)
      };
    }
  }
}
