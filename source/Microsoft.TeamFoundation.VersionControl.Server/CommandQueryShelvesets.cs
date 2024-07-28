// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryShelvesets
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryShelvesets : VersionControlCommand
  {
    private CommandQueryShelvesets.State m_state;
    private List<Shelveset> m_shelvesets;
    private PropertyMerger<Shelveset> m_merger;

    public CommandQueryShelvesets(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(
      string shelvesetName,
      string shelvesetOwnerName,
      string[] propertyNameFilters)
    {
      this.Execute(shelvesetName, shelvesetOwnerName, 0, propertyNameFilters);
    }

    internal void Execute(
      string shelvesetName,
      string shelvesetOwnerName,
      int shelveSetVersion,
      string[] propertyNameFilters)
    {
      this.m_shelvesets = new List<Shelveset>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = string.IsNullOrEmpty(shelvesetOwnerName) ? (Microsoft.VisualStudio.Services.Identity.Identity) null : TfvcIdentityHelper.FindIdentity(this.m_versionControlRequestContext.RequestContext, shelvesetOwnerName);
      ArgumentUtility.CheckForOutOfRange(shelveSetVersion, nameof (shelveSetVersion), -1);
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
      {
        ResultCollection resultCollection = versionedItemComponent.QueryShelvesets(identity != null ? identity.Id : Guid.Empty, shelvesetName, shelveSetVersion);
        ObjectBinder<Shelveset> current1 = resultCollection.GetCurrent<Shelveset>();
        while (current1.MoveNext())
        {
          this.m_shelvesets.Add(current1.Current);
          resultCollection.IncrementRowCounter();
        }
        resultCollection.NextResult();
        ObjectBinder<CheckinNoteFieldValue> current2 = resultCollection.GetCurrent<CheckinNoteFieldValue>();
        List<CheckinNoteFieldValue> checkinNoteFieldValueList = new List<CheckinNoteFieldValue>();
        int num1 = 0;
        int num2 = 0;
        while (current2.MoveNext())
        {
          CheckinNoteFieldValue current3 = current2.Current;
          if (current3.checkinNoteId != num1 && checkinNoteFieldValueList.Count > 0)
          {
            while (num2 < this.m_shelvesets.Count)
            {
              Shelveset shelveset = this.m_shelvesets[num2++];
              if (shelveset.checkinNoteId == num1)
              {
                shelveset.CheckinNote.Values = checkinNoteFieldValueList.ToArray();
                resultCollection.IncrementRowCounter();
                break;
              }
            }
            checkinNoteFieldValueList.Clear();
          }
          checkinNoteFieldValueList.Add(current3);
          num1 = current3.checkinNoteId;
        }
        if (checkinNoteFieldValueList.Count > 0)
        {
          while (num2 < this.m_shelvesets.Count)
          {
            Shelveset shelveset = this.m_shelvesets[num2++];
            if (shelveset.checkinNoteId == num1)
            {
              shelveset.CheckinNote.Values = checkinNoteFieldValueList.ToArray();
              break;
            }
          }
        }
        resultCollection.NextResult();
        ObjectBinder<VersionControlLink> current4 = resultCollection.GetCurrent<VersionControlLink>();
        List<VersionControlLink> versionControlLinkList = new List<VersionControlLink>();
        int index = 0;
        Shelveset shelveset1 = (Shelveset) null;
        while (current4.MoveNext())
        {
          VersionControlLink current5 = current4.Current;
          int shelvesetId = current5.ShelvesetId;
          for (; index < this.m_shelvesets.Count; ++index)
          {
            shelveset1 = this.m_shelvesets[index];
            if (shelveset1.shelvesetId < shelvesetId)
            {
              if (versionControlLinkList.Count > 0)
              {
                shelveset1.Links = versionControlLinkList.ToArray();
                resultCollection.IncrementRowCounter();
                versionControlLinkList.Clear();
              }
            }
            else
              break;
          }
          if (shelveset1 != null && shelveset1.shelvesetId == shelvesetId)
            versionControlLinkList.Add(current5);
        }
        if (versionControlLinkList.Count > 0)
        {
          if (shelveset1 != null)
            shelveset1.Links = versionControlLinkList.ToArray();
        }
      }
      StreamingCollection<Shelveset> streamingCollection = new StreamingCollection<Shelveset>((Command) this)
      {
        HandleExceptions = false
      };
      foreach (Shelveset shelveset in this.m_shelvesets)
      {
        string identityName;
        string displayName;
        this.m_versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(this.m_versionControlRequestContext.RequestContext, shelveset.ownerId, out identityName, out displayName);
        shelveset.Owner = identityName;
        shelveset.OwnerDisplayName = displayName;
        streamingCollection.Enqueue(shelveset);
      }
      streamingCollection.IsComplete = true;
      this.m_state = CommandQueryShelvesets.State.Complete;
      if (propertyNameFilters != null && propertyNameFilters.Length != 0)
      {
        this.m_merger = new PropertyMerger<Shelveset>(this.m_versionControlRequestContext, propertyNameFilters, (VersionControlCommand) this, VersionControlPropertyKinds.Shelveset);
        this.m_merger.Execute(streamingCollection);
        this.m_state = CommandQueryShelvesets.State.Properties;
      }
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_state != CommandQueryShelvesets.State.Properties || this.m_merger.TryMergeNextPage())
        return;
      this.m_state = CommandQueryShelvesets.State.Complete;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing || this.m_merger == null)
        return;
      this.m_merger.Dispose();
      this.m_merger = (PropertyMerger<Shelveset>) null;
    }

    public List<Shelveset> Shelvesets => this.m_shelvesets;

    private enum State
    {
      Properties,
      Complete,
    }
  }
}
