// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ContentValidation.Client.ContentViolationReportResult
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ContentValidation.Client
{
  [DataContract]
  public class ContentViolationReportResult : ISecuredObject
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public ContentViolationReportResultStatus Status { get; set; }

    Guid ISecuredObject.NamespaceId => ContentValidationSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 2;

    string ISecuredObject.GetToken() => ContentValidationSecurityConstants.ViolationsToken;
  }
}
