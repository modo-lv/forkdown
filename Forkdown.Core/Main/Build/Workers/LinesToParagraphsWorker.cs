﻿using System.Linq;
using Forkdown.Core.Elements;
using Forkdown.Core.Elements.Types;
using static MoreLinq.Extensions.GroupAdjacentExtension;
using Simpler.NetCore.Collections;

namespace Forkdown.Core.Build.Workers {
  /**
   * Convert lines to paragraphs.
   */
  public class LinesToParagraphsWorker : Worker, IDocumentWorker {

    public override Element ProcessElement(Element element, Arguments args) {
      if (element is BlockContainer) {
        var newSubs = Nil.L<Element>();
        foreach (Element el in element.Subs) {
          if (el is Paragraph p && p.Subs.Any(_ => _ is LineBreak)) {
            var lineSubs = Nil.L<Element>();
            p.Subs.GroupAdjacent(_ => _ is LineBreak).Where(_ => !_.Key).ForEach(_ => {
              lineSubs.Add(new Paragraph {
                Subs = _.ToList()
              });
            });
            if (lineSubs.Any())
              p.MoveAttributesTo(lineSubs.First());
            newSubs = newSubs.Concat(lineSubs).ToList();
          }
          else {
            newSubs.Add(el);
          }
        }
        element.Subs = newSubs;
      }
      return element;
    }
  }
}
