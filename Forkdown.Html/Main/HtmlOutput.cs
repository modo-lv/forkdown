﻿using System.IO;
using Forkdown.Core;
using Forkdown.Core.Elements;
using Forkdown.Core.Parsing.Forkdown;
using Forkdown.Core.Wiring;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Runtime;
using Simpler.NetCore.Collections;
using Simpler.NetCore.Text;
using Document = Forkdown.Core.Elements.Document;
using Path = Fluent.IO.Path;

#pragma warning disable 1591

namespace Forkdown.Html.Main {
  public class HtmlOutput {
    /// <summary>
    /// Forkdown project to build from.
    /// </summary>
    public readonly Project Project;


    private readonly ILogger<HtmlOutput> _logger;
    private readonly BuildArguments _args;
    public HtmlOutput(Project project, ILogger<HtmlOutput> logger, BuildArguments args) {
      this.Project = project;
      this._logger = logger;
      this._args = args;
    }


    public HtmlOutput BuildHtml() {
      this.Project.Load();
      var outRoot = this._args.ProjectRoot.Combine("out-net").CreateDirectories();

      this._logger.LogInformation("Building {output} in {root}...", "HTML", outRoot.ToString());

      var inRoot = Path.Get("Resources/Output/Default/Html");
      var inFile = inRoot.Combine("page.scriban-html").FullPath;
      var templateContext = new TemplateContext
      {
        TemplateLoader = new ScribanTemplateLoader(inRoot),
        MemberRenamer = _ => _.Name
      };
      var model = new ScriptObject
      {
        { "Project", this.Project },
      };
      templateContext.PushGlobal(model);
      var template = Template.Parse(text: File.ReadAllText(inFile));

      foreach (Document doc in this.Project.Pages)
      {
        this.ProcessLinks(doc);
        ProcessClasses(doc);

        var outFile = doc.FileName + ".html";
        Path outPath = outRoot.Combine("pages", outFile);
        outPath.Parent().CreateDirectories();
        this._logger.LogDebug("Rendering {page}...", outFile);

        model.Add("Document", doc);
        var html = template.Render(templateContext);
        File.WriteAllText(outPath.ToString(), html);
      }

      return this;
    }

    public static void ProcessClasses<T>(T element) where T : Element {
      switch (element) {
        case Listing l when l.IsChecklist: 
          l.Attributes.Classes.Add("fd_checklist");
          break;
        case ListItem li when li.IsCheckbox: 
          li.Attributes.Classes.Add("fd_checkbox");
          break;
      }

      element.Subs.ForEach(ProcessClasses);
    }
    
    
    public void ProcessLinks(Element el, Document? doc = null) {
      doc ??= (Document) el;
      var index = this.Project.InteralLinks;
      if (el is Link link && link.IsInternal)
      {
        var target = GlobalId.From(link.Target);
        if (index.ContainsKey(target))
          link.Target = $"{"../".Repeat(doc.Depth)}{index[target].FileName}.html#{target}";
      }
      else
      {
        el.Subs.ForEach(_ => this.ProcessLinks(_, doc));
      }
    }
  }
}
