﻿using System;
using FluentAssertions;
using Forkdown.Core.Elements;
using Forkdown.Core.Fd;
using Forkdown.Core.Fd.Processors;
using Xunit;

// ReSharper disable ArrangeTypeMemberModifiers

namespace Forkdown.Core.Tests.Parsing.Forkdown {
  public class CheckboxIdTests {
    private static FdBuilder _builder => new FdBuilder()
      .AddProcessor<ArticleProcessor>()
      .AddProcessor<CheckboxIdProcessor>();
    
    [Fact]
    void ExplicitId() {
      var input = (Document)new Document().AddSub(
        new Article((Heading)new Heading(1).AddSub(new Text("Heading One"))).AddSub(
          new Listing().AddSub(
            new ListItem().AddSubs(
              new Paragraph().AddSub(new Text("Item")),
              new Listing().AddSub(
                new ListItem { GlobalIds = { "x" } }.AddSubs(
                  new Paragraph().AddSub(new Text("X")),
                  new Listing().AddSub(
                    new ListItem().AddSub(
                      new Paragraph().AddSub(new Text("Sub"))
                    )
                  )
                )
              )
            )
          )
        )
      );
      
      _builder.Build(input)
        .FirstSub<Listing>()
        .FirstSub<Listing>()
        .FirstSub<Listing>()
        .FirstSub<ListItem>()
        .CheckboxId.Should().Be($"x{CheckboxIdProcessor.G}Sub");
    }


    [Fact]
    void ComplicatedExplicitId() {
      var input = (Document) new Document().AddSub(
        new Article(new Heading(1)) { GlobalIds = { "altar_cave" } }.AddSubs(
          new Paragraph(),
          new Article(new Heading(2)) { GlobalIds = { "enemies" } }.AddSub(
            new Article((Heading) new Heading(3).AddSub(new Text("Level 1"))).AddSub(
              new Listing().AddSub(
                new ListItem().AddSub(
                  new Link().AddSub(
                    new Text("Carbuncle")
                  )
                )
              )
            )
          )
        )
      );

      var result = _builder.Build(input);
      result.FindSub<ListItem>()!.CheckboxId.Should().Be(
        $"enemies" +
        $"{CheckboxIdProcessor.G}Level{CheckboxIdProcessor.W}1" +
        $"{CheckboxIdProcessor.G}Carbuncle"
      );
    }

    [Fact]
    void ComplicatedImplicitId() {

      var input = (Document) new Document().AddSub(
        new Article((Heading)
          new Heading(1).AddSubs(new Text("The "), new Link().AddSub(new Text("Floating Continent")))
        ).AddSubs(
          new Paragraph(),
          new Article((Heading) new Heading(2).AddSub(new Text("Mainland"))).AddSub(
            new Article((Heading) new Heading(3).AddSub(new Text("Northeast"))).AddSubs(
              new Paragraph(),
              new Listing().AddSub(
                new ListItem().AddSub(
                  new Paragraph().AddSub(
                    new Link().AddSub(
                      new Text("Killer Bee")
                    )
                  )
                )
              )
            )
          )
        )
      );

      var result = _builder.Build(input);
      result.FindSub<ListItem>()!.CheckboxId.Should().Be(
        $"The{CheckboxIdProcessor.W}Floating{CheckboxIdProcessor.W}Continent" +
        $"{CheckboxIdProcessor.G}Mainland" +
        $"{CheckboxIdProcessor.G}Northeast" +
        $"{CheckboxIdProcessor.G}Killer{CheckboxIdProcessor.W}Bee"
      );
    }

    [Fact]
    void Duplicates() {
      const String input = @"# Heading One
* Item
* Item
* Item";
      _builder.Build(input)
        .FirstSub<Listing>()
        .Subs[2] // 3rd item
        .As<ListItem>()
        .CheckboxId.Should()
        .Be($"Heading{CheckboxIdProcessor.W}One{CheckboxIdProcessor.G}Item{CheckboxIdProcessor.R}3");
    }

    [Fact]
    void BasicId() {
      const String input = @"# Heading One
* Item one
  * Item two";
      var doc = _builder.Build(input);
      doc
        .FirstSub<Listing>()
        .FirstSub<Listing>()
        .FirstSub<ListItem>().CheckboxId.Should().Be("Heading⸱One␝Item⸱one␝Item⸱two");
    }
  }
}
