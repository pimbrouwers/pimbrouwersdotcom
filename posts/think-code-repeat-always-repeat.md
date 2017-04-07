---
slug: think-code-repeat-always-repeat
title: Think, Code, Repeat. Always Repeat.
template: post.hbs
date: 2016-10-18
author: Pim Brouwers
tags: Productivity
---
Just like Homer taught us all those years ago, the key to rich, full hair is: ["Lather, rinse and repeat. Always repeat."](https://www.youtube.com/watch?v=_HwcYEsXtdk) Writing  ~~software~~  good software is accomplished in a similar fashion. Whereby you **think** of a solution to a problem, make the problem worse by writing **code**, insert a couple keyboard smashes and **repeat** until your code actually solves the problem, which inevtiably will look nothing like your initial solution. An average day for a working programmer.

The aspect of this process I've found myself fixated on lately, is the repetition. More specifically the impact it can have on your productivity, when it becomes a "wait... then repeat". Given that we all work using some form of tooling, you'll invariably come up against a situation where the tool is getting in the way of your productivity. When you notice this happening, which will usually manifest as feelings of impatience or frustration, it's important to stop yourself and attempt to address this problem.

The tooling I rely on everyday is Visual Studio, which is well known to be a bit of a boon when it comes to performance, most notably when the project scales. I spent years foolishly accepting that this was just "part of being a .NET developer". It wasn't until I got stuck for an entire day trying to accomplish what should've been a simple task, that I noticed the impact of waiting before repeating. It was absolutely destroying my thought patterns, and I had enough. 

I cracked open the old Google machine, and got to researching. It took sometime to wade through all of the non-sense, but somewhere along the way I compiled the correct configuration of little fixes to make an astounding impact (the most valueable source was on [MSDN](https://blogs.msdn.microsoft.com/visualstudioalm/2015/03/03/make-debugging-faster-with-visual-studio/) itself). To give a reference, on one of the larger projects I work on daily, the build time went from 45-60s down to under 5s. Even more impressive, was the amount of time to launch the debugger, which was now easily under 5s. 

But enough with the technicalities. The entire point of this rambling is to encourage you not to let the tools that you rely on to get in your way. When that means ditching your tool in favor of another, or finding a way to optimize your existing tool, don't quit. You'll be happy you didnt.

