---
slug: why-i-love-knockout
title: Why I LOVE KnockoutJS
template: post.hbs
date: 2016-11-27
author: Pim Brouwers
tags: KnockoutJS
---
I love KnockoutJS. Aside from SQL, no technology I've encountered has inspired me quite to the level Knockout has. There's a captivating elegance to it's simplicity, and by proxy the ease in which you can learn it. And despite it's simplistic nature, it can transform JavaScript previously bound for spaghetti-town into artful solutions. It is my tool of choice for any project, of any scale, requiring any JavaScript. It **enforces** organization (components, custom bindings, extensions) without **imposing** conventions.

Knockout is a data-centric JavaScript library, enabling you to *bind* data to your DOM. At it's core, this is all Knockout does. Exposing a simple pub/sub concept, dubbed an "observable" (there is of course much more to it). Using the observable enables you to build reactive client-side applications with minimal effort. 

What Knockout **isn’t:**
- It is a library (not a framework), intended to work in unison with your other technologies.
- A complete solution. You’ll often add additional libraries to suit your use-case (ex:  to implement routing you could add in Sammy.js)
- Slow. If done correctly (deferred updates, using pure computed functions and binding to prototypes) you can achieve tremendous performance.
- Only good for small projects. Knockout can scale amazingly, I've built/worked on several large-scale projects with great success.
- Equipped with training wheels. Unlike other JavaScript libraries/frameworks, Knockout is wildly unopinionated and leaves virtually all the decision making to you. If you're a seasoned engineer, or working with one, this can be a tremendous positive. 

What Knockout **is:** 
- Awesome.
- Focussed. It offers several core concepts, that serve as a foundation for building modern data-drive client applications.
- Adaptive. It is small enough to be justifiable on small projects, and powerful enough to use on large applications.
- Empowering. Using Knockout puts the decisions in your hands, you will build it all and thus understand it all. My favorite analogy is a carpenter building a piece of furniture vs. an individual putting together a piece of Ikea furniture. 
- Organized. Once you begin thinking like a "Knockout" programmer, you can achieve a wonderful separation of your view logic, event handlers, components and data bindings.


As someone who uses Knockout daily, I am admittedly biased toward the advantages of the small-but-mighty library. But the power in the small concepts it exposes, are undeniably amazing. I highly recommend giving it a `self.peek()` on your next project.
