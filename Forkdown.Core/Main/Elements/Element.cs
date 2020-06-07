﻿using System;
using System.Collections.Generic;
using System.Linq;
using Forkdown.Core.Elements.Attributes;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Simpler.NetCore.Collections;
using Simpler.NetCore.Text;

// ReSharper disable NotAccessedField.Global

namespace Forkdown.Core.Elements {
  public abstract partial class Element {
    public IList<Element> Subs = Nil.L<Element>();

    public virtual String Title => Element.TitleOf(this);

    public String Type => this.GetType().Name;

    public ElementSettings Settings = new ElementSettings();

    public HtmlAttributes Attributes = new HtmlAttributes();

    public ISet<String> Labels = Nil.SStr;

    /// <summary>
    /// Main identifier that is unique within the project. 
    /// </summary>
    public String GlobalId => this.GlobalIds.FirstOrDefault() ?? "";
    /// <summary>
    /// All project-unique identifiers for this element. 
    /// </summary>
    public IList<String> GlobalIds = Nil.LStr;

    
    
    protected Element() {}

    protected Element(IMarkdownObject mdo) : this() {
      this.Attributes = mdo.GetAttributes();
    }


    public virtual Element AddSub(Element sub) {
      this.Subs.Add(sub);
      return this;
    }

    public virtual Element AddSubs(params Element[] subs) {
      subs.ForEach(this.Subs.Add);
      return this;
    }
    
    public T FirstSub<T>() where T : Element =>
      this.FindSub<T>()!;

    public T? FindSub<T>() where T : Element {
      return (this.Subs.FirstOrDefault(_ => _ is T)
              ?? this.Subs.Select(_ => _.FindSub<T>()).FirstOrDefault(_ => _ != null))
        as T;
    }
    
    /// <summary>
    /// Move HTML attributes and Forkdown settings from this element to another.
    /// </summary>
    /// <param name="element">Target element.</param>
    public void MoveAttributesTo(Element element) {
      element.Attributes = this.Attributes;
      element.Settings = this.Settings;
      element.GlobalIds = this.GlobalIds;
      this.Attributes = new HtmlAttributes();
      this.Settings = new ElementSettings();
      this.GlobalIds = Nil.LStr;
    }
  }
}
