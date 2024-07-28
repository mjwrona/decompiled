// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.SendMailBody
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  public class SendMailBody
  {
    public MailMessage message { get; set; }

    public IEnumerable<int> ids { get; set; }

    public string wiql { get; set; }

    public IEnumerable<string> fields { get; set; }

    public Guid? persistenceId { get; set; }

    public string tempQueryId { get; set; }

    public IEnumerable<string> sortFields { get; set; }

    public string projectId { get; set; }
  }
}
