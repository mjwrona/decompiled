// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EmailRecipients
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class EmailRecipients
  {
    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Copied contract from Microsoft.TeamFoundation.Server.WebAccess.Mail")]
    [DataMember]
    public string[] EmailAddresses { get; set; }

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Copied contract from Microsoft.TeamFoundation.Server.WebAccess.Mail")]
    [DataMember]
    public Guid[] TfsIds { get; set; }
  }
}
