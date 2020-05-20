﻿using System;
using System.Collections.Generic;

namespace Forkdown.Core.Config {
  public partial class ExternalLinkConfig {
    public static ExternalLinkConfig From(MainConfigSource source) {
      return new ExternalLinkConfig
      {
        DefaultUrl = source.GetValueOrDefault("external_links.default_url") as String ?? "",
        Rewrites = ExternalLinkRewriteConfig.From(source)
      };
    }
  }
}
