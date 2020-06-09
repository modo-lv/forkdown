﻿using Forkdown.Core.Elements;
using Forkdown.Core.Fd.Contexts;

namespace Forkdown.Core.Fd.Processors {
  public class DocumentProcessor : ITreeProcessor, IElementProcessor {

    public virtual Result<T> Process<T>(T element, IContext context) where T : Element {
      if (element is Document doc &&
          doc.Subs.Count > 0 &&
          doc.Subs[0] is Paragraph par &&
          par.Subs.Count == 1 &&
          par.Subs[0] is Text text &&
          text.Content == ":") {
        par.MoveAttributesTo(doc);
        doc.Subs.Remove(par);
      }
      return context.ToResult(element);
    }
  }

}