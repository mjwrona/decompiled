// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Mail.EmailRecipients
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Mail
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class EmailRecipients
  {
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string[] EmailAddresses { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public Guid[] TfIds { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public Guid[] UnresolvedEntityIds { get; set; }
  }
}
