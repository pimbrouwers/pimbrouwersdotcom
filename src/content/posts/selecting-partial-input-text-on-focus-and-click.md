---
slug: selecting-partial-input-text-on-focus-and-click
title: Selection Partial Input Text on Focus, Click and Other DOM Events
template: post.hbs
date: 2016-01-07
author: Pim Brouwers
tags: JavaScript UX
---
I was recently working on a project that enabled users to rename files, backed by blob storage assets. Where this type of functionality becomes problematic is when files are renamed without extensions. We of course do not want to limit the user in anyway so we must expose the ability to modify the file name in it's entirety. In addition to checking server-side, which is always advisable in situations like this, I wanted to create an OS-like experience by default selecting the file name only without the extension. Windows 7/8 and OS X do this very nicely, and it makes renaming files more intuitive. How often are we modifying extensions right?

To implement this from a code stand point is super simple. Consider the following:
- Define a functions that locates the final period of the input value (assuming there is a value)
- On DOM ready, bind the "focus" and "click" event handlers to this function 

Now in code:
```Javascript	
//establish text selection method
var selectFilename = function () {
    var textval = $(this).val();

    if (!textval || textval == null || textval == "") return;

    //find last period
    var suffixStartpos = textval.lastIndexOf(".");

    if (!suffixStartpos || suffixStartpos == null || suffixStartpos == "") return;

    //set selection if all requirements fulfilled
    this.setSelectionRange(0, suffixStartpos);
};

//bind
$("#textinput").on("focus click", selectFilename);
```