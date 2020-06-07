﻿using System;
using System.Collections.Generic;
using System.Linq;
using Forkdown.Core.Elements;
using Forkdown.Core.Elements.Attributes;
using Markdig.Renderers.Html;
using Simpler.NetCore.Collections;
using Simpler.NetCore.Text;

namespace Forkdown.Core.Parsing.Forkdown.Processors {
  /// <summary>
  /// Parses and applies Forkdown's <c>{:setting=value}</c> settings from element HTML attributes.
  /// </summary>
  public class SettingsProcessor : IDocumentProcessor {

    public T Process<T>(T element, IDictionary<String, Object> args) where T : Element {
      var html = element.HtmlAttributes;
      if (html.Properties != null) {
        // ReSharper disable once RedundantCast
        element.Settings = new ElementSettings(
          html.Properties
            .Where(_ => _.Key?.StartsWith(":") ?? false)
            .ToDictionary(_ => _.Key.Part(1), _ => _.Value)
        );

        html.Properties.RemoveAll(_ => _.Key.StartsWith(":"));
      }

      return element;
    }
  }
}
