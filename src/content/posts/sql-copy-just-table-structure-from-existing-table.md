---
slug: sql-copy-just-table-structure-from-existing-table
title: SQL: How to copy just the structure from an existing table
template: post.hbs
date: 2015-03-13
author: Pim Brouwers
tags: SQL
---
You might find yourself in a situation where you want to clone an SQL table, but leave the existing data behind.

Here's how you would copy just the structure of an existing SQL table:
```sql
CREATE TABLE new_table
AS (SELECT * FROM old_table WHERE 1=2);
```
This would create a new table called "new_table" that includes all columns from the "old_table" table, but no data from the "new_table" table.