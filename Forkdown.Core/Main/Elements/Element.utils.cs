﻿using System;
using System.Linq;
using Forkdown.Core.Elements.Types;
using Simpler.NetCore.Collections;

namespace Forkdown.Core.Elements {
  public partial class Element {
    /// <summary>
    /// Extract an element's <see cref="Title"/> (first line in plain text) from it's sub-elements.
    /// </summary>
    /// <param name="el">Element.</param>
    /// <param name="contentFound">When <c>true</c>, it means the first text-containing sub-element has been found
    /// and the title must be constructed from it.</param>
    /// <returns></returns>
    protected static String TitleOf(Element el, Boolean contentFound = false) {
      if (el is Text t)
        return t.Content;

      if (contentFound) {
        return el.Subs.TakeWhile(_ => _ is Inline && !(_ is LineBreak))
          .Select(_ => Element.TitleOf(_, contentFound))
          .StringJoin();
      }

      return (el.Subs.FirstOrDefault() is Inline inline && inline != null
               ? TitleOf(el, true)
               : el.Subs.Take(1)
                 .Select(_ => TitleOf(_, false))
                 .FirstOrDefault())
             ?? "";
    }
  }
}
