// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.FailingSince
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class FailingSince : TestManagementBaseSecuredObject
  {
    private const string FailingSinceForBuildFormat = "{{Date:\"{0:o}\",Build: {{Id:\"{1}\", Number:\"{2}\", BuildSystem:\"{3}\"}} }}";
    private const string FailingSinceForReleaseFormat = "{{Date:\"{0:o}\",Release: {{Id:\"{1}\", Name:\"{2}\", EnvironmentId:\"{3}\"}}, Build: {{ BranchName:\"{4}\"}} }}";

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public DateTime Date { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReference Build { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ReleaseReference Release { get; set; }

    public override string ToString()
    {
      if (this.Release != null && this.Release.Id > 0)
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{Date:\"{0:o}\",Release: {{Id:\"{1}\", Name:\"{2}\", EnvironmentId:\"{3}\"}}, Build: {{ BranchName:\"{4}\"}} }}", (object) this.Date.ToUniversalTime(), (object) this.Release.Id, (object) this.Release.Name, (object) this.Release.EnvironmentId, this.Build == null || string.IsNullOrEmpty(this.Build.BranchName) ? (object) string.Empty : (object) this.Build.BranchName);
      if (this.Build == null || this.Build.Id <= 0)
        return string.Empty;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{Date:\"{0:o}\",Build: {{Id:\"{1}\", Number:\"{2}\", BuildSystem:\"{3}\"}} }}", (object) this.Date.ToUniversalTime(), (object) this.Build.Id, this.Build == null || string.IsNullOrEmpty(this.Build.Number) ? (object) string.Empty : (object) this.Build.Number, this.Build == null || string.IsNullOrEmpty(this.Build.BuildSystem) ? (object) string.Empty : (object) this.Build.BuildSystem);
    }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.Build?.InitializeSecureObject(securedObject);
      this.Release?.InitializeSecureObject(securedObject);
    }
  }
}
