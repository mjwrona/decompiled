// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestCaseResult : ISecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Id;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int AfnStripId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Configuration;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Project;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CompletedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double DurationInMs;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Outcome;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Revision;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestCase;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestPoint;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestRun;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ResolutionStateId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ResolutionState;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastUpdatedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Priority;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ComputerName;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ResetCount;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Build;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Release;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreatedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestIterationDetailsModel> IterationDetails;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<ShallowReference> AssociatedBugs;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string FailureType;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestStorage;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestType;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestTypeId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Area;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestCaseTitle;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestCaseRevision;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StackTrace;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<CustomTestField> CustomFields;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public FailingSince FailingSince;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReference BuildReference;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ReleaseReference ReleaseReference;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestPlan;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestSuite;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestCaseReferenceId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef Owner;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef RunBy;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef LastUpdatedBy;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ResultGroupType ResultGroupType;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestSubResult> SubResults;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestResultDimension> Dimensions;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<Link<ResultLinkType>> Links;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildType;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestPhase;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LayoutUid;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Attempt;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsSystemIssue;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ExceptionType;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Locale;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BucketUid;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BucketingSystem;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ExecutionNumber;
    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TopologyId;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestName { get; set; }

    internal void EnsureSecureObject()
    {
      this.Configuration?.InitializeSecureObject((ISecuredObject) this);
      this.Project?.InitializeSecureObject((ISecuredObject) this);
      this.TestCase?.InitializeSecureObject((ISecuredObject) this);
      this.TestPoint?.InitializeSecureObject((ISecuredObject) this);
      this.TestRun?.InitializeSecureObject((ISecuredObject) this);
      this.Build?.InitializeSecureObject((ISecuredObject) this);
      this.BuildReference?.InitializeSecureObject((ISecuredObject) this);
      this.Release?.InitializeSecureObject((ISecuredObject) this);
      this.ReleaseReference?.InitializeSecureObject((ISecuredObject) this);
      this.TestSuite?.InitializeSecureObject((ISecuredObject) this);
      this.TestPlan?.InitializeSecureObject((ISecuredObject) this);
      this.Area?.InitializeSecureObject((ISecuredObject) this);
      this.FailingSince?.InitializeSecureObject((ISecuredObject) this);
      this.SecureIdentityRef(this.LastUpdatedBy);
      this.SecureIdentityRef(this.RunBy);
      this.SecureIdentityRef(this.Owner);
      if (this.CustomFields != null)
      {
        foreach (TestManagementBaseSecuredObject customField in this.CustomFields)
          customField.InitializeSecureObject((ISecuredObject) this);
      }
      if (this.IterationDetails != null)
      {
        foreach (TestManagementBaseSecuredObject iterationDetail in this.IterationDetails)
          iterationDetail.InitializeSecureObject((ISecuredObject) this);
      }
      if (this.AssociatedBugs != null)
      {
        foreach (TestManagementBaseSecuredObject associatedBug in this.AssociatedBugs)
          associatedBug.InitializeSecureObject((ISecuredObject) this);
      }
      if (this.SubResults == null)
        return;
      foreach (TestManagementBaseSecuredObject subResult in this.SubResults)
        subResult.InitializeSecureObject((ISecuredObject) this);
    }

    private void SecureIdentityRef(IdentityRef identity)
    {
      if (identity == null || identity.Links == null || identity.Links.Links == null)
        return;
      ReferenceLinks referenceLinks = new ReferenceLinks();
      foreach (KeyValuePair<string, object> link in (IEnumerable<KeyValuePair<string, object>>) identity.Links.Links)
        referenceLinks.AddLink(link.Key, link.Value is ReferenceLink referenceLink ? referenceLink.Href : (string) null, (ISecuredObject) identity);
      identity.Links = referenceLinks;
    }

    Guid ISecuredObject.NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => TeamProjectSecurityConstants.ViewTestResults;

    string ISecuredObject.GetToken()
    {
      ArgumentUtility.CheckForNull<ShallowReference>(this.Project, "Project");
      return TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.Project.Id));
    }
  }
}
