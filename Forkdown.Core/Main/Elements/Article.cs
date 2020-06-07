﻿using System;
using System.Collections.Generic;
using Forkdown.Core.Elements.Attributes;
using Forkdown.Core.Elements.Types;
using Simpler.NetCore.Collections;

// ReSharper disable NotAccessedField.Global

namespace Forkdown.Core.Elements {
  /// <summary>
  /// A piece of forkdown content grouped in an article based on a heading
  /// </summary>
  public class Article : BlockContainer {
    // ReSharper disable once SuggestBaseTypeForParameter
    public Article(Heading heading) {
      this.Subs.Insert(0, new Header(heading));
      heading.MoveAttributesTo(this);
    }
  }
}
