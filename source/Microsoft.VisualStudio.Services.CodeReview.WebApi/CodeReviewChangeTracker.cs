// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CodeReviewChangeTracker
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public class CodeReviewChangeTracker
  {
    private readonly ChangeArtifactBuilder m_ChangeArtifactBuilder;
    private static readonly ChangeType[] s_validChangeTypes = new ChangeType[7]
    {
      ChangeType.None,
      ChangeType.Add,
      ChangeType.Edit,
      ChangeType.Rename,
      ChangeType.Move,
      ChangeType.Delete,
      ChangeType.Edit | ChangeType.Rename
    };

    public CodeReviewChangeTracker() => this.m_ChangeArtifactBuilder = new ChangeArtifactBuilder();

    public bool ComputeChangeTrackingIds(
      IEnumerable<Iteration> previousIterations,
      Iteration iterationToCompute)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Iteration>>(previousIterations, nameof (previousIterations));
      ArgumentUtility.CheckForNull<Iteration>(iterationToCompute, nameof (iterationToCompute));
      IEnumerable<Iteration> previousIterations1 = (IEnumerable<Iteration>) previousIterations.OrderByDescending<Iteration, int?>((Func<Iteration, int?>) (iter => iter.Id));
      this.ValidateIterations(previousIterations1, iterationToCompute);
      foreach (Iteration iteration in previousIterations1)
        this.m_ChangeArtifactBuilder.BuildMap(iteration.ChangeList);
      return this.m_ChangeArtifactBuilder.Compute(iterationToCompute.ChangeList);
    }

    public bool ComputeChangeTrackingIds(
      Iteration iterationToCompute,
      Iteration previousIteration = null,
      bool continueIfMatchNotFound = true)
    {
      List<Iteration> iterationList;
      if (previousIteration != null)
        iterationList = new List<Iteration>()
        {
          previousIteration
        };
      else
        iterationList = new List<Iteration>();
      List<Iteration> previousIterations = iterationList;
      ArgumentUtility.CheckForNull<Iteration>(iterationToCompute, nameof (iterationToCompute));
      this.ValidateIterations((IEnumerable<Iteration>) previousIterations, iterationToCompute);
      IList<ChangeEntry> previousChangeEntries = (IList<ChangeEntry>) null;
      if (previousIteration != null)
        previousChangeEntries = previousIteration.ChangeList;
      return this.ComputeChangeTrackingIds(iterationToCompute.ChangeList, previousChangeEntries, continueIfMatchNotFound);
    }

    public bool ComputeChangeTrackingIds(
      IList<ChangeEntry> changeEntriesToCompute,
      IList<ChangeEntry> previousChangeEntries = null,
      bool continueIfMatchNotFound = true)
    {
      if (previousChangeEntries != null)
        this.m_ChangeArtifactBuilder.BuildMap(previousChangeEntries);
      return this.m_ChangeArtifactBuilder.Compute(changeEntriesToCompute, continueIfMatchNotFound);
    }

    private void ValidateIterations(
      IEnumerable<Iteration> previousIterations,
      Iteration iterationToCompute)
    {
      List<Iteration> source = new List<Iteration>()
      {
        iterationToCompute
      };
      source.AddRange(previousIterations);
      foreach (Iteration var in source)
      {
        ArgumentUtility.CheckForNull<Iteration>(var, "iteration");
        ArgumentUtility.CheckForOutOfRange(var.Id.GetValueOrDefault(), "iterationId", 1);
        ArgumentUtility.CheckForNull<IList<ChangeEntry>>(var.ChangeList, "ChangeList");
        foreach (ChangeEntry change in (IEnumerable<ChangeEntry>) var.ChangeList)
        {
          ArgumentUtility.CheckForNull<ChangeEntry>(change, "change");
          int? id = var.Id;
          int? nullable = iterationToCompute.Id;
          if (!(id.GetValueOrDefault() == nullable.GetValueOrDefault() & id.HasValue == nullable.HasValue))
          {
            nullable = change.IterationId;
            ArgumentUtility.CheckForOutOfRange(nullable.GetValueOrDefault(), "iterationId", 1);
            nullable = change.ChangeId;
            ArgumentUtility.CheckForOutOfRange(nullable.GetValueOrDefault(), "ChangeId", 1);
          }
          if (!((IEnumerable<ChangeType>) CodeReviewChangeTracker.s_validChangeTypes).Contains<ChangeType>(change.Type))
          {
            int type = (int) change.Type;
            nullable = var.Id;
            int iterationId = nullable.Value;
            throw new InvalidChangeTypeException((ChangeType) type, iterationId);
          }
        }
      }
      List<int?> list = source.Select<Iteration, int?>((Func<Iteration, int?>) (iteration => iteration.Id)).ToList<int?>();
      int num = list.Count<int?>() == list.Distinct<int?>().Count<int?>() ? list.First<int?>().Value : throw new IterationListNotCompleteException();
      for (int index = 0; index < list.Count; ++index)
      {
        if (list[index].Value > num--)
          throw new IterationListNotCompleteException();
      }
    }
  }
}
