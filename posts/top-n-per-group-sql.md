---
slug: top-n-per-group-sql
title: Solving Top-n Per Group in SQL
template: post.hbs
date: 2015-04-18
author: Pim Brouwers
tags: SQL
---
A common problem you run into with normalized DBs is top-n-per-group situation. A good example is:

Suppose you have a table of customers and a table of purchases. Each purchase belongs to one customer and you want to get a list of all customers along with their last purchase.

I used to solve this problem with a subquery as a derived table for the inner join, looking for the max ID. However, I came across this solution today, and it’s AWESOMELY simple. Take a look:
```sql
select *
from customers c
inner join purchases p1
	on p1.CustomerID = c.ID
left join purchases p2
	on p2.CustomerID = c.ID
	and       
	(
		p1.ID < p2.ID
	)
where c.ID = 1 and p2.ID is null
```