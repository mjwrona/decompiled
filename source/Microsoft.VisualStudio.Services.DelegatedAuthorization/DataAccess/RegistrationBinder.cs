// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.RegistrationBinder
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class RegistrationBinder : ObjectBinder<Registration>
  {
    private SqlColumnBinder RegistrationIdColumn = new SqlColumnBinder("RegistrationId");
    private SqlColumnBinder IdentityIdColumn = new SqlColumnBinder("IdentityId");
    private SqlColumnBinder OrganizationNameColumn = new SqlColumnBinder("OrganizationName");
    private SqlColumnBinder OrganizationLocationColumn = new SqlColumnBinder("OrganizationLocation");
    private SqlColumnBinder RegistrationNameColumn = new SqlColumnBinder("RegistrationName");
    private SqlColumnBinder RegistrationDescriptionColumn = new SqlColumnBinder("RegistrationDescription");
    private SqlColumnBinder RegistrationLocationColumn = new SqlColumnBinder("RegistrationLocation");
    private SqlColumnBinder RegistrationLogoSecureLocationColumn = new SqlColumnBinder("RegistrationLogoSecureLocation");
    private SqlColumnBinder RegistrationTermsOfServiceLocationColumn = new SqlColumnBinder("RegistrationTermsOfServiceLocation");
    private SqlColumnBinder RegistrationPrivacyPolicyLocationColumn = new SqlColumnBinder("RegistrationPrivacyPolicyLocation");
    private SqlColumnBinder ResponseTypesColumn = new SqlColumnBinder("ResponseTypes");
    private SqlColumnBinder ScopesColumn = new SqlColumnBinder("Scopes");
    private SqlColumnBinder SecretVersionIdColumn = new SqlColumnBinder("SecretVersionId");
    private SqlColumnBinder IsValidColumn = new SqlColumnBinder("IsValid");

    protected override Registration Bind()
    {
      Guid guid1 = this.RegistrationIdColumn.GetGuid((IDataReader) this.Reader);
      Guid guid2 = this.IdentityIdColumn.GetGuid((IDataReader) this.Reader);
      string str1 = this.OrganizationNameColumn.GetString((IDataReader) this.Reader, true);
      string uriString1 = this.OrganizationLocationColumn.GetString((IDataReader) this.Reader, true);
      string str2 = this.RegistrationNameColumn.GetString((IDataReader) this.Reader, true);
      string str3 = this.RegistrationDescriptionColumn.GetString((IDataReader) this.Reader, true);
      string uriString2 = this.RegistrationLocationColumn.GetString((IDataReader) this.Reader, true);
      string uriString3 = this.RegistrationLogoSecureLocationColumn.GetString((IDataReader) this.Reader, true);
      string uriString4 = this.RegistrationTermsOfServiceLocationColumn.GetString((IDataReader) this.Reader, true);
      string uriString5 = this.RegistrationPrivacyPolicyLocationColumn.GetString((IDataReader) this.Reader, true);
      string str4 = this.ResponseTypesColumn.GetString((IDataReader) this.Reader, true);
      string str5 = this.ScopesColumn.GetString((IDataReader) this.Reader, true);
      Guid guid3 = this.SecretVersionIdColumn.GetGuid((IDataReader) this.Reader, true);
      bool boolean = this.IsValidColumn.GetBoolean((IDataReader) this.Reader, true);
      Registration registration = new Registration()
      {
        IdentityId = guid2,
        RegistrationId = guid1,
        OrganizationName = str1,
        RegistrationName = str2,
        RegistrationDescription = str3,
        ResponseTypes = str4,
        Scopes = str5,
        SecretVersionId = guid3,
        IsValid = boolean
      };
      if (!string.IsNullOrEmpty(uriString2))
        registration.RegistrationLocation = new Uri(uriString2);
      if (!string.IsNullOrEmpty(uriString1))
        registration.OrganizationLocation = new Uri(uriString1);
      if (!string.IsNullOrEmpty(uriString3))
        registration.RegistrationLogoSecureLocation = new Uri(uriString3);
      if (!string.IsNullOrEmpty(uriString4))
        registration.RegistrationTermsOfServiceLocation = new Uri(uriString4);
      if (!string.IsNullOrEmpty(uriString5))
        registration.RegistrationPrivacyStatementLocation = new Uri(uriString5);
      return registration;
    }
  }
}
