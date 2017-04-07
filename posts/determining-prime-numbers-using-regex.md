---
slug: determining-prime-numbers-using-regex
title: Determining Prime Numbers using RegEx
template: post.hbs
date: 2015-11-24
author: Pim Brouwers
tags: RegEx
---
This post is about programmatically determining if a number is prime. To begin, what is a prime number? A prime number is a number that is only evenly-divisible (ie: producing a natural number) by itself and 1. In sequence, the first natural prime, is 2.

From a programmatic stand point there are many ways to solve this problem. Most notably using iteration. For example, let's considering the number 7. Using iteration, we could loop through all the numbers between 1 and 7 and test whether the product is natural. I've used this method myself and it works quite well. I came across this solution today, which uses regular expressions and it's amazing to say the least (the following implementation is using PHP):

```php	
function isPrime($number) {
    return !preg_match('/^1?$|^(11+?)\1+$/x', str_repeat('1', $number));
}
```	
The regular expression tests a sequence of 1's, equally the count of the number passed into the method. Let's breakdown the regular expression step by step:

```	
/
  ^1?$   # matches beginning, optional 1, ending.
         # thus matches the empty string and "1".
         # this matches the cases where N was 0 and 1
         # and since it matches, will not flag those as prime.
|   # or...
  ^                # match beginning of string
    (              # begin first stored group
     1             # match a one
      1+?          # then match one or more ones, minimally.
    )              # end storing first group
    \1+            # match the first group, repeated one or more times.
  $                # match end of string.
/x
```	
So, you can see how this process is analogous to trying to divide a number by successively larger divisors, leaving no remainder. In the case of a prime number, this is never going to succeed.

