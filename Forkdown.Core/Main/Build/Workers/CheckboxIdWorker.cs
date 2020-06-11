﻿using System;
using System.Collections.Generic;
using Forkdown.Core.Elements;
using Forkdown.Core.Elements.Types;
using Simpler.NetCore.Collections;
using Simpler.NetCore.Text;

namespace Forkdown.Core.Build.Workers {
  public class CheckboxIdWorker : Worker {
    public const Char G = '␝'; // Group separator
    public const Char R = '␞'; // Record separator
    public const Char W = '⸱'; // Word separator
    
    private readonly IDictionary<String, Int32> _times = new Dictionary<String, Int32>();

    public override T ProcessElement<T>(T element, Arguments args) {
      String parentId = args.Get();

      var id = parentId;
      
      if (element is Document dEl) {
        id = dEl.ProjectFileId;
      }
      else if (element.GlobalId.NotBlank()) {
        id = element.GlobalId;
      }
      else if (element is BlockContainer bc) {
        id = element.Title.Replace(' ', W);
        if (parentId.NotBlank())
          id = $"{parentId}{G}{id}";
        if (id.NotBlank())
          _times[id] = _times.GetOr(id, 0) + 1;
        if (_times.GetOr(id, 0) > 1)
          id += $"{R}{_times[id]}";
        bc.CheckboxId = id;
      }

      args.Put(id);
      return element;
    }


  }
}
