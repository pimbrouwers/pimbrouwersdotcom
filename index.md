# This is a website

<p class="big">Pim has a website and this is that website.</p>

***
$(foreach($post in $postMeta) {
@"
<p>
    <small class="muted monospace">$($post.Date)</small>
    <br /><a href="$($post.Url)">$($post.Title)</a>
</p>
"@
})
***

## Projects
[Falco](//github.com/pimbrouwers/Falco) - A toolkit for building fast and functional-first web applications using F#.

[Donald](//github.com/pimbrouwers/Donald) - A lightweight, generic F# database abstraction.

[Validus](//github.com/pimbrouwers/Validus) - An extensible F# validation library.

[Falco.Markup](//github.com/pimbrouwers/Falco.Markup) - An F# DSL for generating markup, including HTML, SVG and XML.

[Jay](//github.com/pimbrouwers/Jay) - An F# JSON parser & serializer.

[LunchPail](//github.com/pimbrouwers/LunchPail) - .NET Standard Unit of Work implementation for ADO.NET.