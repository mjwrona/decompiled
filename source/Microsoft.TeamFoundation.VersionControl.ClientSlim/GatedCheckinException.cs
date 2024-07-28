// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.GatedCheckinException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.Framework.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public sealed class GatedCheckinException : ActionDeniedBySubscriberException
  {
    private Uri m_buildUri;
    private List<string> m_overridePermissionFailures;
    private List<KeyValuePair<string, Uri>> m_affectedBuildDefinitions;

    internal GatedCheckinException(
      ActionDeniedBySubscriberException actionDeniedException)
      : base(actionDeniedException.Message, actionDeniedException.InnerException)
    {
    }

    public ReadOnlyCollection<KeyValuePair<string, Uri>> AffectedBuildDefinitions
    {
      get
      {
        if (this.m_affectedBuildDefinitions == null)
        {
          this.m_affectedBuildDefinitions = new List<KeyValuePair<string, Uri>>();
          object[] property1 = this.GetProperty<object[]>("AffectedBuildDefinitionUris");
          object[] property2 = this.GetProperty<object[]>("AffectedBuildDefinitionNames");
          if (property1 != null && property2 != null && property1.Length == property2.Length)
          {
            for (int index = 0; index < property2.Length; ++index)
              this.m_affectedBuildDefinitions.Add(new KeyValuePair<string, Uri>(property2[index] as string, new Uri(property1[index] as string)));
          }
        }
        return this.m_affectedBuildDefinitions.AsReadOnly();
      }
    }

    public int QueueId => this.GetProperty<int>(nameof (QueueId));

    public Uri BuildUri
    {
      get
      {
        if (this.m_buildUri == (Uri) null)
        {
          string property = this.GetProperty<string>(nameof (BuildUri));
          if (!string.IsNullOrEmpty(property))
            this.m_buildUri = new Uri(property);
        }
        return this.m_buildUri;
      }
    }

    public ReadOnlyCollection<string> OverridePermissionFailures
    {
      get
      {
        if (this.m_overridePermissionFailures == null)
        {
          this.m_overridePermissionFailures = new List<string>();
          object[] property = this.GetProperty<object[]>(nameof (OverridePermissionFailures));
          if (property != null)
          {
            for (int index = 0; index < property.Length; ++index)
              this.m_overridePermissionFailures.Add(property[index] as string);
          }
        }
        return this.m_overridePermissionFailures.AsReadOnly();
      }
    }

    public bool HasOverridePermission => this.GetProperty<bool>(nameof (HasOverridePermission));

    public string ShelvesetName => this.GetProperty<string>(nameof (ShelvesetName));

    public string CheckInTicket => this.GetProperty<string>(nameof (CheckInTicket));

    public int SubCode => this.GetProperty<int>(nameof (SubCode));

    public bool CheckInContainsLocks => this.GetProperty<bool>(nameof (CheckInContainsLocks));
  }
}
