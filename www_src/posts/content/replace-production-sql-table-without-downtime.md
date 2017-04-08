---
slug: replace-production-sql-table-without-downtime
title: How to Replace a Production SQL Table Without Downtime (Almost)
template: post.hbs
date: 2016-03-11
author: Pim Brouwers
tags: SQL
---

There are instances when a production SQL table, or the data within it, needs to be modified in some way and significant downtime is not an option. I was recently confronted with this situation when a rogue script/code had injected millions (literally) of duplicate records into one of our production system tables.

My first thought was to write a simple "DELETE FROM..." query to expunge the duplicates, but given the sheer number of rows, the writes to the transaction log and of course the indexes/constraints on the table, this option would have an extensive execution time. Factoring in that downtime wasn't acceptable in this case, I had to come up with a solution that would essentially be "instantaneous". Here's what I came up with:

To begin, we need to recreate the table.
```sql
CREATE TABLE [dbo].[tmp_YourTable](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[someCol1] [int] NOT NULL,
	[someCol2] [int] NOT NULL,
	[someCol3] [int] NOT NULL,	
 CONSTRAINT [tmp_YourTable_PK_v2] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [dbo].[tmp_YourTable]  WITH CHECK ADD  CONSTRAINT [SomeOtherTable_tmp_YourTable_FK1_v2] FOREIGN KEY([someCol1])
REFERENCES [dbo].[SomeOtherTable]([someCol1])
GO

ALTER TABLE [dbo].[tmp_YourTable] CHECK CONSTRAINT [SomeOtherTable_tmp_YourTable_FK1_v2]
GO
```
Next, we need to write a query to get the unique records from our original table and insert them into our new table. It's critical to enable IDENTITY_INSERT to allow us to write values to the PK.
```sql
SET IDENTITY_INSERT tmp_YourTable ON

INSERT INTO tmp_YourTable (id, someCol1, someCol2, someCol3)
select max(id) as id, pageItemID, someCol1, someCol2, someCol3
from YourTable with (nolock)
GROUP BY id, someCol1, someCol2, someCol3

SET IDENTITY_INSERT tmp_YourTable OFF
GO
```
Finally, we do the always scary table rename.
```sql
BEGIN TRANSACTION;  
	DROP TABLE [dbo].[YourTable]
	GO

	EXEC sp_rename 'tmp_YourTable', 'YourTable';
COMMIT TRANSACTION;
```
And there you have it, we swapped a production table with little to no down time whatsoever.

