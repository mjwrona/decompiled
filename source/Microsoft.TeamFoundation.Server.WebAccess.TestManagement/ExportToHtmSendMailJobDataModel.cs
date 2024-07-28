// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ExportToHtmSendMailJobDataModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class ExportToHtmSendMailJobDataModel
  {
    public ExportToHtmSendMailJobDataModel()
    {
    }

    public ExportToHtmSendMailJobDataModel(
      int planId,
      int suiteId,
      string parametersXml,
      string notesText,
      string projectName,
      Guid teamId,
      MailMessage message,
      Guid currentProjectGuid,
      string xsltFileText,
      List<string> dialogSettings)
    {
      this.PlanId = planId;
      this.SuiteId = suiteId;
      this.ParametersXml = parametersXml;
      this.NotesText = notesText;
      this.Message = message;
      this.TeamId = teamId;
      this.ProjectName = projectName;
      this.CurrentProjectGuid = currentProjectGuid;
      this.XsltFileText = xsltFileText;
      this.DialogSettings = dialogSettings;
    }

    [DataMember(Name = "planId")]
    public int PlanId { get; set; }

    [DataMember(Name = "suiteId")]
    public int SuiteId { get; set; }

    [DataMember(Name = "parametersXml")]
    public string ParametersXml { get; set; }

    [DataMember(Name = "notesText")]
    public string NotesText { get; set; }

    [DataMember(Name = "projectName")]
    public string ProjectName { get; set; }

    [DataMember(Name = "TeamId")]
    public Guid TeamId { get; set; }

    [DataMember(Name = "currentProjectGuid")]
    public Guid CurrentProjectGuid { get; set; }

    [DataMember(Name = "message")]
    public MailMessage Message { get; set; }

    [DataMember(Name = "xsltFileText")]
    public string XsltFileText { get; set; }

    [DataMember(Name = "dialogSettings")]
    public List<string> DialogSettings { get; set; }
  }
}
