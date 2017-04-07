---
slug: e-commerce-minor-units
title: Why Minor Units are a Smart Choice for E-commerce Solutions
template: post.hbs
date: 2016-03-10
author: Pim Brouwers
tags: E-commerce
---
E-commerce websites are among the most common types of web applications around on the internet today. It is estimated that in 2016, e-commerce sites as a collective, will account for $1.155 trillion USD in global revenue. A growth of nearly 50% since 2013.

There are many technologies available to get an e-commerce site up and running quickly. Services like Shopify and Magento for example. These services are fantasic, so long as you implementation is very straight forward. But there are instances where trying to implement them will feel like putting a square peg in a round hole. Presenting a perfect opportunity to develop a bespoke e-commerce platform. This approach is indeed more flexible, but isn't without it's concerns.

One of the more common challengers faced when developing a custom e-commerce implementation is deciding how to store monetary values. You might ask yourself "wouldn't you just store the values as they're represented in real life? (ie: $99.99)". The short answer to that question is, no. And the reason for this is the fundamental flaw of floating point arithmetic.

The problem stems from the fact that floats and doubles cannot accurately represent the base 10 multiples that we use for money. This isn't an explicitly technical article, so we won't get caught up too much in why this happens just that it does. Beyond rounding errors, arithmetic done using integers requires far fewer processing cycles which equates to faster calculations.

Now that we've decided to store our currency in minor units, it's time to pick a rounding strategy. It's inevitable that when you're performing calculations on dollar values, even in minor units, that you will need to round at some point. The question you need to ask yourself is, do you want to keep the extra for yourself or be a nice guy and let the customer keep it? Whichever direction you choose, it's important to stick with it throughout your application. From a coding point of view I prefer to round down as preference because the floor method in most languages tends to be much more reliable that a ceiling method.

So there you have it. In no way is this article designed to be doctrine for creating a trouble-free finance system for your e-commerce store, but will hopefully help guide you to developing a solid starting point and hopefully avoid a few hours of head-scratching.
