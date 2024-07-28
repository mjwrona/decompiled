// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExpandedChangeEnumerator
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ExpandedChangeEnumerator : 
    IEnumerator<ExpandedChange>,
    IDisposable,
    IEnumerator,
    IEnumerable<ExpandedChange>,
    IEnumerable
  {
    private List<Exception> m_expansionExceptions;
    private int m_currentIndex;
    private ChangeRequest[] m_requests;
    private VersionControlRequestContext m_versionControlRequestContext;
    private Workspace m_workspace;
    private VersionedItemComponent m_db;
    private List<Failure> m_failures;
    private ExpandedChange m_currentChange;

    public ExpandedChangeEnumerator(
      VersionControlRequestContext requestContext,
      ChangeRequest[] requests,
      Workspace workspace,
      VersionedItemComponent db)
    {
      this.m_versionControlRequestContext = requestContext;
      this.m_requests = requests;
      this.m_workspace = workspace;
      this.m_db = db;
      this.Reset();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<ExpandedChange> GetEnumerator() => (IEnumerator<ExpandedChange>) this;

    public void Reset()
    {
      this.m_currentIndex = 0;
      this.m_expansionExceptions = new List<Exception>();
      this.m_failures = new List<Failure>();
      this.m_currentChange = (ExpandedChange) null;
      for (int index = 0; index < this.m_requests.Length; ++index)
        this.m_requests[index].Reset();
    }

    public bool MoveNext()
    {
      for (; this.m_currentIndex < this.m_requests.Length; ++this.m_currentIndex)
      {
        ChangeRequest request = this.m_requests[this.m_currentIndex];
        try
        {
          if (request.GetNextExpandedChange(this.m_versionControlRequestContext, this.m_workspace, this.m_expansionExceptions, this.m_db, this.m_currentIndex, out this.m_currentChange))
            return true;
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (ArgumentException ex)
        {
          this.m_versionControlRequestContext.RequestContext.TraceException(700059, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
          this.m_failures.Add(new Failure((Exception) ex, request.ItemSpec.Item, request.RequestType));
        }
        catch (ApplicationException ex)
        {
          this.m_versionControlRequestContext.RequestContext.TraceException(700060, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
          this.m_failures.Add(new Failure((Exception) ex, request.ItemSpec.Item, request.RequestType));
        }
        finally
        {
          foreach (Exception expansionException in this.m_expansionExceptions)
            this.m_failures.Add(new Failure(expansionException, request.ItemSpec.Item, request.RequestType));
          this.m_expansionExceptions.Clear();
        }
      }
      return false;
    }

    public List<Failure> Failures => this.m_failures;

    object IEnumerator.Current => (object) this.Current;

    public ExpandedChange Current => this.m_currentChange;

    public void Dispose()
    {
    }
  }
}
