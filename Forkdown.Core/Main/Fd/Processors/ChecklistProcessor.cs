﻿using System;
using Forkdown.Core.Elements;
using Forkdown.Core.Fd.Contexts;

namespace Forkdown.Core.Fd.Processors {
  public class ChecklistProcessor : IElementProcessor {

    public virtual Result<T> Process<T>(T element, IContext context) where T : Element {
      var inChecklist = context.GetArg<Boolean>();
      
      if (element is ListItem item) {
        if (inChecklist)
          item.IsCheckbox = item.Settings.NotFalse("checkbox");
        
        item.IsCheckbox = item.BulletChar switch {
          '+' => true,
          '-' => false,
          _ => item.IsCheckbox
        };
      }

      if (element.Settings.IsTrue("checklist"))
        inChecklist = true;
      else if (element.Settings.IsFalse("checklist"))
        inChecklist = false;

      if (element is Listing l)
        l.IsChecklist = inChecklist;

      context.SetArg(inChecklist);
      return context.ToResult(element);
    }
  }
}