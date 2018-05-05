drop table if exists PostTag;
drop table if exists Tag;
drop table if exists Post;

create table Post (
	Id integer primary key autoincrement
	,Title nvarchar(100) not null
	,Tldr nvarchar(255)
	,Body nvarchar(4000) not null
	,Created datetime default datetime
);

create table Tag (
  Id integer primary key autoincrement
  ,Label nvarchar(255) not null
);

create table PostTag (
  PostId integer
  ,TagId integer
  ,primary key (PostId, TagId)
);

create index ix_tag_label on Tag(Label);