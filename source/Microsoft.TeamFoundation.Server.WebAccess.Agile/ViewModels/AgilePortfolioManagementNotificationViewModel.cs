// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.AgilePortfolioManagementNotificationViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Models;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class AgilePortfolioManagementNotificationViewModel
  {
    [DataMember(Name = "className", EmitDefaultValue = true)]
    public string ClassName { get; set; }

    [DataMember(Name = "closeable", EmitDefaultValue = true)]
    public bool Closeable { get; set; }

    [DataMember(Name = "message", EmitDefaultValue = true)]
    public NotificationMessageModel Message { get; set; }
  }
}
