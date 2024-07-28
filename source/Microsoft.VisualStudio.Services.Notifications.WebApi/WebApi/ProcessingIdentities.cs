// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.ProcessingIdentities
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class ProcessingIdentities
  {
    [DataMember]
    public Dictionary<Guid, ProcessingDiagnosticIdentity> IncludedIdentities = new Dictionary<Guid, ProcessingDiagnosticIdentity>();
    [DataMember]
    public Dictionary<Guid, ProcessingDiagnosticIdentity> ExcludedIdentities = new Dictionary<Guid, ProcessingDiagnosticIdentity>();
    [DataMember]
    public ISet<Guid> MissingIdentities = (ISet<Guid>) new HashSet<Guid>();

    [DataMember]
    public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

    [DataMember]
    public List<NotificationDiagnosticLogMessage> Messages { get; } = new List<NotificationDiagnosticLogMessage>();
  }
}
